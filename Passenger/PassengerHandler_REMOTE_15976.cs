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

        private static string log = "passenger_handler_log.txt";

        static PassengerHandler()
        {
            PassengerHandlerThread = new Thread(HandlePassengers);
            PassengerHandlerThread.Start();
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

                        //если пассажир на точке спавна в аэропорту
                        if (passenger.CurrentZone == Zone.PASSENGER_SPAWN)
                        {
                            //если пассажир уходит (отказ или прилЄт)
                            if (passenger.RegState == RegistrationState.EXITING || passenger.RegState == RegistrationState.REJECTED)
                            {
                                //TODO: сделать запрос визуализатору DESPAWN
                                Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
                            }
                            else //если пассажир только по¤вилс¤
                            {
                                //TODO: отправить визуализатору SPAWN
                                passenger.State = EntityState.MOVING;
                                //TODO: сделать запрос визуализатору на движение к регистрационной стойке
                                passenger.CurrentZone = Zone.REGISTRATION_STAND;
                                passenger.State = EntityState.WAITING_FOR_COMMAND;
                            }
                            //continue;
                        }

                        //если пассажир находитс¤ у стойки регистрации
                        if (passenger.CurrentZone == Zone.REGISTRATION_STAND)
                        {
                            //если пассажир незарегистрирован
                            if (passenger.RegState == RegistrationState.NOT_REGISTERED)
                            {
                                //если пассажир улетающий
                                if (passenger.IsFlyingAway)
                                {
                                    passenger.State = EntityState.STANDING_BY;
                                    //запрос регистрации
                                    //TODO: сделать нормально запрос
                                    string urlParameters = "?flightId=" + passenger.FlightId + "&passengerId=" +
                                                           passenger.Id + "&cargo=" + passenger.CargoCount;
                                    string url = "http://localhost:" + Common.Ports.RegStand +
                                                 "/RegistrationStandService.svc/" + "Register";
                                    string result = Common.Util.MakeRequest(url + urlParameters);
                                    bool response = JsonConvert.DeserializeObject<bool>(result);

                                    passenger.RegState = !response
                                        ? RegistrationState.REJECTED
                                        : RegistrationState.REGISTERED;
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                                else //если пассажир прилетающий
                                {
                                    //так не должо быть
                                    Common.Util.Log(log,
                                        "Passenger " + passenger.Id + " is't flying away, but go to registration!");
                                    Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
                                    passenger.RegState = RegistrationState.EXITING;
                                    passenger.State = EntityState.MOVING;
                                    //TODO: сделать запрос визуализатору на перемещение к спауну
                                    passenger.CurrentZone = Zone.PASSENGER_SPAWN;
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                            }

                            //если пассажиру отказано в регистрации
                            if (passenger.RegState == RegistrationState.REJECTED)
                            {
                                passenger.State = EntityState.MOVING;
                                //TODO: сделать запрос визуализатору на перемещение к спауну
                                passenger.CurrentZone = Zone.PASSENGER_SPAWN;
                                passenger.State = EntityState.WAITING_FOR_COMMAND;
                            }

                            //если пассажир уже зарегистрирован
                            if (passenger.RegState == RegistrationState.REGISTERED)
                            {
                                //если пассажир улетающий
                                if (passenger.IsFlyingAway)
                                {
                                    passenger.State = EntityState.MOVING;
                                    //TODO: сделать запрос визуализатору на движение в зону багажа
                                    passenger.CurrentZone = Zone.CARGO_DROPOFF;
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                                else //если пассажир прилетающий
                                {
                                    //так не должо быть
                                    Common.Util.Log(log, "Passenger " + passenger.Id + " is't flying away, but go to registration!");
                                    Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
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
                                passenger.HoldingCargo = false;
                                passenger.State = EntityState.MOVING;
                                //TODO: сделать запрос визуализатору на движение в зону ожидани¤
                                passenger.CurrentZone = Zone.WAITING_AREA;
                                passenger.State = EntityState.WAITING_FOR_COMMAND;
                            }
                            else //если пассажир прилетающий
                            {
                                passenger.HoldingCargo = true;
                                passenger.RegState = RegistrationState.EXITING;
                                passenger.State = EntityState.MOVING;
                                //TODO: сделать запрос визуализатору на перемещение к спауну
                                passenger.CurrentZone = Zone.PASSENGER_SPAWN;
                                passenger.State = EntityState.WAITING_FOR_COMMAND;
                            }
                            //continue;
                        }

                        //если пассажир на автобусной остановке
                        if (passenger.CurrentZone == Zone.BUS_STATION)
                        {
                            //если пассажир прилетающий
                            if (!passenger.IsFlyingAway)
                            {
                                //TODO: отправить визуализатору SPAWN
                                passenger.State = EntityState.MOVING;
                                //TODO: сделать запрос визуализатору на движение в зону ожидани¤
                                passenger.CurrentZone = Zone.WAITING_AREA;
                                passenger.State = EntityState.WAITING_FOR_COMMAND;
                            }
                            //continue;
                        }

                        //если пассажир находитс¤ в зоне ожидани¤
                        if (passenger.CurrentZone == Zone.WAITING_AREA)
                        {
                            //если пассажир зарегистрирован
                            if (passenger.RegState == RegistrationState.REGISTERED)
                            {
                                //если пассажир улетающий
                                if (passenger.IsFlyingAway)
                                {
                                    passenger.State = EntityState.MOVING;
                                    //TODO: запрос визуализатору на движение куда-то в аэропорту
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                    continue;
                                }
                                else //прилетающий
                                {
                                    passenger.State = EntityState.MOVING;
                                    //TODO: сделать запрос визуализатору на движение к выдаче багажа
                                    passenger.CurrentZone = Zone.CARGO_DROPOFF;
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                            }
                            else
                            {
                                //так не должно быть
                                Common.Util.Log(log, "Passenger " + passenger.Id + " in waiting_area, but not registered!");
                                Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
                            }
                            //continue;
                        }
                    }
                }
            }
        }
    }
}