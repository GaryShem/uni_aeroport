using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;

namespace Visualizer
{
    public class LandVehicle
    {
        public Entity VehicleType { get; set; }
        public ExpandedZone CurrentZone { get; set; }
        public ExpandedZone TargetZone { get; set; }
        public EntityState State { get; set; }
        public int Cargo { get; set; }
    }
}