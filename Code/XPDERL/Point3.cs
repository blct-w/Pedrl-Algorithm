using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDERLTest
{
    public struct Point3
    {
        public Point3(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static Point3 operator -(Point3 x, Point3 y)
        {
            return new Point3(x.X - y.X, x.Y - y.Y, x.Z - y.Z);
        }
    }
}
