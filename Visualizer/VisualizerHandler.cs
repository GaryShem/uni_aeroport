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
            Zones.Add(new ExpandedZone(Zone.REGISTRATION_STAND, new Point(60, 127), new Point(60+204,127+190)));
            Zones.Add(new ExpandedZone(Zone.WAITING_AREA, new Point(60,419), new Point(60+187,419+219)));
            Zones.Add(new ExpandedZone(Zone.BUS_STATION, new Point(314,506), new Point(314+3,506+39)));
            Zones.Add(new ExpandedZone(Zone.FUEL_STATION, new Point(757,638), new Point(757+11,638+20)));
            Zones.Add(new ExpandedZone(Zone.CARGO_AREA, new Point(305,340), new Point(305+10,340+19)));
            Zones.Add(new ExpandedZone(Zone.CARGO_DROPOFF, new Point(192,320), new Point(192+39,320+26)));
            Zones.Add(new ExpandedZone(Zone.HANGAR_1, new Point(557,120), new Point(557+21,120+24)));
            Zones.Add(new ExpandedZone(Zone.HANGAR_2, new Point(714,266), new Point(714+20,266+22)));
            Zones.Add(new ExpandedZone(Zone.PLANE_SPAWN_1, new Point(2004,120), new Point(2004+61,120+24)));
            Zones.Add(new ExpandedZone(Zone.PLANE_SPAWN_2, new Point(2004, 266), new Point(2004 + 61, 266+22)));

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
                        int speed = 3;
                        LandVehicle vehicle = landVehicleTriple.Item1;
                        if (vehicle.State != EntityState.MOVING)
                            continue;
                        Point currentPoint = landVehicleTriple.Item2;
                        Point targetPoint = landVehicleTriple.Item3;
                        int diffX = targetPoint.X - currentPoint.X;
                        int diffY = targetPoint.Y - currentPoint.Y;
                        double length = Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2));
                        if (length > speed)
                        {
                            diffX = (int)(diffX / length * speed);
                            diffY = (int)(diffY / length * speed);
                        }
                        landVehicleTriple.Item2 = new Point(currentPoint.X + diffX, currentPoint.Y + diffY);
                        if (landVehicleTriple.Item2.X == targetPoint.X && landVehicleTriple.Item2.Y == targetPoint.Y)
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
//                        if (passenger.State != EntityState.MOVING)
//                        {
//                            int chance = RandomGen.Next(10);
//                            if (chance < 1)
//                            {
//                                
//                            }
//                        }
                        Point currentPoint = passengerTriple.Item2;
                        Point targetPoint = passengerTriple.Item3;
                        int diffX = Math.Sign(targetPoint.X - currentPoint.X);
                        int diffY = Math.Sign(targetPoint.Y - currentPoint.Y);
                        if (diffX == 0 && diffY == 0 && passenger.State == EntityState.WAITING_FOR_COMMAND)
                        {
                            int chance = RandomGen.Next(500);
                            if (chance < 1)
                            {
                                passengerTriple.Item3 = GetZonePoint(passengerTriple.Item4);
                            }
                        }
                        passengerTriple.Item2 = new Point(currentPoint.X + diffX, currentPoint.Y + diffY);
                        if (passengerTriple.Item2.X == targetPoint.X && passengerTriple.Item2.Y == targetPoint.Y)
                        {
                            if (passengerTriple.Item1.State == EntityState.MOVING)
                            {
                                CompleteMove(Entity.PASSENGER, passenger.Id, passengerTriple.Item4);
                            }
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
            string URL;
            switch (entity)
            {
                case Entity.PLANE:
                    URL = ServiceStrings.Plane;
                    break;
                case Entity.PASSENGER:
                    URL = ServiceStrings.Passenger;
                    break;
                case Entity.FUEL_TRUCK:
                    URL = ServiceStrings.Fuel;
                    break;
                case Entity.CARGO_TRUCK:
                    URL = ServiceStrings.Cargo;
                    break;
                case Entity.BUS:
                    URL = ServiceStrings.Bus;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity), entity, null);
            }
            URL += String.Format("/CompleteMove?id={0}&zone={1}", id, (int)zone);
            await Util.MakeRequestAsync(URL);
        }
    }
}