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

                        // если пассажир выполняет дейятвие, то пропускаем его
                        if (passenger.State == EntityState.MOVING)
                        {
                            continue;
                        }

                        //если пассажир только-только сгенерирован в аэропорту
                        if (passenger.CurrentZone == Zone.PASSENGER_SPAWN)
                        {
                            //TODO: отправить визуализатору 'отрисовать'? (или это уже где-то в другом месте)
                            passenger.State = EntityState.MOVING;
                            //TODO: сделать запрос визуализатору на движение к регистрационной стойке
                            passenger.CurrentZone = Zone.REGISTRATION_STAND;
                            passenger.State = EntityState.WAITING_FOR_COMMAND;
                        }

                        //если пассажир находится у стойки регистрации
                        if (passenger.CurrentZone == Zone.REGISTRATION_STAND)
                        {
                            //если пассажир незарегистрирован
                            if (passenger.RegState == RegistrationState.NOT_REGISTERED)
                            {
                                if (passenger.IsFlyingAway)
                                {
                                    passenger.State = EntityState.STANDING_BY;
                                    //запрос регистрации
                                    //TODO: сделать нормально запрос
                                    string urlParameters = "?flightId=" + passenger.FlightId + "&passengerId=" + passenger.Id + "&cargo=" + passenger.CargoCount;
                                    string url = "http://localhost:" + Common.Ports.RegStand + "/RegistrationStandService.svc/" + "Register";
                                    string result = Common.Util.MakeRequest(url + urlParameters);
                                    bool response = JsonConvert.DeserializeObject<bool>(result);

                                    passenger.RegState = !response ? RegistrationState.REJECTED : RegistrationState.REGISTERED;
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                                else
                                {
                                    passenger.State = EntityState.MOVING;
                                    //TODO: сделать запрос визуализатору на выход из аэропорта
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                    passenger.RegState = RegistrationState.EXITING;
                                    Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
                                }
                            }

                            if (passenger.RegState == RegistrationState.REJECTED)
                            {
                                passenger.State = EntityState.MOVING;
                                //TODO: сделать запрос визуализатору на выход из аэропорта
                                passenger.State = EntityState.WAITING_FOR_COMMAND;
                                Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
                            }

                            if (passenger.RegState == RegistrationState.REGISTERED)
                            {
                                if (passenger.IsFlyingAway)
                                {
                                    passenger.State = EntityState.MOVING;
                                    //TODO: сделать запрос визуализатору на движение в зону ожидания
                                    passenger.CurrentZone = Zone.WAITING_AREA;
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                                else
                                {
                                    passenger.State = EntityState.MOVING;
                                    //TODO: сделать запрос визуализатору на из аэропорта
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                    passenger.RegState = RegistrationState.EXITING;
                                    Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
                                }
                            }

                            Common.Util.Log(log, "Passenger " + passenger.Id + " removed");
                            Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
                        }

                        //если пассажир находится в зоне ожидания
                        if (passenger.CurrentZone == Zone.WAITING_AREA)
                        {
                            if (passenger.RegState == RegistrationState.REGISTERED)
                            {
                                if (passenger.IsFlyingAway)
                                {
                                    passenger.State = EntityState.MOVING;
                                    //TODO: запрос визуализатору на движение куда-то в аэропорту
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                    continue;
                                }
                                else
                                {
                                    passenger.State = EntityState.MOVING;
                                    //TODO: сделать запрос визуализатору на движение к регистрационной стойке
                                    passenger.CurrentZone = Zone.REGISTRATION_STAND;
                                    passenger.State = EntityState.WAITING_FOR_COMMAND;
                                }
                            }
                            Common.Util.Log(log, "Passenger " + passenger.Id + " removed");
                            Passengers.Remove(Passengers.Find(x => x.Id == passenger.Id));
                        }
                    }
                }
            }
        }
    }
}