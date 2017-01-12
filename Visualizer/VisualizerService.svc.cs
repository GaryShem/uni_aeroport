using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;
using Newtonsoft.Json;

namespace Visualizer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "VisualizerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select VisualizerService.svc or VisualizerService.svc.cs at the Solution Explorer and start debugging.
    public class VisualizerService : IVisualizerService
    {
        public static Point GetZonePoint(Zone zone)
        {
            return VisualizerHandler.GetZonePoint(zone); //получение рандомной точки в нужной зоне
        }

        public void Spawn(int entityNum, string id, int zoneNum, int cargoCount) //спавним объект (все, кроме самолётов)
        {
            Zone zone = (Zone)zoneNum;
            Entity entity = (Entity)entityNum;
            switch (entity)
            {
                case Entity.BUS:
                case Entity.CARGO_TRUCK:
                case Entity.FUEL_TRUCK:
                {
                    LandVehicle vehicle = new LandVehicle() //новая машинка
                    {
                        Cargo = cargoCount,
                        CurrentZone = zone,
                        Id = id,
                        State = EntityState.WAITING_FOR_COMMAND,
                        VehicleType = entity
                    };
                    Point coords = GetZonePoint(zone); //выбираем рандомную точку в зоне для спавна
                    lock (VisualizerHandler.LandVehicles)
                    {
                        VisualizerHandler.LandVehicles.Add(new Triple<LandVehicle, Point, Point, Zone>(vehicle, coords,
                            coords, zone)); //добавляем в список отрисовки
                    }
                }
                    break;
                case Entity.PASSENGER: //новый пассажир
                {
                    Passenger passenger = new Passenger()
                    {
                        CargoCount = cargoCount,
                        CurrentZone = zone,
                        Id = id,
                        State = EntityState.WAITING_FOR_COMMAND,

                    };
                    Point coords = GetZonePoint(zone);
                    lock (VisualizerHandler.Passengers)
                    {
                        VisualizerHandler.Passengers.Add(new Triple<Passenger, Point, Point, Zone>(passenger, coords,
                            coords, zone)); //добавляем в список отрисовки
                        }
                }
                    break;
            }
        }

        public void SpawnPlane(int entityNum, string id, int zoneNum, int cargoCount, int passengerCount, int fuelCount) //спавним самолёт
        {
            Zone zone = (Zone) zoneNum;
            Entity entity = (Entity) entityNum;
            List<string> fakePassengers = new List<string>(); // лист с фэйковыми пассажирами для вывода информации
            for (int i = 0; i < passengerCount; i++)
            {
                fakePassengers.Add(null);
            }
            Plane plane = new Plane(id, fakePassengers, cargoCount, fuelCount); // новый самолёт
            Point coords = GetZonePoint(zone);
            lock (VisualizerHandler.Planes)
            {
                VisualizerHandler.Planes.Add(new Triple<Plane, Point, Point, Zone>(plane, coords, coords, zone)); //добавляем в спсок отрисовки
            }
        }

        public void Despawn(string id) //удаляем объект
        {
            lock (VisualizerHandler.LandVehicles)
            {
                VisualizerHandler.LandVehicles.RemoveAll(x => x.Item1.Id.Equals(id));
            }
            lock (VisualizerHandler.Planes)
            {
                VisualizerHandler.Planes.RemoveAll(x => x.Item1.Id.Equals(id));
            }
            lock (VisualizerHandler.Passengers)
            {
                VisualizerHandler.Passengers.RemoveAll(x => x.Item1.Id.Equals(id));
            }
        }

        public void Move(int entityNum, string id, int zoneNum) //вызываем когда надо запустить движение объекта
        {
            Zone zone = (Zone)zoneNum;
            Entity entity = (Entity)entityNum;
            switch (entity)
            {
                case Entity.PLANE:
                    lock (VisualizerHandler.Planes)
                    {
                        var planeTuple = VisualizerHandler.Planes.Find(x => x.Item1.Id.Equals(id));
//                        if (planeTuple == null) return;
                        Plane plane = planeTuple.Item1;
                        planeTuple.Item4 = zone;
                        Point newCoords = GetZonePoint(zone);
                        planeTuple.Item3 = newCoords;
                        plane.State = EntityState.MOVING;
                    }
                    break;
                case Entity.PASSENGER:
                    lock (VisualizerHandler.Passengers)
                    {
                        var passengerTuple = VisualizerHandler.Passengers.Find(x => x.Item1.Id.Equals(id));
//                        if (passengerTuple == null) return;
                        Passenger passenger = passengerTuple.Item1;
                        passengerTuple.Item4 = zone;
                        Point newCoords = GetZonePoint(zone);
                        passengerTuple.Item3 = newCoords;
                        passenger.State = EntityState.MOVING;
                    }
                    break;
                case Entity.FUEL_TRUCK:
                case Entity.CARGO_TRUCK:
                case Entity.BUS:
                    lock (VisualizerHandler.LandVehicles)
                    {
                        var vehicleTuple = VisualizerHandler.LandVehicles.Find(x => x.Item1.Id.Equals(id));

                        LandVehicle vehicle = vehicleTuple.Item1;
                        vehicleTuple.Item4 = zone;
                        Point newCoords = GetZonePoint(zone);
                        vehicleTuple.Item3 = newCoords;
                        vehicle.State = EntityState.MOVING;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity), entity, null);
            }
        }

        public string GetAllVehicles() //получение списка всех машинок
        {
            lock (VisualizerHandler.LandVehicles)
            {
                List<Tuple<int, Point, int>> vehicleTuples =
                    VisualizerHandler.LandVehicles.Select(x => new Tuple<int, Point, int>((int)x.Item1.VehicleType, x.Item2, x.Item1.Cargo))
                        .ToList();
                return JsonConvert.SerializeObject(vehicleTuples);
            }
        }

        public string GetAllPlanes() //получение списка всех самолётов
        {
            lock (VisualizerHandler.Planes)
            {
                List<Tuple<int, Point, int, int, int>> planeTuples =
                    VisualizerHandler.Planes.Select(x => new Tuple<int, Point, int, int, int>((int)Entity.PLANE, x.Item2, x.Item1.Passengers.Count, x.Item1.CargoCount, x.Item1.FuelCount))
                        .ToList();
                return JsonConvert.SerializeObject(planeTuples);
            }
        }

        public string GetAllPassengers() //получение списка всех людей
        {
            lock (VisualizerHandler.Passengers)
            {
                List<Tuple<int, Point>> passengerTuples =
                    VisualizerHandler.Passengers.Select(x => new Tuple<int, Point>((int)Entity.PASSENGER, x.Item2))
                        .ToList();
                return JsonConvert.SerializeObject(passengerTuples);
            }
        }

        public void UpdateCargo(string id, int cargoCount) //обновление значений полей с гурзом/топливом у машинок
        {
            lock (VisualizerHandler.Planes)
            {
                Plane plane = VisualizerHandler.Planes.Select(x => x.Item1).ToList().Find(x => x.Id.Equals(id));
                if (plane != null)
                {
                    plane.CargoCount = cargoCount;
                }
            }
            lock (VisualizerHandler.LandVehicles)
            {
                LandVehicle vehicle = VisualizerHandler.LandVehicles.Select(x => x.Item1).ToList().Find(x => x.Id.Equals(id));
                if (vehicle != null)
                {
                    vehicle.Cargo = cargoCount;
                }
            }
        }

        public void UpdatePlane(string id, int passengerCount, int cargoCount, int fuelCount) //обновление полей у самолётов
        {
            lock (VisualizerHandler.Planes)
            {
                Plane plane = VisualizerHandler.Planes.Select(x => x.Item1).ToList().Find(x => x.Id.Equals(id));
                if (plane != null)
                {
                    int passengerDifference = plane.PassengerCount - passengerCount;
                    while (passengerDifference > 0) //если в самолёте стало меньше пассажиров
                    {
                        plane.Passengers.RemoveAt(0);
                        passengerDifference--;
                    }
                    while (passengerDifference < 0) //если в самолёте стало больше пассажиров
                    {
                        plane.Passengers.Add("");
                        passengerDifference++;
                    }
                    plane.FuelCount = fuelCount;
                    plane.CargoCount = cargoCount;
                }
            }
        }
    }
}
