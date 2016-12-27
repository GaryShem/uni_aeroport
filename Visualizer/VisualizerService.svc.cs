using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;

namespace Visualizer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "VisualizerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select VisualizerService.svc or VisualizerService.svc.cs at the Solution Explorer and start debugging.
    public class VisualizerService : IVisualizerService
    {
        public void Spawn(Entity entity, string id, Zone zone, int cargoCount)
        {
            switch (entity)
            {
                case Entity.BUS:
                case Entity.CARGO_TRUCK:
                case Entity.FUEL_TRUCK:
                {
                    LandVehicle vehicle = new LandVehicle()
                    {
                        Cargo = cargoCount,
                        CurrentZone = zone,
                        Id = id,
                        State = EntityState.WAITING_FOR_COMMAND,
                        VehicleType = entity
                    };
                        Point coords = VisualizerHandler.Zones.Find(x => x.ZoneType == zone).GetCoordinates();
                    lock (VisualizerHandler.LandVehicles)
                    {
                        VisualizerHandler.LandVehicles.Add(new Tuple<LandVehicle, Point, Point>(vehicle, coords, coords));
                    }
                }
                    break;
                case Entity.PASSENGER:
                {
                    Passenger passenger = new Passenger()
                    {
                        CargoCount = cargoCount,
                        CurrentZone = zone,
                        Id = id,
                        State = EntityState.WAITING_FOR_COMMAND,

                    };
                    Point coords = VisualizerHandler.Zones.Find(x => x.ZoneType == zone).GetCoordinates();
                    lock (VisualizerHandler.Passengers)
                    {
                        VisualizerHandler.Passengers.Add(new Tuple<Passenger, Point, Point>(passenger, coords, coords));
                    }
                }
                    break;
            }
        }

        public void SpawnPlane(Entity entity, string id, Zone zone, int cargoCount, int passengerCount, int fuelCount)
        {
            List<string> fakePassengers = new List<string>();
            for (int i = 0; i < passengerCount; i++)
            {
                fakePassengers.Add(null);
            }
            Plane plane = new Plane(id, fakePassengers, cargoCount, fuelCount);
            Point coords = VisualizerHandler.Zones.Find(x => x.ZoneType == zone).GetCoordinates();
            lock (VisualizerHandler.Planes)
            {
                VisualizerHandler.Planes.Add(new Tuple<Plane, Point, Point>(plane, coords, coords));
            }
        }

        public void Despawn(string id, Zone zone)
        {
            lock (VisualizerHandler.LandVehicles)
            {
                VisualizerHandler.LandVehicles.RemoveAll(x => x.Item1.Id.Equals(id));
            }
            lock (VisualizerHandler.Passengers)
            {
                VisualizerHandler.Passengers.RemoveAll(x => x.Item1.Id.Equals(id));
            }
            lock (VisualizerHandler.Planes)
            {
                VisualizerHandler.Planes.RemoveAll(x => x.Item1.Id.Equals(id));
            }
        }

        public void Move(Entity entity, string id, Zone zone)
        {
            switch (entity)
            {
                case Entity.PLANE:
                    lock (VisualizerHandler.Planes)
                    {
                        var planeTuple = VisualizerHandler.Planes.Find(x => x.Item1.Id.Equals(id));
                        Plane plane = planeTuple.Item1;
                        Point newCoords = VisualizerHandler.Zones.Find(x => x.ZoneType == zone).GetCoordinates();
                        Point oldTargetCoorts = planeTuple.Item3;
                        oldTargetCoorts.X = newCoords.X;
                        oldTargetCoorts.Y = newCoords.Y;
                        plane.State = EntityState.MOVING;
                    }
                    break;
                case Entity.PASSENGER:
                    lock (VisualizerHandler.Passengers)
                    {
                        var passengerTuple = VisualizerHandler.Passengers.Find(x => x.Item1.Id.Equals(id));
                        Passenger passenger = passengerTuple.Item1;
                        Point newCoords = VisualizerHandler.Zones.Find(x => x.ZoneType == zone).GetCoordinates();
                        Point oldTargetCoorts = passengerTuple.Item3;
                        oldTargetCoorts.X = newCoords.X;
                        oldTargetCoorts.Y = newCoords.Y;
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
                        Point newCoords = VisualizerHandler.Zones.Find(x => x.ZoneType == zone).GetCoordinates();
                        Point oldTargetCoorts = vehicleTuple.Item3;
                        oldTargetCoorts.X = newCoords.X;
                        oldTargetCoorts.Y = newCoords.Y;
                        vehicle.State = EntityState.MOVING;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity), entity, null);
            }
        }
    }
}
