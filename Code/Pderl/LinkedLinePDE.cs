using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PderlTest
{
    /// <summary>
    /// 用于同时标记值与相应的位置
    /// </summary>
    class LinkedLinePDE
    {
        public LinkedLinePDE(double endK, double a)
        {
            EndK = endK;
            A = a;
        }
        
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
        /// 本线段连接的下一段
        /// </summary>
        public LinkedLinePDE Next;

        /// <summary>
        /// 本线段连接的上一段
        /// </summary>
        public LinkedLinePDE Pre;

        /// <summary>
        /// 向前连接
        /// </summary>
        /// <param name="line"></param>
        public void LinkForword(LinkedLinePDE line)
        {
            this.Next = line;
            line.Pre = this;
        }
        
        
    }
}
