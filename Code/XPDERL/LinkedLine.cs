using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDERLTest
{
    /// <summary>
    /// 用于同时标记值与相应的位置
    /// </summary>
    class LinkedLine
    {
        public LinkedLine(double endK, double a, double g)
        {
            EndK = endK;
            A = a;
            G = g;
        }

        /// <summary>
        /// Double的无效值
        /// </summary>
        public static double InvaluedValue = 1e-15;

        /// <summary>
        /// 该曲线的末点K
        /// </summary>
        public double EndK;

        /// <summary>
        /// 该曲线的起点K
        /// </summary>
        public double StartK
        {
            get
            {
                return this.Pre.EndK;
            }
        }

        /// <summary>
        /// 参考线参数a
        /// </summary>
        public double A;

        /// <summary>
        /// 参考线参数b(用的时候再求)
        /// </summary>
        public double B
        {
            get
            {
                return G - A * EndK;
            }
        }

        /// <summary>
        /// 结束点的HXV
        /// </summary>
        public double G;

        /// <summary>
        /// 本线段连接的下一段
        /// </summary>
        public LinkedLine Next;

        /// <summary>
        /// 本线段连接的上一段
        /// </summary>
        public LinkedLine Pre;

        /// <summary>
        /// 向前连接
        /// </summary>
        /// <param name="line"></param>
        public void LinkForword(LinkedLine line)
        {
            this.Next = line;
            line.Pre = this;
        }

        /// <summary>
        /// 插入点
        /// </summary>
        /// <param name="line"></param>
        public void InsertForword(LinkedLine line)
        {
            if (this.Next != null)
            {
                this.Next.Pre = line;
                line.Next = this.Next;
            }
            this.Next = line;
            line.Pre = this;
        }

        /// <summary>
        /// 获取关键点
        /// </summary>
        /// <returns></returns>
        public string getOSKeyPoints()
        {
            string os = "";
            var cu = this;
            do
            {
                os += cu.EndK + "," + cu.G;
                os += "\r\n";
                cu = cu.Next;
            }
            while (cu != null);
            return os;
        }
        

        /// <summary>
        /// 查找到指定段线之前有多少节
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int CountToLine(LinkedLine line)
        {
            int i = 1;
            var cu = this;
            while (cu.Next != null && cu.Next != line)
            {
                i++;
                cu = cu.Next;
            }
            i++;
            return i;
        }
        

        /// <summary>
        /// 从上一次参与计算的参考线段到现在为止查找交点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="minK">上一格网点在哪个参考线上求的</param>
        /// <param name="crossK"></param>
        /// <param name="crossLine">与具体哪根参考线有交点</param>
        /// <returns></returns>
        public bool TryGetCross(double a, double b, double minK, double maxK, out double crossK, out LinkedLine crossLine)
        {
            //标记在哪个线段上求交的
            crossLine = this;
            crossK = (b - this.B) / (this.A - a);
            if (crossK >= StartK && crossK < maxK)
                return true;
            if (Math.Abs(crossK - StartK) < 1e-15 || Math.Abs(crossK - maxK) < InvaluedValue)
                return true;

            while (crossLine.Pre != null && crossLine.Pre.EndK >= minK)
            {
                crossLine = crossLine.Pre;

                crossK = (b - crossLine.B) / (crossLine.A - a);
                if (crossK >= crossLine.StartK && crossK <= crossLine.EndK)
                    return true;

                if (Math.Abs(crossK - crossLine.StartK) < 1e-15 || Math.Abs(crossK - crossLine.EndK) < InvaluedValue)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 求指定方向的斜率
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double GetG(double k)
        {
            return G;
        }

        public string LineKStr
        {
            get
            {
                string str = "";
                var ne = this;
                while (ne != null)
                {
                    str += ne.EndK.ToString("0.000") + ",";
                    ne = ne.Next;
                }
                return str;
            }
        }
    }
}
