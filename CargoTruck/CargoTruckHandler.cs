using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Common;
using Newtonsoft.Json;

namespace CargoTruck
{
    public static class CargoTruckHandler
    {
        public static Common.CargoTruck _CargoTruck;
        private static Thread FuelTruckHandlerThread;

        static CargoTruckHandler()
        {
            _CargoTruck = new Common.CargoTruck();
            FuelTruckHandlerThread = new Thread(HandleCargoTruck);
            FuelTruckHandlerThread.Start();
        }

        private static PlaneServiceStage CheckPlaneStage(string flightId)
        {
            string URL = String.Format("{0}/CheckStage?id={1}", ServiceStrings.GrControl, flightId);
            string response = Util.MakeRequest(URL);
            PlaneServiceStage result = (PlaneServiceStage)JsonConvert.DeserializeObject<int>(response);
            return result;
        }

        private static void MoveCargoTruck(Zone zone)
        {
            string URL = String.Format("{0}/Move?type={1}&id={2}&zone={3}",
                ServiceStrings.Vis, (int)Entity.CARGO_TRUCK, _CargoTruck.Id, (int)zone);
            string response = Util.MakeRequest(URL);
            _CargoTruck.State = EntityState.MOVING;
        }

        private static void TakeCargoFromPlane()
        {
            string URL = String.Format("{0}/GiveCargoToTruck?flightId={1}&cargo={2}",
                ServiceStrings.Plane, _CargoTruck.CurrentCommand.Item1, Math.Min(Common.CargoTruck.CARGO_CAPACITY - _CargoTruck.CurrentCargo, Common.CargoTruck.LOAD_SPEED));
            string response = Util.MakeRequest(URL);
            int cargoTaken = JsonConvert.DeserializeObject<int>(response);
            _CargoTruck.CurrentCargo = Math.Min(Common.CargoTruck.CARGO_CAPACITY, _CargoTruck.CurrentCargo + cargoTaken);
            UpdateCargo();
        }

        private static void GiveCargoToPlane()
        {
            int cargoToUnload = Math.Min(Common.CargoTruck.LOAD_SPEED, _CargoTruck.CurrentCargo);
            string URL = String.Format("{0}/TakeCargoFromTruck?flightId={1}&cargo={2}",
                ServiceStrings.Plane, _CargoTruck.CurrentCommand.Item1, cargoToUnload);
            Util.MakeRequest(URL);
            _CargoTruck.CurrentCargo -= cargoToUnload;
            UpdateCargo();
        }

        private static void SpawnCargoTruck()
        {
            string URL = String.Format("{0}/Spawn?type={1}&id={2}&zone={3}&cargo={4}",
                ServiceStrings.Vis, (int)Entity.CARGO_TRUCK, _CargoTruck.Id, (int)Zone.CARGO_AREA, _CargoTruck.CurrentCargo);
            Util.MakeRequest(URL);
        }

        private static void CompleteCommand()
        {
            _CargoTruck.CurrentCommand = null;
            lock (_CargoTruck.Commands)
            {
                _CargoTruck.Commands.RemoveAt(0);
            }
        }

        private static int GetRemainingCargoToLoad()
        {
            string URL = String.Format("{0}/GetRemainingCargoToLoad?flightId={1}", ServiceStrings.Plane, _CargoTruck.CurrentCommand.Item1);
            string response = Util.MakeRequest(URL);
            int remainingCargo = JsonConvert.DeserializeObject<int>(response);
            return remainingCargo;
        }

        private static void UpdateCargo()
        {
            string URL = String.Format("{0}/UpdateCargo?id={1}&cargoCount={2}",
                ServiceStrings.Vis, _CargoTruck.Id, _CargoTruck.CurrentCargo);
            Util.MakeRequest(URL);
        }

