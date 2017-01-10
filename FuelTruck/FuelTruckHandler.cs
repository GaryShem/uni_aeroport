using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Common;
using Newtonsoft.Json;

namespace FuelTruck
{
    public static class FuelTruckHandler
    {
        public static Common.FuelTruck _FuelTruck;
        private static Thread FuelTruckHandlerThread;

        static FuelTruckHandler()
        {
            _FuelTruck = new Common.FuelTruck();
            FuelTruckHandlerThread = new Thread(HandleFuelTruck);
            FuelTruckHandlerThread.Start();
        }

        private static PlaneServiceStage CheckPlaneStage(string flightId)
        {
            string URL = String.Format("{0}/CheckStage?id={1}", ServiceStrings.GrControl, flightId);
            string response = Util.MakeRequest(URL);
            PlaneServiceStage result = (PlaneServiceStage)JsonConvert.DeserializeObject<int>(response);
            return result;
        }

        private static void MoveFuelTruck(Zone zone)
        {
            string URL = String.Format("{0}/Move?type={1}&id={2}&zone={3}",
                ServiceStrings.Vis, (int)Entity.FUEL_TRUCK, _FuelTruck.Id, (int)zone);
            string response = Util.MakeRequest(URL);
            _FuelTruck.State = EntityState.MOVING;
        }

        private static void SpawnFuelTruck()
        {
            string URL = String.Format("{0}/Spawn?type={1}&id={2}&zone={3}&cargo={4}",
                ServiceStrings.Vis, (int)Entity.FUEL_TRUCK, _FuelTruck.Id, (int)Zone.FUEL_STATION, _FuelTruck.CurrentFuel);
            Util.MakeRequest(URL);
        }

        private static void CompleteCommand()
        {
            _FuelTruck.CurrentCommand = null;
            lock (_FuelTruck.Commands)
            {
                _FuelTruck.Commands.RemoveAt(0);
            }
        }

        public static void HandleFuelTruck()
        {
            SpawnFuelTruck();
            while (true)
            {
                Thread.Sleep(1000);
                if (_FuelTruck.State == EntityState.MOVING)
                {
                    continue;
                }
                if (_FuelTruck.Commands.Count == 0)
                {
                    continue;
                }
                if (_FuelTruck.CurrentCommand == null)
                {
                    _FuelTruck.CurrentCommand = _FuelTruck.Commands[0];
                }

                if (_FuelTruck.CurrentZone == Zone.FUEL_STATION)
                {
                    var planeStage = CheckPlaneStage(_FuelTruck.CurrentCommand.Item1);
                    if (planeStage != PlaneServiceStage.REFUEL)
                    {
                        CompleteCommand();
                    }
                    else if (_FuelTruck.CurrentFuel >= Common.FuelTruck.FUEL_CAPACITY)
                    {
                        MoveFuelTruck(_FuelTruck.CurrentCommand.Item2);
                    }
                    else
                    {
                        _FuelTruck.CurrentFuel = Math.Min(_FuelTruck.CurrentFuel+Common.FuelTruck.REFUEL_SPEED, Common.FuelTruck.FUEL_CAPACITY);
                    }
                }
                else if (_FuelTruck.CurrentZone == Zone.HANGAR_1 || _FuelTruck.CurrentZone == Zone.HANGAR_2)
                {
                    if (_FuelTruck.CurrentFuel == 0)
                    {
                        MoveFuelTruck(Zone.FUEL_STATION);
                    }
                    else
                    {
                        var planeStage = CheckPlaneStage(_FuelTruck.CurrentCommand.Item1);
                        if (planeStage != PlaneServiceStage.REFUEL)
                        {
                            CompleteCommand();
                            MoveFuelTruck(Zone.FUEL_STATION);
                        }
                        else
                        {
                            RefuelPlane();
                        }
                    }
                }
            }
        }

        private static void RefuelPlane()
        {
            string URL = String.Format("{0}/AcceptFuel?flightId={1}&count={2}", ServiceStrings.Plane, _FuelTruck.CurrentCommand.Item1, Math.Min(_FuelTruck.CurrentFuel, Common.FuelTruck.REFUEL_SPEED));
            Util.MakeRequest(URL);
        }
    }
}