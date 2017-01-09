using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Common;
using Newtonsoft.Json;

namespace Bus
{
    public static class BusHandler
    {
        private static Thread BusHandlerThread;
        public static Common.Bus _bus;
        static BusHandler()
        {
            _bus = new Common.Bus();
            BusHandlerThread = new Thread(HandleBus);
            BusHandlerThread.Start();
        }

        private static void GivePassengersToPlane(string flightId)
        {
            string URL = String.Format("{0}/TakePassengersFromBus?flightId={1}&count={2}", ServiceStrings.Plane, flightId, Common.Bus.TAKE_PASSENGERS);
            Util.MakeRequest(URL);
        }

        private static void TakePassengersFromPlane(string flightId)
        {
            string URL = String.Format("{0}/UnloadPassengers?flightId={1}&count={2}", ServiceStrings.Plane, flightId, Common.Bus.TAKE_PASSENGERS);
            string response = Util.MakeRequest(URL);
            List<string> passengerList = JsonConvert.DeserializeObject<List<string>>(response);
            foreach (string passenger in passengerList)
            {
                _bus.Passengers.Add(passenger);
                URL = String.Format("{0}/CompleteMove?id={1}&zone={2}", ServiceStrings.Passenger, passenger, (int)Zone.BUS);
                Util.MakeRequest(URL);
            }
        }

        private static void GivePassengersToStation()
        {
            string URL = String.Format("{0}/TakePassengersFromBus", ServiceStrings.Passenger);
            Util.MakeRequest(URL);
        }

        private static void TakePassengersFromStation(string flightId)
        {
            //GivePassengersToBus?flightId={flightId}
            string URL = String.Format("{0}/GivePassengersToBus?flightId={1}", ServiceStrings.Passenger, flightId);
            string response = Util.MakeRequest(URL);
            List<string> passengerIdList = JsonConvert.DeserializeObject<List<string>>(response);
            //деспавнит служба пассажиров, поэтому здесь просто добавляем в автобус
            _bus.Passengers.AddRange(passengerIdList);
        }

        private static PlaneServiceStage CheckPlaneStage(string flightId)
        {
            string URL = String.Format("{0}/CheckStage?id={1}", ServiceStrings.GrControl, flightId);
            string response = Util.MakeRequest(URL);
            PlaneServiceStage result = (PlaneServiceStage) JsonConvert.DeserializeObject<int>(response);
            return result;
        }

        private static void MoveBus(Zone zone)
        {
            string URL = String.Format("{0}/Move?type={1}&id={2}&zone={3}",
                ServiceStrings.Vis, (int)Entity.BUS, _bus.Id, (int)zone);
            string response = Util.MakeRequest(URL);
            _bus.State = EntityState.MOVING;
        }

        private static void SpawnBus()
        {
            string URL = String.Format("{0}/Spawn?type={1}&id={2}&zone={3}&cargo={4}",
                                    ServiceStrings.Vis, (int)Entity.BUS, _bus.Id, (int)Zone.BUS_STATION, _bus.Passengers.Count);
            Util.MakeRequest(URL);
        }

        private static void CompleteCommand()
        {
            _bus.CurrentCommand = null;
            lock (_bus.Commands)
            {
                _bus.Commands.RemoveAt(0);
            }
        }

