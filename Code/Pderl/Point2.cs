using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PderlTest
{
    public struct Point2
    {
        public Point2(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// 距离平方
        /// </summary>
        public double Length2
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        /// <summary>
        /// 距离
        /// </summary>
        public double Length => Math.Sqrt(Length2);

        public static Point2 operator -(Point2 x, Point2 y)
        {
            return new Point2(x.X - y.X, x.Y - y.Y);
        }

        public static Point2 operator -(Point3 x, Point2 y)
        {
            return new Point2(x.X - y.X, x.Y - y.Y);
        }

        public static Point2 operator -(Point2 x, Point3 y)
        {
            return new Point2(x.X - y.X, x.Y - y.Y);
        }
    }
}
