using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Common;
using Newtonsoft.Json;
using RegistrationStand;

namespace Plane
{
    public static class PlaneHandler
    {
        public static List<Common.Plane> Planes { get; set; }
        private static Thread PlaneHandlerThread;

        private static Common.Plane GeneratePlane()
        {
            int passengerCount = RandomGen.Next(1, Common.Plane.PassengerCapacity + 1);
            string planeId = Guid.NewGuid().ToString();
            string URL = String.Format("http://localhost:{0}/PassengerService.svc/GeneratePassengers?id={1}&count={2}", Ports.Passenger, planeId, passengerCount);
            string passengerString = Util.MakeRequest(URL);
            RegistrationList registrationList = JsonConvert.DeserializeObject<RegistrationList>(passengerString);
            int fuelCount = RandomGen.Next(Common.Plane.MIN_GENERATED_FUEL, Common.Plane.MAX_GENERATED_FUEL + 1);

            Common.Plane plane = new Common.Plane(planeId, registrationList.Passengers, registrationList.CargoCount, fuelCount);
            return plane;
        }

        static PlaneHandler()
        {
            Planes = new List<Common.Plane>(4);
            for (int i = 0; i < 4; i++)
            {
                Planes.Add(GeneratePlane());
            }
            PlaneHandlerThread = new Thread(HandlePlanes);
            PlaneHandlerThread.Start();
        }

        public static void HandlePlanes()
        {
            // все изменения списка самолётов проводятся ТОЛЬКО в основном цикле
            // веб-методы только меняют состояния уже существующих объектов
            int activePlanes = 0;
            while (true)
            {
                Thread.Sleep(1000);
                lock (Planes)
                {
                    // в данном цикле обрабатываются только те действия, которые инициируются самим самолётом
                    for (int i = 0; i < Planes.Count; i++)
                    {
                        Common.Plane plane = Planes[i];
                        // если самолёт находится в движении, то пропускаем его - ждём, пока закончит
                        if (plane.State == EntityState.MOVING)
                        {
                            continue;
                        }

                        // дальше действия зависят от текущей локации самолёта
                        // движение не рассматриваем

                        //если самолёт находится в воздухе
                        if (plane.CurrentZone == Zone.PLANE_SPAWN)
                        {
                            // если самолёт закончил движение, то его можно убрать и сгенерить новый
                            if (plane.State == EntityState.FINISHED_TASK)
                            {
                                //TODO: убрать самолёт, сделать новый
                                Planes.Remove(plane);
                                plane = GeneratePlane();
                                Planes.Add(plane);

                            }
                            // если самолёт ждёт разрешения на посадку, то проверяем свободные полосы
                            else if (plane.State == EntityState.WAITING_FOR_COMMAND)
                            {
                                //TODO: проверить наличие свободной полосы. если есть - поставить таймер
                                if (activePlanes < 2)
                                {
                                    activePlanes++;
                                    int landingDelay = RandomGen.Next(30, 35);
                                    plane.ActionTime = DateTime.Now + TimeSpan.FromSeconds(landingDelay);
                                    plane.State = EntityState.STANDING_BY;
                                }
                            }
                            // если самолёт ждёт своей очереди на посадку, то проверяем, пришло ли его время
                            else if (plane.State == EntityState.STANDING_BY)
                            {
                                //TODO: проверка таймера действия, начало посадки

                            }
                        }

                        // если самолёт уже сел
                        if (plane.CurrentZone == Zone.HANGAR_1 || plane.CurrentZone == Zone.HANGAR_2)
                        {
                            // если самолёт закончил движение, то надо сказать службе наземного контроля о начале разгрузки/погрузки
                            if (plane.State == EntityState.FINISHED_TASK)
                            {
                                //TODO: начать погрузку/разгрузку самолёта
                            }
                            // если самолёт ждёт команды, то его уже погрузили, и пора улетать
                            else if (plane.State == EntityState.WAITING_FOR_COMMAND)
                            {
                                //TODO: начать взлёт
                            }
                            // если самолёт ждёт, то его ещё не загрузили 
                            else if (plane.State == EntityState.STANDING_BY)
                            {
                                continue;
                            }
                        }
                        
                        // остальные действия, такие как принятие пассажиров или отлёт, а также регистрация в службе наземного контроля, выполняются в веб-методах
                    }
                }
                Thread.Sleep(200);
            }
        }
    }
}