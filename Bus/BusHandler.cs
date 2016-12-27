using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Common;
using Newtonsoft.Json;

namespace Bus
{
    public class Command
    {
        public string PlaneId { get; set; }
        public bool isUnload { get; set; }
        public Zone Hangar { get; set; }

        public Command(string planeid, bool isunload, Zone hangar)
        {
            PlaneId = planeid;
            isUnload = isunload;
            Hangar = hangar;

        }
        public Command() { }

    }

    public static class Bus
    {

        public const int MAX_COUNT_OF_PASSENGERS = 6;
        public const int TAKE_PASSENGERS = 2;
        public const int TIME_TO_SLEEP = 1000;
        public static int PassengersCount = 0;
        public static Zone CurrentZone = Zone.BUS_STATION;
        public static EntityState State = EntityState.WAITING_FOR_COMMAND;
        public static List<Command> Commands = new List<Command>();
        public static List<string> Passengers = new List<string>();

    }
    public static class BusHandler
    {
        private static Thread BusHandlerThread;
        static BusHandler()
        {
            BusHandlerThread = new Thread(HandleBus);
            BusHandlerThread.Start();
        }

        private static void HandleBus()
        {
            while (true)
            {
                if (Bus.Commands.Count == 0)//если список команд пуст
                {
                    Thread.Sleep(1000);
                    continue;
                }
                if (Bus.State == EntityState.MOVING)
                {
                    continue;
                }

                if (Bus.CurrentZone == Zone.BUS_STATION)//если на автобусной станции
                {
                    if (Bus.Commands[0].isUnload == true)//если на разгрузке
                    {
                        if (Bus.PassengersCount > 0)//если еще есть пассажиры внутри
                        {
                            Thread.Sleep(Bus.TIME_TO_SLEEP);
                            //TODO вставить вместо трех точек верный метод пассажиров на разгрузку 2 пассажиров
                            //вызываем пассажиров и говорю что разгружаю два пассажира- параметр айдишники
                            string unloadbus = "...?id1=" + Bus.Passengers[0];
                            if (Bus.PassengersCount >= 2)//если есть два пассажира как минимум
                            {
                                unloadbus += "&id2=" + Bus.Passengers[1];

                            }
                            else           //если остался один пассажир
                            {
                                unloadbus += "&id2=" + "0";
                            }
                            Util.MakeRequest(unloadbus);
                            //удаление пассажиров из списка в автобусе   
                            lock (Bus.Passengers)
                            {
                                if (Bus.PassengersCount >= 2)
                                {
                                    Bus.Passengers.RemoveAt(1);
                                    Bus.PassengersCount--;
                                }
                                Bus.Passengers.RemoveAt(0);
                                Bus.PassengersCount--;
                            }
                            continue;
                        }
                        if (Bus.PassengersCount == 0)//если автобус пуст

                        {
                            //TODO Вcтавить верный запрос к СДК с вопросом завершил ли самолет с данным рейсом разгрузку 
                            string checkSDK = "...?planeID=" + Bus.Commands[0].PlaneId;
                            string checkresult = Util.MakeRequest(checkSDK);
                            bool unloadfinished = JsonConvert.DeserializeObject<bool>(checkresult);
                            if (unloadfinished) //если разгрузка закончена(самолет пуст)
                            {
                                //завершаем действие
                                lock (Bus.Commands)
                                {
                                    Bus.Commands.RemoveAt(0);
                                }
                                continue;
                            }
                            else
                            {
                                //TODO запрос визуализатору чтобы довез до нужного ангара, передаю свой entity и ангар
                                Bus.State = EntityState.MOVING;
                                string moveme = "...?entity=" + Entity.BUS + "&zone=" + Bus.Commands[0].Hangar;
                                Util.MakeRequest(moveme);
                                continue;
                            }
                        }

                    }
                    else if (Bus.Commands[0].isUnload == false)// если мы загружаем самолет
                    {
                        if (Bus.PassengersCount == Bus.MAX_COUNT_OF_PASSENGERS) //если автобус заполнен
                        {
                            //TODO запрос визуализатору чтобы довез до нужного ангара, передаю свой entity и ангар
                            Bus.State = EntityState.MOVING;
                            string moveme = "...?entity=" + Entity.BUS + "&zone=" + Bus.Commands[0].Hangar;
                            Util.MakeRequest(moveme);
                            continue;
                        }
                        else if (Bus.PassengersCount < Bus.MAX_COUNT_OF_PASSENGERS)
                        {
                            Thread.Sleep(Bus.TIME_TO_SLEEP);
                            //TODO вставить вместо трех точек верный метод пассажиров на загрузку n пассажиров у нас 2
                            //вызываем пассажиров и говорю что загружаю два пассажира- параметр число загружаемых пассажиров ( 2 или меньше 2)
                            string loadbus = "...?n=" + Bus.TAKE_PASSENGERS;
                            string loadbusresult = Util.MakeRequest(loadbus);
                            List<string> loadedpassengers = JsonConvert.DeserializeObject<List<string>>(loadbusresult);
                            //добавляем их в автобус
                            foreach (string pass in loadedpassengers)
                            {
                                Bus.Passengers.Add(pass);
                                Bus.PassengersCount++;
                            }
                            if (loadedpassengers.Count < Bus.TAKE_PASSENGERS)//если пассажиры на загрузку закончились
                            {
                                //TODO запрос визуализатору чтобы довез до нужного ангара, передаю свой entity и ангар
                                Bus.State = EntityState.MOVING;
                                string moveme = "...?entity=" + Entity.BUS + "&zone=" + Bus.Commands[0].Hangar;
                                Util.MakeRequest(moveme);
                                continue;
                            }
                            else continue; //если еще не закончились пассажиры на загрузку

                        }
                    }

                }
                else if (Bus.CurrentZone == Zone.HANGAR_1 || Bus.CurrentZone == Zone.HANGAR_2)//если автобус в ангаре
                {
                    if (Bus.Commands[0].isUnload == true)//если на разгрузке
                    {
                        if (Bus.PassengersCount == Bus.MAX_COUNT_OF_PASSENGERS) //если автобус заполнен
                        {
                            //TODO запрос визуализатору чтобы довез до автобусной станции, передаю свой entity и куда везти
                            Bus.State = EntityState.MOVING;
                            string moveme = "...?entity=" + Entity.BUS + "&zone=" + Zone.BUS_STATION;
                            Util.MakeRequest(moveme);
                            continue;
                        }
                        else if (Bus.PassengersCount < Bus.MAX_COUNT_OF_PASSENGERS)
                        {
                            Thread.Sleep(Bus.TIME_TO_SLEEP);
                            //TODO вставить вместо трех точек верный метод самолета на выгрузку из самолета в автобус n пассажиров у нас 2
                            //вызываем самолет и говорю что загружаю два пассажира- параметр число загружаемых пассажиров ( 2 или меньше 2)
                            string loadbus = "...?n=" + Bus.TAKE_PASSENGERS;
                            string loadbusresult = Util.MakeRequest(loadbus);
                            List<string> loadedpassengers = JsonConvert.DeserializeObject<List<string>>(loadbusresult);
                            //добавляем их в автобус
                            foreach (string pass in loadedpassengers)
                            {
                                Bus.Passengers.Add(pass);
                                Bus.PassengersCount++;
                            }
                            if (loadedpassengers.Count < Bus.TAKE_PASSENGERS)//если пассажиры на выгрузку из самолета закончились
                            {
                                //TODO запрос визуализатору чтобы довез до автобусной станции, передаю свой entity и куда везти
                                Bus.State = EntityState.MOVING;
                                string moveme = "...?entity=" + Entity.BUS + "&zone=" + Zone.BUS_STATION;
                                Util.MakeRequest(moveme);
                                continue;
                            }
                            else continue; //если еще не закончились пассажиры на загрузку
                        }
                    }
                    else if (Bus.Commands[0].isUnload == false)// если мы загружаем самолет
                    {
                        if (Bus.PassengersCount > 0)//если еще есть пассажиры внутри
                        {
                            Thread.Sleep(Bus.TIME_TO_SLEEP);
                            //TODO вставить вместо трех точек верный метод самолета на загрузку в него 2 пассажиров (параметр айдишники)
                            string unloadbus = "...?id1=" + Bus.Passengers[0];
                            if (Bus.PassengersCount >= 2)//если есть два пассажира как минимум
                            {
                                unloadbus += "&id2=" + Bus.Passengers[1];

                            }
                            else           //если остался один пассажир
                            {
                                unloadbus += "&id2=" + "0";
                            }
                            Util.MakeRequest(unloadbus);
                            //удаление пассажиров из списка в автобусе   
                            lock (Bus.Passengers)
                            {
                                if (Bus.PassengersCount >= 2)
                                {
                                    Bus.Passengers.RemoveAt(1);
                                    Bus.PassengersCount--;
                                }
                                Bus.Passengers.RemoveAt(0);
                                Bus.PassengersCount--;
                            }
                            continue;
                        }
                        if (Bus.PassengersCount == 0)//если автобус пуст

                        {
                            //TODO запрос визуализатору чтобы довез до автобусной станции, передаю свой entity и куда везти
                            Bus.State = EntityState.MOVING;
                            string moveme = "...?entity=" + Entity.BUS + "&zone=" + Zone.BUS_STATION;
                            Util.MakeRequest(moveme);
                            continue;

                        }

                    }
                }
                //добавлять действия сюда
            }

        }
    }
}