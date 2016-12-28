using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web;
using Common;

namespace Visualizer
{
    public static class VisualizerHandler
    {
        public static List<ExpandedZone> Zones = new List<ExpandedZone>();

        public static Point GetZonePoint(Zone zone)
        {
            return Zones.Find(x => x.ZoneType == zone).GetCoordinates();
        }

        public static List<Triple<LandVehicle, Point, Point, Zone>> LandVehicles;
        public static List<Triple<Passenger, Point, Point, Zone>> Passengers;
        public static List<Triple<Plane, Point, Point, Zone>> Planes;

        private static Thread VisThread;

        static VisualizerHandler()
        {
            Zones.Add(new ExpandedZone(Zone.PASSENGER_SPAWN, new Point(-20,175), new Point(-20+1,175+134)));
            Zones.Add(new ExpandedZone(Zone.REGISTRATION_STAND, new Point(60, 127), new Point(60+204,127+193)));
            Zones.Add(new ExpandedZone(Zone.WAITING_AREA, new Point(45,419), new Point(56+187,419+219)));
            Zones.Add(new ExpandedZone(Zone.BUS_STATION, new Point(287,486), new Point(287+56,486+78)));
            Zones.Add(new ExpandedZone(Zone.FUEL_STATION, new Point(637,533), new Point(637+148,533+134)));
            Zones.Add(new ExpandedZone(Zone.CARGO_AREA, new Point(306,317), new Point(306+36,317+42)));
            Zones.Add(new ExpandedZone(Zone.CARGO_DROPOFF, new Point(192,320), new Point(192+39,210+26)));
            Zones.Add(new ExpandedZone(Zone.HANGAR_1, new Point(557,120), new Point(557+21,120+24)));
            Zones.Add(new ExpandedZone(Zone.HANGAR_2, new Point(788,268), new Point(788+20,268+22)));
            Zones.Add(new ExpandedZone(Zone.PLANE_SPAWN, new Point(2004,182), new Point(2004+61,182+55)));

            LandVehicles = new List<Triple<LandVehicle, Point, Point, Zone>>();
            Passengers = new List<Triple<Passenger, Point, Point, Zone>>();
            Planes = new List<Triple<Plane, Point, Point, Zone>>();

            VisThread = new Thread(HandleVisuals);
            VisThread.Start();
        }
        public static void HandleVisuals()
        {
            while (true)
            {
                Thread.Sleep(10);
                lock (LandVehicles)
                {
                    foreach (Triple<LandVehicle, Point, Point, Zone> landVehicleTriple in LandVehicles)
                    {
                        LandVehicle vehicle = landVehicleTriple.Item1;
                        if (vehicle.State != EntityState.MOVING)
                            continue;
                        Point currentPoint = landVehicleTriple.Item2;
                        Point targetPoint = landVehicleTriple.Item3;
                        int diffX = targetPoint.X - currentPoint.X;
                        diffX = Math.Min(5, Math.Abs(targetPoint.X - currentPoint.X))*Math.Sign(diffX);
                        int diffY = targetPoint.Y - currentPoint.Y;
                        diffY = Math.Min(5, Math.Abs(targetPoint.Y - currentPoint.Y)) * Math.Sign(diffY);
                        currentPoint.X += diffX;
                        currentPoint.Y += diffY;
                        landVehicleTriple.Item2 = currentPoint;
                        if (currentPoint.X == targetPoint.X && currentPoint.Y == targetPoint.Y)
                        {
                            CompleteMove(vehicle.VehicleType, vehicle.Id, landVehicleTriple.Item4);
                            vehicle.CurrentZone = landVehicleTriple.Item4;
                            vehicle.State = EntityState.WAITING_FOR_COMMAND;
                        }
                    }
                }

                lock (Passengers)
                {
                    foreach (Triple<Passenger, Point, Point, Zone> passengerTriple in Passengers)
                    {
                        Passenger passenger = passengerTriple.Item1;
                        if (passenger.State != EntityState.MOVING)
                            continue;
                        Point currentPoint = passengerTriple.Item2;
                        Point targetPoint = passengerTriple.Item3;
                        int diffX = targetPoint.X - currentPoint.X;
                        diffX = Math.Sign(diffX);
                        int diffY = targetPoint.Y - currentPoint.Y;
                        diffY = Math.Sign(diffY);
                        passengerTriple.Item2 = new Point(currentPoint.X + diffX, currentPoint.Y + diffY);
                        if (passengerTriple.Item2.X == targetPoint.X && passengerTriple.Item2.Y == targetPoint.Y)
                        {
                            CompleteMove(Entity.PASSENGER, passenger.Id, passengerTriple.Item4);
                            passenger.CurrentZone = passengerTriple.Item4;
                            passenger.State = EntityState.WAITING_FOR_COMMAND;
                        }
                    }
                }

                lock (Planes)
                {
                    foreach (Triple<Plane, Point, Point, Zone> planeTriple in Planes)
                    {
                        int speed = 5;
                        Plane plane = planeTriple.Item1;
                        if (plane.State != EntityState.MOVING)
                            continue;
                        Point currentPoint = planeTriple.Item2;
                        Point targetPoint = planeTriple.Item3;
                        int diffX = targetPoint.X - currentPoint.X;
                        int diffY = targetPoint.Y - currentPoint.Y;
                        double length = Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2));
                        if (length > speed)
                        {
                            diffX = (int) (diffX/length*speed);
                            diffY = (int) (diffY/length*speed);
                        }
                        planeTriple.Item2 = new Point(currentPoint.X + diffX, currentPoint.Y + diffY);
                        if (planeTriple.Item2.X == targetPoint.X && planeTriple.Item2.Y == targetPoint.Y)
                        {
                            CompleteMove(Entity.PLANE, plane.Id, planeTriple.Item4);
                            plane.CurrentZone = planeTriple.Item4;
                            plane.State = EntityState.WAITING_FOR_COMMAND;
                        }
                    }
                }
            }
        }

        public static async void CompleteMove(Entity entity, string id, Zone zone)
        {
            string URL = "http://localhost:";
            switch (entity)
            {
                case Entity.PLANE:
                    URL += String.Format("{0}/PlaneService.svc", Ports.Plane);
                    break;
                case Entity.PASSENGER:
                    URL += String.Format("{0}/PassengerService.svc", Ports.Passenger);
                    break;
                case Entity.FUEL_TRUCK:
                    URL += String.Format("{0}/FuelTruckService.svc", Ports.FuelTruck);
                    break;
                case Entity.CARGO_TRUCK:
                    URL += String.Format("{0}/CargoTruckService.svc", Ports.CargoTruck);
                    break;
                case Entity.BUS:
                    URL += String.Format("{0}/BusService.svc", Ports.Bus);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity), entity, null);
            }
            URL += String.Format("/CompleteMove?id={0}&zone={1}", id, (int)zone);
            await Util.MakeRequestAsync(URL);
        }
    }
}