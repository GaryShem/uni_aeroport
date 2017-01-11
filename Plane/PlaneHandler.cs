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
        private static bool h1free;
        private static bool h2free;
//        private static int iter = 0;

        public static Common.Plane GeneratePlane(int passengerCount)
        {
            string planeId = Guid.NewGuid().ToString();
            //OpenRegistration?flightId={flightId}
            string URL = String.Format("{0}/OpenRegistration?flightId={1}", ServiceStrings.RegStand, planeId);
            Util.MakeRequest(URL);
            URL = String.Format("{0}/GeneratePassengers?id={1}&count={2}", ServiceStrings.Passenger, planeId, passengerCount);
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
                int passengerCount = RandomGen.Next(1, Common.Plane.PassengerCapacity + 1);
                Planes.Add(GeneratePlane(passengerCount));
            }
            h1free = true;
            h2free = true;
            PlaneHandlerThread = new Thread(HandlePlanes);
            PlaneHandlerThread.Start();
        }

        public static void FreeHangar(Zone zone)
        {
            switch (zone)
            {
                case Zone.HANGAR_1:
                    h1free = true;
                    break;
                case Zone.HANGAR_2:
                    h2free = true;
                    break;
                default:
                    return;
            }
        }

        public static void OccupyHangar(Zone zone)
        {
            switch (zone)
            {
                case Zone.HANGAR_1:
                    h1free = false;
                    break;
                case Zone.HANGAR_2:
                    h2free = false;
                    break;
                default:
                    return;
            }
        }

        private static void StartPlaneMaintenance(Common.Plane plane)
        {
            string URL = String.Format("{0}/StartService?id={1}&zone={2}",
                ServiceStrings.GrControl, plane.Id, (int)plane.CurrentZone);
            Util.MakeRequest(URL);
            plane.State = EntityState.STANDING_BY;
        }

        private static void MovePlane(Common.Plane plane, Zone zone)
        {
            string URL = String.Format("{0}/Move?type={1}&id={2}&zone={3}",
                                    ServiceStrings.Vis, (int)Entity.PLANE, plane.Id, (int)zone);
            Util.MakeRequest(URL);
            plane.State = EntityState.MOVING;
        }

        private static void SpawnPlane(Common.Plane plane)
        {
            string URL = String.Format("{0}/SpawnPlane?type={1}&id={2}&zone={3}&cargo={4}&passengerCount={5}&fuelCount={6}",
                         ServiceStrings.Vis, (int)Entity.PLANE, plane.Id, (int)plane.CurrentZone, plane.CargoCount, plane.PassengerCount, plane.FuelCount);
            Util.MakeRequest(URL);
        }

        private static void RemovePlane(Common.Plane plane)
        {
            string URL = String.Format("{0}/Despawn?id={1}",
                ServiceStrings.Vis, plane.Id);
            Util.MakeRequest(URL);
            Planes.Remove(plane);
            if (Planes.Count < 4)
            {
                int passengerCount = RandomGen.Next(1, Common.Plane.PassengerCapacity + 1);
                plane = GeneratePlane(passengerCount);
                Planes.Add(plane);
            }
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
//                    Util.Log(@"F:\Programming\CPP\Winsock\Aeroport\Plane\log"+ (iter++) +".txt", JsonConvert.SerializeObject(Planes));
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
                        if (plane.CurrentZone == Zone.PLANE_SPAWN_1 || plane.CurrentZone == Zone.PLANE_SPAWN_2)
                        {
                            // если самолёт закончил движение, то его можно убрать и сгенерить новый
                            if (plane.State == EntityState.FINISHED_TASK)
                            {
                                RemovePlane(plane);
                                activePlanes--;
                            }
                            // если самолёт ждёт разрешения на посадку, то проверяем свободные полосы
                            else if (plane.State == EntityState.WAITING_FOR_COMMAND)
                            {
                                if (activePlanes < 2)
                                {
                                    activePlanes++;
                                    int landingDelay = RandomGen.Next(1, 5);
                                    plane.ActionTime = DateTime.Now + TimeSpan.FromSeconds(landingDelay);
                                    plane.HasAction = true;
                                    plane.State = EntityState.STANDING_BY;
                                }
                            }
                            // если самолёт ждёт своей очереди на посадку, то проверяем, пришло ли его время
                            else if (plane.State == EntityState.STANDING_BY)
                            {
                                // если время пришло, то спавним самолёт и отправляем на посадку
                                if (plane.HasAction && plane.ActionTime < DateTime.Now)
                                {
                                    Zone zone = Zone.FUEL_STATION;
                                    bool isLanding = false;
                                    
                                    if (h1free)
                                    {
                                        zone = Zone.HANGAR_1;
                                        plane.CurrentZone = Zone.PLANE_SPAWN_1;
                                        isLanding = true;
                                        h1free = false;
                                    }
                                    else if (h2free)
                                    {
                                        zone = Zone.HANGAR_2;
                                        plane.CurrentZone = Zone.PLANE_SPAWN_2;
                                        isLanding = true;
                                        h2free = false;

                                    }
                                    if (isLanding)
                                    {
                                        
                                        SpawnPlane(plane);
                                        MovePlane(plane, zone);
                                    }
                                }
                            }
                        }

                        // если самолёт уже сел
                        else if (plane.CurrentZone == Zone.HANGAR_1 || plane.CurrentZone == Zone.HANGAR_2)
                        {
                            Zone despawnPoint = plane.CurrentZone == Zone.HANGAR_1
                                ? Zone.PLANE_SPAWN_1
                                : Zone.PLANE_SPAWN_2;
                            // если самолёт закончил движение, то надо сказать службе наземного контроля о начале разгрузки/погрузки
                            if (plane.State == EntityState.FINISHED_TASK)
                            {
                                //TODO: начать погрузку/разгрузку самолёта
                                StartPlaneMaintenance(plane);
                            }
                            // если самолёт ждёт команды, то его уже погрузили, и пора улетать
                            else if (plane.State == EntityState.WAITING_FOR_COMMAND)
                            {
                                MovePlane(plane, despawnPoint);
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
            }
        }
    }
}