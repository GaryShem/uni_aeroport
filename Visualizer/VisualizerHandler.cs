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

        public static List<Tuple<LandVehicle, Point, Point>> LandVehicles;
        public static List<Tuple<Passenger, Point, Point>> Passengers;
        public static List<Tuple<Plane, Point, Point>> Planes;

        private static Thread VisThread;

        static VisualizerHandler()
        {
            Zones.Add(new ExpandedZone(Zone.PASSENGER_SPAWN, new Point(0,0), new Point(20,20)));
            Zones.Add(new ExpandedZone(Zone.REGISTRATION_STAND, new Point(0, 50), new Point(50,100)));
            Zones.Add(new ExpandedZone(Zone.WAITING_AREA, new Point(0, 110), new Point(50, 160)));
            Zones.Add(new ExpandedZone(Zone.BUS_STATION, new Point(25, 185), new Point(50, 210)));
            Zones.Add(new ExpandedZone(Zone.FUEL_STATION, new Point(25, 215), new Point(50, 240)));
            Zones.Add(new ExpandedZone(Zone.CARGO_AREA, new Point(52, 40), new Point(52, 50)));
            Zones.Add(new ExpandedZone(Zone.CARGO_DROPOFF, new Point(40,75), new Point(50,75)));
            Zones.Add(new ExpandedZone(Zone.HANGAR_1, new Point(90,100), new Point(100,110)));
            Zones.Add(new ExpandedZone(Zone.HANGAR_2, new Point(90,150), new Point(100, 160)));
            Zones.Add(new ExpandedZone(Zone.PLANE_SPAWN, new Point(140, 120), new Point(150,130)));

            LandVehicles = new List<Tuple<LandVehicle, Point, Point>>();
            Passengers = new List<Tuple<Passenger, Point, Point>>();
            Planes = new List<Tuple<Plane, Point, Point>>();

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
                    foreach (Tuple<LandVehicle, Point, Point> landVehicleTuple in LandVehicles)
                    {
                        
                    }
                }

                lock (Passengers)
                {
                    foreach (Tuple<Passenger, Point, Point> passengerTuple in Passengers)
                    {
                        
                    }
                }

                lock (Planes)
                {
                    foreach (Tuple<Plane, Point, Point> planeTuple in Planes)
                    {
                        
                    }
                    {
                        
                    }
                }
            }
        }
    }
}