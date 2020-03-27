using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PderlTest
{
    public class Line
    {
        public Line()
        {
            Points = new List<LinePoint>();
        }

        public List<LinePoint> Points;
    }

    public struct LinePoint
    {
        public LinePoint(double lon = 0, double lat = 0)
        {
            Lon = lon;
            Lat = lat;
        }

        public double Lon { get; set; }

        public double Lat { get; set; }
    }


}
