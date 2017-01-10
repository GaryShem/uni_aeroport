using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Common;
using Newtonsoft.Json;

namespace Passenger
{
    public static class PassengerHandler
    {
        public static List<Common.Passenger> Passengers = new List<Common.Passenger>();
        private static Thread PassengerHandlerThread;

        static PassengerHandler()
        {
            PassengerHandlerThread = new Thread(HandlePassengers);
            PassengerHandlerThread.Start();
        }

        private static void DespawnPassenger(Common.Passenger passenger)
        {
            string URL = String.Format("http://localhost:{0}/VisualizerService.svc/Despawn?id={1}",
                                    Ports.Visualizer, passenger.Id);
            Util.MakeRequest(URL);
            Passengers.Remove(Passengers.Find(x => x.Id.Equals(passenger.Id)));
        }

        private static void SpawnPassenger(Common.Passenger passenger)
        {
            string URL = String.Format("http://localhost:{0}/VisualizerService.svc/Spawn?type={1}&id={2}&zone={3}&cargo={4}",
                                    Ports.Visualizer, (int)Entity.PASSENGER, passenger.Id, (int)passenger.CurrentZone, passenger.CargoCount);
            Util.MakeRequest(URL);
        }

        private static void MovePassenger(Common.Passenger passenger, Zone zone)
        {
            string URL = String.Format("http://localhost:{0}/VisualizerService.svc/Move?type={1}&id={2}&zone={3}",
                                    Ports.Visualizer, (int)Entity.PASSENGER, passenger.Id, (int)zone);
            Util.MakeRequest(URL);
            passenger.State = EntityState.MOVING;
        }

        private static void RegisterPassenger(Common.Passenger passenger)
        {
            string URL = String.Format("http://localhost:{0}/RegistrationStandService.svc/Register?flightId={1}&passengerId={2}&cargo={3}",
                                            Ports.RegStand, passenger.FlightId, passenger.Id, passenger.CargoCount);
            string result = Common.Util.MakeRequest(URL);
            bool response = JsonConvert.DeserializeObject<bool>(result);

            passenger.RegState = response ? RegistrationState.REGISTERED : RegistrationState.REJECTED;
            passenger.State = EntityState.WAITING_FOR_COMMAND;
        }

        private static PlaneServiceStage CheckPlaneStage(string flightId)
        {
            string URL = String.Format("{0}/CheckStage?id={1}", ServiceStrings.GrControl, flightId);
            string response = Util.MakeRequest(URL);
            PlaneServiceStage result = (PlaneServiceStage)JsonConvert.DeserializeObject<int>(response);
            return result;
        }

        public static void HandlePassengers()
        {
            while (true)
            {
                Thread.Sleep(1000);
                lock (Passengers)
                {
                    for (int i = 0; i < Passengers.Count; i++)
                    {
                        Common.Passenger passenger = Passengers[i];

                        // если пассажир выполн¤ет дей¤твие, то пропускаем его
                        if (passenger.State == EntityState.MOVING)
                        {
                            continue;
                        }
                        if (passenger.CurrentZone == Zone.BUS || passenger.CurrentZone == Zone.PLANE)
                        {
                            continue;
                        }

                        //если пассажир на точке спавна в аэропорту
                        if (passenger.CurrentZone == Zone.PASSENGER_SPAWN)
                        {
                            //если пассажир уходит (отказ или прилЄт)
                            if (passenger.RegState == RegistrationState.EXITING || passenger.RegState == RegistrationState.REJECTED || passenger.IsFlyingAway == false)
                            {
                                DespawnPassenger(passenger);
                            }
                            else //если пассажир только по¤вилс¤
                            {
                                SpawnPassenger(passenger);
                                MovePassenger(passenger, Zone.REGISTRATION_STAND);
                            }
                            //continue;
                        }
                        else if (passenger.CurrentZone == Zone.REGISTRATION_STAND)
                        {
                            //если пассажир незарегистрирован
                            if (passenger.RegState == RegistrationState.NOT_REGISTERED)
                            {
                                //если пассажир улетающий
                                if (passenger.IsFlyingAway)
                                {
                                    passenger.State = EntityState.STANDING_BY;
                                    //запрос регистрации
                                    RegisterPassenger(passenger);
                                }
                                else //если пассажир прилетающий
                                {
                                    //TODO: сделать запрос визуализатору на перемещение к спауну
                                    MovePassenger(passenger, Zone.PASSENGER_SPAWN);
//                                    passenger.CurrentZone = Zone.PASSENGER_SPAWN;
//                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                            }
                            //если пассажиру отказано в регистрации
                            else if (passenger.RegState == RegistrationState.REJECTED)
                            {
                                //TODO: сделать запрос визуализатору на перемещение к спауну
                                MovePassenger(passenger, Zone.PASSENGER_SPAWN);
//                                passenger.CurrentZone = Zone.PASSENGER_SPAWN;
//                                passenger.State = EntityState.WAITING_FOR_COMMAND;
                            }
                            //если пассажир уже зарегистрирован
                            else if (passenger.RegState == RegistrationState.REGISTERED)
                            {
                                //если пассажир улетающий
                                if (passenger.IsFlyingAway)
                                {
                                    //TODO: сделать запрос визуализатору на движение в зону багажа
                                    MovePassenger(passenger, Zone.CARGO_DROPOFF);
//                                    passenger.CurrentZone = Zone.CARGO_DROPOFF;
//                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                                else //если пассажир прилетающий
                                {
                                    MovePassenger(passenger, Zone.PASSENGER_SPAWN);
                                }
                            }
                            //continue;
                        }

                        //пассажир находитс¤ у стойки с багажом
                        if (passenger.CurrentZone == Zone.CARGO_DROPOFF)
                        {
                            //если пассажир улетающий
                            if (passenger.IsFlyingAway)
                            {
                                if (passenger.HoldingCargo)
                                {
                                    passenger.HoldingCargo = false;
                                }
                                else
                                {
                                    MovePassenger(passenger, Zone.WAITING_AREA);
                                }
                            }
                            else //если пассажир прилетающий
                            {
                                if (passenger.HoldingCargo)
                                {
                                    MovePassenger(passenger, Zone.REGISTRATION_STAND);
                                }
                                else
                                {
                                    passenger.HoldingCargo = true;
                                }
                            }
                        }
                        //если пассажир на автобусной остановке
                        else if (passenger.CurrentZone == Zone.BUS_STATION)
                        {
                            //если пассажир прилетающий
                            if (!passenger.IsFlyingAway)
                            {
                                MovePassenger(passenger, Zone.WAITING_AREA);
                            }
                            //continue;
                        }
                        //если пассажир находитс¤ в зоне ожидани¤
                        else if (passenger.CurrentZone == Zone.WAITING_AREA)
                        {
                            //если пассажир зарегистрирован
//                            if (passenger.RegState == RegistrationState.REGISTERED)
//                            {
                                //если пассажир улетающий
                                if (passenger.IsFlyingAway)
                                {
                                    // ждём, пока автобус его запросит
                                    continue;
                                }
                                else //прилетающий
                                {
                                    var planeStage = CheckPlaneStage(passenger.FlightId);
                                    if (planeStage != PlaneServiceStage.UNLOAD_PASSENGERS && planeStage != PlaneServiceStage.UNLOAD_CARGO)
                                    {
                                        MovePassenger(passenger, Zone.CARGO_DROPOFF);
                                    }
                                }
//                            }
//                            else
//                            {
//                                //так не должно быть
//                                DespawnPassenger(passenger);
//                                Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
//                            }
                            //continue;
                        }
                    }
                }
            }
        }
    }
}