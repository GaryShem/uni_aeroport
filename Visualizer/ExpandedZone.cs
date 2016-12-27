using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Common;

namespace Visualizer
{
    public class ExpandedZone
    {
        public Zone ZoneType { get; set; }
        public Point TopLeft { get; set; }
        public Point BorromRight { get; set; }

        public Point GetCoordinates()
        {
            int x = RandomGen.Next(TopLeft.X, BorromRight.X);
            int y = RandomGen.Next(TopLeft.Y, BorromRight.Y);
            return new Point(x, y);
        }
    }
}