        public static void HandleCargoTruck()
        {
            SpawnCargoTruck();
            while (true)
            {
                Thread.Sleep(1000);
                if (_CargoTruck.State == EntityState.MOVING)
                {
                    continue;
                }
                if (_CargoTruck.Commands.Count == 0)
                {
                    continue;
                }
                if (_CargoTruck.CurrentCommand == null)
                {
                    _CargoTruck.CurrentCommand = _CargoTruck.Commands[0];
                }

                var planeStage = CheckPlaneStage(_CargoTruck.CurrentCommand.Item1);

                if (_CargoTruck.CurrentZone == Zone.CARGO_AREA)
                {
                    if (_CargoTruck.CurrentCommand.Item3 == PlaneServiceStage.UNLOAD_CARGO)
                    {
                        if (_CargoTruck.CurrentCargo > 0) //если ещё остался груз на выгрузку в тележке
                        {
                            _CargoTruck.CurrentCargo = Math.Max(_CargoTruck.CurrentCargo-Common.CargoTruck.LOAD_SPEED, 0);
                            UpdateCargo();
                        }
                        else if (planeStage == PlaneServiceStage.UNLOAD_CARGO) //если в самолёте ещё есть груз
                        {
                            MoveCargoTruck(_CargoTruck.CurrentCommand.Item2);
                        }
                        else //если всё разгрузили
                        {
                            CompleteCommand();
                        }
                    }
                    else if (_CargoTruck.CurrentCommand.Item3 == PlaneServiceStage.LOAD_CARGO && planeStage == PlaneServiceStage.LOAD_CARGO)
                    {
                        int remainingCargo = GetRemainingCargoToLoad();
                        if (_CargoTruck.CurrentCargo >= Common.CargoTruck.CARGO_CAPACITY ||
                            remainingCargo - _CargoTruck.CurrentCargo <= 0 && _CargoTruck.CurrentCargo > 0)
                        {
                            MoveCargoTruck(_CargoTruck.CurrentCommand.Item2);
                        }
                        else if (_CargoTruck.CurrentCargo < Common.CargoTruck.CARGO_CAPACITY && remainingCargo - _CargoTruck.CurrentCargo > 0)
                        {
                            _CargoTruck.CurrentCargo = Math.Min(_CargoTruck.CurrentCargo + Common.CargoTruck.LOAD_SPEED, Common.CargoTruck.CARGO_CAPACITY);
                            _CargoTruck.CurrentCargo = _CargoTruck.CurrentCargo <= remainingCargo ? _CargoTruck.CurrentCargo : remainingCargo;
                            UpdateCargo();
                        }
                        else
                        {
                            _CargoTruck.CurrentCargo = 0;
                            UpdateCargo();
                            CompleteCommand();
                        }
                    }
                    else
                    {
                        //не должны попадать
                        _CargoTruck.CurrentCargo = 0;
                        UpdateCargo();
                        CompleteCommand();
                    }
                }
                else if (_CargoTruck.CurrentZone == Zone.HANGAR_1 || _CargoTruck.CurrentZone == Zone.HANGAR_2)
                {
                    if (_CargoTruck.CurrentCommand.Item3 == PlaneServiceStage.UNLOAD_CARGO)
                    {
                        if (_CargoTruck.CurrentCargo >= Common.CargoTruck.CARGO_CAPACITY || planeStage != PlaneServiceStage.UNLOAD_CARGO) //если тележка полностью загружена
                        {
                            MoveCargoTruck(Zone.CARGO_AREA);
                        }
                        else if (planeStage == PlaneServiceStage.UNLOAD_CARGO) //если в самолёте ещё есть груз
                        {
                            TakeCargoFromPlane();
                        }
                    }
                    else if (_CargoTruck.CurrentCommand.Item3 == PlaneServiceStage.LOAD_CARGO)
                    {
                        if (_CargoTruck.CurrentCargo == 0)
                        {
                            MoveCargoTruck(Zone.CARGO_AREA);
                        }
                        else if (planeStage != PlaneServiceStage.LOAD_CARGO)
                        {
                            CompleteCommand();
                            MoveCargoTruck(Zone.CARGO_AREA);
                        }
                        else
                        {
                            GiveCargoToPlane();
                        }
                    }
                    else
                    {
                        MoveCargoTruck(Zone.CARGO_AREA);
                        CompleteCommand();
                    }
                }
            }
        }
    }
}