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
        public Point BottomRight { get; set; }

        public Point GetCoordinates()
        {
            int x = RandomGen.Next(TopLeft.X, BottomRight.X);
            int y = RandomGen.Next(TopLeft.Y, BottomRight.Y);
            return new Point(x, y);
        }

        public ExpandedZone(Zone zone, Point topLeft, Point bottomRight)
        {
            ZoneType = zone;
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }
    }
}