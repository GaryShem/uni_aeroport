﻿using System;
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

            VisThread = new Thread(HandleVisuals);
            VisThread.Start();
        }
        public static void HandleVisuals()
        {
            while (true)
            {
                
            }
        }
    }
}