        private static void HandleBus()
        {
            SpawnBus();
            while (true)
            {
                Thread.Sleep(1000);
                if (_bus.Commands.Count == 0)//если список команд пуст
                {
                    continue;
                }
                if (_bus.State == EntityState.MOVING)
                {
                    continue;
                }
                if (_bus.CurrentCommand == null)
                {
                    _bus.CurrentCommand = _bus.Commands[0];
                }

                if (_bus.CurrentZone == Zone.BUS_STATION)//если на автобусной станции
                {
                    if (_bus.CurrentCommand.Item3 == PlaneServiceStage.UNLOAD_PASSENGERS)//если на разгрузке
                    {
                        if (_bus.Passengers.Count > 0)//если еще есть пассажиры внутри
                        {
                            GivePassengersToStation();
                        }
                        else//если автобус пуст
                        {
                            PlaneServiceStage currentPlaneServiceStage = CheckPlaneStage(_bus.CurrentCommand.Item1);
                            if (currentPlaneServiceStage == _bus.CurrentCommand.Item3)
                            {
                                MoveBus(_bus.CurrentCommand.Item2);
                            }
                            else
                            {
                                CompleteCommand();
                            }
                        }

                    }
                    else if (_bus.Commands[0].Item3 == PlaneServiceStage.LOAD_PASSENGERS)// если мы загружаем самолет
                    {
                        if (_bus.Passengers.Count == Common.Bus.MAX_COUNT_OF_PASSENGERS) //если автобус заполнен
                        {
                            //запрос визуализатору чтобы довез до нужного ангара, передаю свой entity и ангар
                            MoveBus(_bus.CurrentCommand.Item2);
                        }
                        else if (_bus.Passengers.Count < Common.Bus.MAX_COUNT_OF_PASSENGERS)
                        {
                            //вставить вместо трех точек верный метод пассажиров на загрузку n пассажиров у нас 2
                            //вызываем пассажиров и говорю что загружаю два пассажира- параметр число загружаемых пассажиров ( 2 или меньше 2)
                            int oldPassengerCount = _bus.Passengers.Count;
                            TakePassengersFromStation(_bus.CurrentCommand.Item1);
                            int newPassengerCount = _bus.Passengers.Count;
                            int loadedPassengerCount = newPassengerCount - oldPassengerCount;
                            if (loadedPassengerCount < Common.Bus.TAKE_PASSENGERS)//если пассажиры на загрузку закончились
                            {
                                if (_bus.Passengers.Count > 0)
                                {
                                    MoveBus(_bus.CurrentCommand.Item2);
                                }
                                else
                                {
                                    CompleteCommand();
                                }
                            }
                        }
                    }

                }
                else if (_bus.CurrentZone == Zone.HANGAR_1 || _bus.CurrentZone == Zone.HANGAR_2)//если автобус в ангаре
                {
                    if (_bus.CurrentCommand.Item3 == PlaneServiceStage.UNLOAD_PASSENGERS)//если на разгрузке
                    {
                        if (_bus.Passengers.Count == Common.Bus.MAX_COUNT_OF_PASSENGERS) //если автобус заполнен
                        {
                            // запрос визуализатору чтобы довез до автобусной станции, передаю свой entity и куда везти
                            MoveBus(Zone.BUS_STATION);
                        }
                        else if (_bus.Passengers.Count < Common.Bus.MAX_COUNT_OF_PASSENGERS)
                        {
                            //вставить вместо трех точек верный метод самолета на выгрузку из самолета в автобус n пассажиров у нас 2
                            //вызываем самолет и говорю что загружаю два пассажира- параметр число загружаемых пассажиров ( 2 или меньше 2)
                            int oldPassengerCount = _bus.Passengers.Count;
                            TakePassengersFromPlane(_bus.CurrentCommand.Item1);
                            int newPassengerCount = _bus.Passengers.Count;
                            int loadedPassengerCount = newPassengerCount - oldPassengerCount;
                            if (loadedPassengerCount < Common.Bus.TAKE_PASSENGERS)//если пассажиры в самолёте закончились
                            {
                                MoveBus(Zone.BUS_STATION);
                            }
                        }
                    }
                    else if (_bus.CurrentCommand.Item3 == PlaneServiceStage.LOAD_PASSENGERS)// если мы загружаем самолет
                    {
                        if (_bus.Passengers.Count > 0)//если еще есть пассажиры внутри
                        {
                            GivePassengersToPlane(_bus.CurrentCommand.Item1);
                        }
                        if (_bus.Passengers.Count == 0)//если автобус пуст
                        {
                            MoveBus(Zone.BUS_STATION);
                        }
                    }
                }
                //добавлять действия сюда
            }

        }
    }
}