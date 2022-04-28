using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDERLTest
{
    /// <summary>
    /// 算法实施类
    /// </summary>
    public class DemAnalysisHandle
    {
        private DEM dem;
        public DEM Dem { get => dem; set => dem = value; }

        /// <summary>
        /// Double无效值位数
        /// </summary>
        public static readonly double doubleErr = 1e-15;

        public DemAnalysisHandle(DEM _data)
        {
            this.dem = _data;
        }

        /// <summary>
        /// 以经纬度为值进行调用的分析方法
        /// </summary>
        /// <param name="centerLon">观测点中心的经度</param>
        /// <param name="centerLat">观测点中心的纬度</param>
        /// <param name="toEndPointLon">观测区域边缘的经度</param>
        /// <param name="toEndPointLat">观测区域边缘的纬度</param>
        /// <param name="standH">观测点相对地面高程</param>
        /// <param name="result">通视矩阵</param>
        /// <param name="demMinLon">通视矩阵最小经度</param>
        /// <param name="demMinLat">通视矩阵最小纬度</param>
        /// <param name="perLon">经差</param>
        /// <param name="perLat">纬差</param>
        public delegate void DoAnalysisLonLat(double centerLon, double centerLat,
            double toEndPointLon, double toEndPointLat, double standH,
            out int[,] result, out double demMinLon, out double demMinLat,
            out double perLon, out double perLat);

        /// <summary>
        /// 在DEM定义的投影中进行计算的方法
        /// </summary>
        /// <param name="centerX">观测点中心的X坐标(UTM的X/经纬度的lon)</param>
        /// <param name="centerY">观测点中心的Y坐标(UTM的Y/经纬度的lat)</param>
        /// <param name="centerH">观测点的地面海拔高程</param>
        /// <param name="toEndPointX">观测区域边缘的X坐标</param>
        /// <param name="toEndPointY">观测区域边缘的Y坐标</param>
        /// <param name="standH">观测点相对地面高度</param>
        /// <param name="result">结果矩阵</param>
        /// <param name="demMinX">结果矩阵的X小值</param>
        /// <param name="demMinY">结果矩阵的Y小值</param>
        /// <param name="perX">结果矩阵横间距</param>
        /// <param name="perY">结果矩阵纵间距</param>
        public delegate void DoAnalysisOnDEM(double centerX, double centerY,
            double centerH, double toEndPointX, double toEndPointY, double seeHeight,
            out int[,] result, out double demMinX, out double demMinY, out double perX, out double perY);

        #region PDERL算法（与R3算法结果相同，计算量与参考面算法同阶）

        /// <summary>
        /// 进行PDERL通视分析（经纬度）
        /// </summary>
        /// <param name="centerLon">观测点中心的经度</param>
        /// <param name="centerLat">观测点中心的纬度</param>
        /// <param name="toEndPointLon">观测区域边缘的经度</param>
        /// <param name="toEndPointLat">观测区域边缘的纬度</param>
        /// <param name="standH">观测点相对地面高程</param>
        /// <param name="result">通视矩阵</param>
        /// <param name="demMinLon">通视矩阵最小经度</param>
        /// <param name="demMinLat">通视矩阵最小纬度</param>
        /// <param name="perLon">经差</param>
        /// <param name="perLat">纬差</param>
        public void DoAnalysisByPedrlLonLat(double centerLon, double centerLat,
            double toEndPointLon, double toEndPointLat, double standH,
            out int[,] result, out double demMinLon, out double demMinLat,
            out double perLon, out double perLat) =>
            DoCommomAnalysisLonLat(DoAnalysisByPedrl, centerLon, centerLat,
             toEndPointLon, toEndPointLat, standH,
            out result, out demMinLon, out demMinLat,
            out perLon, out perLat);

        /// <summary>
        /// 进行PDERL通视分析（在DEM文件投影的基础上）
        /// </summary>
        /// <param name="centerX">观测点中心的X坐标(UTM的X/经纬度的lon)</param>
        /// <param name="centerY">观测点中心的Y坐标(UTM的Y/经纬度的lat)</param>
        /// <param name="centerH">观测点的地面海拔高程</param>
        /// <param name="toEndPointX">观测区域边缘的X坐标</param>
        /// <param name="toEndPointY">观测区域边缘的Y坐标</param>
        /// <param name="standH">观测点相对地面高度</param>
        /// <param name="result">结果矩阵</param>
        /// <param name="demMinX">结果矩阵的X小值</param>
        /// <param name="demMinY">结果矩阵的Y小值</param>
        /// <param name="perX">结果矩阵横间距</param>
        /// <param name="perY">结果矩阵纵间距</param>
        public void DoAnalysisByPedrl(double centerX, double centerY, double centerH,
            double toEndPointX, double toEndPointY, double standH, out int[,] result,
            out double demMinX, out double demMinY, out double perX, out double perY)
        {
            GetInitialParam(centerX, centerY, centerH, toEndPointX, toEndPointY, standH,
                out result, out demMinX, out demMinY, out perX, out perY,
                out int xCount, out int yCount, out double seeHeight, out int centerXP,
                out int centerYP, out int minXP, out int maxXP, out int minYP, out int maxYP);

            //Console.WriteLine(11);
            GetXYD(centerX, centerY, demMinX, demMinY, xCount, yCount, out double[] dX, out double[] dY);

            //Console.WriteLine(12);
            double u = 1 / (dem.DY * dem.Rdy);//纵向格网间距实地距离的倒数
            double v = 0;//用于存储某一深度的倒数
            double currentHeight = 0;
            double k = 0;
            double a = 0;
            double startBaseG = 0, currentBaseG = 0;

            LinkedLinePDE startBaseLine = null;//所有线的起始点
            LinkedLinePDE baseLine = null;//用于对比旧基准线的当前点
            double lastK = 0;//用于对比旧基准线上一次的当前点

            //以下算法认为横向间隔和纵向间隔相同
            //右半圆:从南往北，从西往东算
            int lonIndex = 0;
            int latIndex = 0;
            int resultI = 0;
            int resultJ = 0;
            double lastHeight = 0;

            int rightResultIIndex = 2 * xCount - 1;
            int topResultJIndex = 2 * yCount - 1;

            double dG = 0;//记录当前轴斜率与参考线的差值
            double dGTmp = 0;//记录差值的增量
            double crossK = 0;//临时记录当前的交点方向
            double minK = 0;//记录一次求交运算中交点区间的最小值
            LinkedLinePDE currentNewLinePart = null;//即将接入新参考线的一部分线段，在不确定该线段定义域范围时暂存于该变量

            #region 右半面
            //以下算法认为横向间隔和纵向间隔相同
            //右半圆:从南往北，从西往东算
            lonIndex = centerXP + 1;
            latIndex = minYP;
            resultI = xCount;
            resultJ = 0;
            lastHeight = dem.Height[lonIndex, latIndex - 1] - seeHeight;
            dG = 0;

            v = 1 / dX[resultI];//更新v
            //构造右半边初始参考线
            for (; latIndex <= maxYP; latIndex++, resultJ++)
            {
                //Console.WriteLine(13);
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dY[resultJ] * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLinePDE(k, a);
                    baseLine = startBaseLine;
                    startBaseG = currentHeight * v;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLinePDE(k, a));
                    baseLine = baseLine.Next;//前移一步
                }
                result[resultI, resultJ] = 1;
                lastHeight = currentHeight;
            }
            lonIndex++;
            resultI++;

            //右边逐层计算参考线与点的显隐性
            for (; lonIndex <= maxXP; lonIndex++, resultI++)//从右边第二列开始
            {
                //Console.WriteLine(14);
                latIndex = minYP;
                resultJ = 0;
                lastHeight = dem.Height[lonIndex, latIndex - 1] - seeHeight;

                v = 1 / dX[resultI];//更新v
                baseLine = startBaseLine;
                

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dY[resultJ] * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    //Console.WriteLine(15);
                    baseLine = baseLine.Next;
                    startBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }
                startBaseLine = baseLine;
                currentBaseG = startBaseG;
                while (baseLine.EndK < k)
                {
                    //Console.WriteLine(16);
                    baseLine = baseLine.Next;
                    currentBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }

                //这是当前起点轴斜率与参考线的差值
                dG = currentHeight * v - currentBaseG + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (dG >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLinePDE(k, a);
                    startBaseG = currentHeight * v;

                    result[resultI, resultJ] = 1;
                }
                else
                {
                    result[resultI, resultJ] = 0;
                }
                lastK = k;
                lastHeight = currentHeight;

                latIndex++; resultJ++;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; latIndex <= maxYP; latIndex++, resultJ++)
                {
                    //Console.WriteLine(17);
                    k = dY[resultJ] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //Console.WriteLine(18);
                        //求出轴斜率差值的增量
                        dGTmp = dG + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (dG >= 0)//当前是通视的
                        {
                            if (dGTmp < 0)//变为不通视
                            {
                                if (dG < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dGTmp >= 0)//变为通视
                            {
                                if (dG > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                    startBaseG = currentHeight * v - a * (k - crossK);
                                }
                            }
                        }
                        dG = dGTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dGTmp = dG + (k - minK) * (a - baseLine.A);
                    if (dG >= 0)//当前是通视的
                    {
                        if (dGTmp < 0)//变为不通视
                        {
                            if (dG < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dGTmp >= 0)//变为通视
                        {
                            if (dG > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                                startBaseG = currentHeight * v - a * (k - crossK);
                            }
                        }
                    }
                    dG = dGTmp;

                    if (dG >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLinePDE(k, a));
                        currentNewLinePart = currentNewLinePart.Next;
                        result[resultI, resultJ] = 1;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }


                    lastK = k;
                    lastHeight = currentHeight;
                }
            }

            #endregion

            #region 左半面
            //以下算法认为横向间隔和纵向间隔相同
            //左半圆:从北往南，从东往西算
            lonIndex = centerXP;
            latIndex = maxYP;
            resultI = xCount - 1;
            resultJ = topResultJIndex;
            lastHeight = dem.Height[lonIndex, latIndex + 1] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;

            v = -1 / dX[resultI];//更新v
            //构造右半边初始参考线
            for (; latIndex >= minYP; latIndex--, resultJ--)
            {
                //Console.WriteLine(19);
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dY[resultJ] * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLinePDE(k, a);
                    baseLine = startBaseLine;
                    startBaseG = currentHeight * v;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLinePDE(k, a));
                    baseLine = baseLine.Next;//前移一步
                }
                result[resultI, resultJ] = 1;
                lastHeight = currentHeight;
            }
            lonIndex--;
            resultI--;

            //逐层计算参考线与点的显隐性
            for (; lonIndex >= minXP; lonIndex--, resultI--)//从第二列开始
            {
                //Console.WriteLine(20);
                latIndex = maxYP;
                resultJ = topResultJIndex;
                lastHeight = dem.Height[lonIndex, latIndex + 1] - seeHeight;

                v = -1 / dX[resultI];//更新v
                baseLine = startBaseLine;

                #region 用于查错的代码
                //if (resultI == 34)
                //{
                //    string currentPoints = "";
                //    for (int latJ = minYP, j = 0; latJ <= maxYP; latJ++, j++)
                //    {
                //        double k0 = -dY[j] * v;
                //        double a0 = u * (dem.Height[lonIndex, latJ] - dem.Height[lonIndex, latJ + 1]);
                //        double b0 = v * (dem.Height[lonIndex, latJ] - standH) - a0 * k0;
                //        double s0 = (a0 * k0 + b0) / Math.Sqrt(k0 * k0 + 1);
                //        currentPoints += k0 + "," + s0 + "\r\n";
                //    }
                //    string refer_MainPoints = startBaseLine.getOSKeyPoints();
                //    string refer_Line = startBaseLine.getOSLine();
                //}
                #endregion

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dY[resultJ] * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    //Console.WriteLine(21);
                    baseLine = baseLine.Next;
                    startBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }
                startBaseLine = baseLine;
                currentBaseG = startBaseG;
                while (baseLine.EndK < k)
                {
                    //Console.WriteLine(22);
                    baseLine = baseLine.Next;
                    currentBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }

                //这是当前起点轴斜率与参考线的差值
                dG = currentHeight * v - currentBaseG + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (dG >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLinePDE(k, a);
                    startBaseG = currentHeight * v;

                    result[resultI, resultJ] = 1;
                }
                else
                {
                    result[resultI, resultJ] = 0;
                }
                lastK = k;
                lastHeight = currentHeight;

                latIndex--; resultJ--;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; latIndex >= minYP; latIndex--, resultJ--)
                {
                    k = -dY[resultJ] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //Console.WriteLine(23);
                        //求出轴斜率差值的增量
                        dGTmp = dG + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (dG >= 0)//当前是通视的
                        {
                            if (dGTmp < 0)//变为不通视
                            {
                                if (dG < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dGTmp >= 0)//变为通视
                            {
                                if (dG > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                    startBaseG = currentHeight * v - a * (k - crossK);
                                }
                            }
                        }
                        dG = dGTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dGTmp = dG + (k - minK) * (a - baseLine.A);
                    if (dG >= 0)//当前是通视的
                    {
                        if (dGTmp < 0)//变为不通视
                        {
                            if (dG < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dGTmp >= 0)//变为通视
                        {
                            if (dG > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                                startBaseG = currentHeight * v - a * (k - crossK);
                            }
                        }
                    }
                    dG = dGTmp;

                    if (dG >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLinePDE(k, a));
                        currentNewLinePart = currentNewLinePart.Next;
                        result[resultI, resultJ] = 1;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }


                    lastK = k;
                    lastHeight = currentHeight;
                }
            }
            #endregion

            u = 1 / (dem.DX * dem.Rdx);

            #region 上半面 用于清查
            //以下算法认为横向间隔和纵向间隔相同
            //上半圆:从南往北，从东往西算
            lonIndex = maxXP;
            latIndex = centerYP + 1;
            resultI = rightResultIIndex;
            resultJ = yCount;
            lastHeight = dem.Height[lonIndex + 1, latIndex] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;

            v = 1 / dY[resultJ];//更新v
            //构造右半边初始参考线
            for (; lonIndex >= minXP; lonIndex--, resultI--)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dX[resultI] * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLinePDE(k, a);
                    baseLine = startBaseLine;
                    startBaseG = currentHeight * v;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLinePDE(k, a));
                    baseLine = baseLine.Next;//前移一步
                }
                lastHeight = currentHeight;
            }
            latIndex++;
            resultJ++;

            //逐层计算参考线与点的显隐性
            for (; latIndex <= maxYP; latIndex++, resultJ++)//从第二列开始
            {
                lonIndex = maxXP;
                resultI = rightResultIIndex;
                lastHeight = dem.Height[lonIndex + 1, latIndex] - seeHeight;

                v = 1 / dY[resultJ];//更新v
                baseLine = startBaseLine;

                #region 用于查错的代码
                //if (resultJ == 40)
                //{
                //    string currentPoints = "";
                //    for (int lonI = maxXP, i = rightResultIIndex; lonI >= minXP; lonI--, i--)
                //    {
                //        double k0 = -dX[i] * v;
                //        double a0 = u * (dem.Height[lonI, latIndex] - dem.Height[lonI + 1, latIndex]);
                //        double g0 = (dem.Height[lonI, latIndex] - standH) * v;
                //        currentPoints += k0 + "," + g0 + "\r\n";
                //    }
                //    string refer_MainPoints = startBaseLine.getOSKeyPoints();
                //}
                #endregion

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dX[resultI] * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    baseLine = baseLine.Next;
                    startBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }
                startBaseLine = baseLine;
                currentBaseG = startBaseG;
                while (baseLine.EndK < k)
                {
                    baseLine = baseLine.Next;
                    currentBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }

                //这是当前起点轴斜率与参考线的差值
                dG = currentHeight * v - currentBaseG + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (dG >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLinePDE(k, a);
                    startBaseG = currentHeight * v;

                }
                else
                {
                    result[resultI, resultJ] = 0;
                }
                lastK = k;
                lastHeight = currentHeight;


                lonIndex--; resultI--;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; lonIndex >= minXP; lonIndex--, resultI--)
                {
                    k = -dX[resultI] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //求出轴斜率差值的增量
                        dGTmp = dG + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (dG >= 0)//当前是通视的
                        {
                            if (dGTmp < 0)//变为不通视
                            {
                                if (dG < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dGTmp >= 0)//变为通视
                            {
                                if (dG > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                    startBaseG = currentHeight * v - a * (k - crossK);
                                }
                            }
                        }
                        dG = dGTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dGTmp = dG + (k - minK) * (a - baseLine.A);
                    if (dG >= 0)//当前是通视的
                    {
                        if (dGTmp < 0)//变为不通视
                        {
                            if (dG < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dGTmp >= 0)//变为通视
                        {
                            if (dG > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                                startBaseG = currentHeight * v - a * (k - crossK);
                            }
                        }
                    }
                    dG = dGTmp;

                    if (dG >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLinePDE(k, a));
                        currentNewLinePart = currentNewLinePart.Next;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }




                    lastK = k;
                    lastHeight = currentHeight;
                }
            }

            #endregion

            #region 下半面 用于清查
            //以下算法认为横向间隔和纵向间隔相同
            //上半圆:从南往北，从东往西算
            lonIndex = minXP;
            latIndex = centerYP;
            resultI = 0;
            resultJ = yCount - 1;
            lastHeight = dem.Height[lonIndex - 1, latIndex] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;

            v = -1 / dY[resultJ];//更新v
            //构造右半边初始参考线
            for (; lonIndex <= maxXP; lonIndex++, resultI++)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dX[resultI] * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLinePDE(k, a);
                    baseLine = startBaseLine;
                    startBaseG = currentHeight * v;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLinePDE(k, a));
                    baseLine = baseLine.Next;//前移一步
                }
                lastHeight = currentHeight;
            }
            latIndex--;
            resultJ--;

            //逐层计算参考线与点的显隐性
            for (; latIndex >= minYP; latIndex--, resultJ--)//从第二列开始
            {
                lonIndex = minXP;
                resultI = 0;
                lastHeight = dem.Height[lonIndex - 1, latIndex] - seeHeight;

                v = -1 / dY[resultJ];//更新v
                baseLine = startBaseLine;


                #region 用于查错的代码
                //if (resultJ == 1)
                //{
                //    string currentPoints = "";
                //    for (int lonI = minXP, i = 0; lonI <= maxXP; lonI++, i++)
                //    {
                //        double k0 = dX[i] * v;
                //        double a0 = u * (dem.Height[lonI, latIndex] - dem.Height[lonI - 1, latIndex]);
                //        double g0 = (dem.Height[lonI, latIndex] - standH) * v;
                //        currentPoints += k0 + "," + g0 + "\r\n";
                //    }
                //    string refer_MainPoints = startBaseLine.getOSKeyPoints();
                //}
                #endregion

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dX[resultI] * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    baseLine = baseLine.Next;
                    startBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }
                startBaseLine = baseLine;
                currentBaseG = startBaseG;
                while (baseLine.EndK < k)
                {
                    baseLine = baseLine.Next;
                    currentBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }

                //这是当前起点轴斜率与参考线的差值
                dG = currentHeight * v - currentBaseG + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (dG >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLinePDE(k, a);
                    startBaseG = currentHeight * v;

                }
                else
                {
                    result[resultI, resultJ] = 0;
                }

                lastK = k;
                lastHeight = currentHeight;

                lonIndex++; resultI++;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; lonIndex <= maxXP; lonIndex++, resultI++)
                {
                    k = dX[resultI] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //求出轴斜率差值的增量
                        dGTmp = dG + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (dG >= 0)//当前是通视的
                        {
                            if (dGTmp < 0)//变为不通视
                            {
                                if (dG < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dGTmp >= 0)//变为通视
                            {
                                if (dG > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                    startBaseG = currentHeight * v - a * (k - crossK);
                                }
                            }
                        }
                        dG = dGTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dGTmp = dG + (k - minK) * (a - baseLine.A);
                    if (dG >= 0)//当前是通视的
                    {
                        if (dGTmp < 0)//变为不通视
                        {
                            if (dG < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dGTmp >= 0)//变为通视
                        {
                            if (dG > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                                startBaseG = currentHeight * v - a * (k - crossK);
                            }
                        }
                    }
                    dG = dGTmp;

                    if (dG >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLinePDE(k, a));
                        currentNewLinePart = currentNewLinePart.Next;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }


                    lastK = k;
                    lastHeight = currentHeight;
                }
            }
            #endregion
        }

        #endregion

        #region PDERL算法（用于统计PDERL算法中地形的交叉点等信息，实现上和以上函数完全一样，仅添加了统计内容）

        /// <summary>
        /// 最长参考线长度（在正式算法中将不计算该值）
        /// </summary>
        public int pde_max_refer_length = 0;
        /// <summary>
        /// 参考线总长度（在正式算法中将不计算该值）
        /// </summary>
        public int pde_all_refer_length = 0;
        /// <summary>
        /// 交叉点总数（在正式算法中将不计算该值）
        /// </summary>
        public int pde_cross_point_count = 0;

        public double[,] recordG_refer;

        /// <summary>
        /// 进行PDERL通视分析（在经纬度坐标的基础上）
        /// </summary>
        public void DoAnalysisByPderlLonLat_Refer(double centerLon, double centerLat,
            double toEndPointLon, double toEndPointLat, double standH,
            out int[,] result, out double demMinLon, out double demMinLat,
            out double perLon, out double perLat) =>
            DoCommomAnalysisLonLat(DoAnalysisByPderl_Refer, centerLon, centerLat,
             toEndPointLon, toEndPointLat, standH,
            out result, out demMinLon, out demMinLat,
            out perLon, out perLat);

        /// <summary>
        /// 进行PDERL分析（在DEM文件投影的基础上）
        /// </summary>
        public void DoAnalysisByPderl_Refer(double centerX, double centerY, double centerH,
            double toEndPointX, double toEndPointY, double standH, out int[,] result,
            out double demMinX, out double demMinY, out double perX, out double perY)
        {
            pde_all_refer_length = 0;
            pde_cross_point_count = 0;
            pde_max_refer_length = 0;

            GetInitialParam(centerX, centerY, centerH, toEndPointX, toEndPointY, standH,
                out result, out demMinX, out demMinY, out perX, out perY,
                out int xCount, out int yCount, out double seeHeight, out int centerXP,
                out int centerYP, out int minXP, out int maxXP, out int minYP, out int maxYP);

            GetXYD(centerX, centerY, demMinX, demMinY, xCount, yCount, out double[] dX, out double[] dY);

            recordG_refer = new double[result.GetLength(0), result.GetLength(1)];

            double u = 1 / (dem.DY * dem.Rdy);//纵向格网间距实地距离的倒数
            double v = 0;//用于存储某一深度的倒数
            double currentHeight = 0;
            double k = 0;
            double a = 0;
            double g = 0;

            LinkedLine startBaseLine = null;//所有线的起始点
            LinkedLine baseLine = null;//用于对比旧基准线的当前点
            double lastK = 0;//用于对比旧基准线上一次的当前点

            //以下算法认为横向间隔和纵向间隔相同
            //右半圆:从南往北，从西往东算
            int lonIndex = 0;
            int latIndex = 0;
            int resultI = 0;
            int resultJ = 0;
            double lastHeight = 0;

            int rightResultIIndex = 2 * xCount - 1;
            int topResultJIndex = 2 * yCount - 1;

            double d = 0;//记录当前轴斜率与参考线的差值
            double dTmp = 0;//记录差值的增量
            double crossK = 0;//临时记录当前的交点方向
            double minK = 0;//记录一次求交运算中交点区间的最小值
            LinkedLine currentNewLinePart = null;//即将接入新参考线的一部分线段，在不确定该线段定义域范围时暂存于该变量

            #region 右半面
            //以下算法认为横向间隔和纵向间隔相同
            //右半圆:从南往北，从西往东算
            lonIndex = centerXP + 1;
            latIndex = minYP;
            resultI = xCount;
            resultJ = 0;
            lastHeight = dem.Height[lonIndex, latIndex - 1] - seeHeight;
            d = 0;

            v = 1 / dX[resultI];//更新v
            //构造右半边初始参考线
            for (; latIndex <= maxYP; latIndex++, resultJ++)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dY[resultJ] * v;
                g = currentHeight * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLine(k, a, g);
                    baseLine = startBaseLine;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLine(k, a, g));
                    baseLine = baseLine.Next;//前移一步
                }
                result[resultI, resultJ] = 1;
                lastHeight = currentHeight;
            }
            lonIndex++;
            resultI++;

            //右边逐层计算参考线与点的显隐性
            for (; lonIndex <= maxXP; lonIndex++, resultI++)//从右边第二列开始
            {
                latIndex = minYP;
                resultJ = 0;
                lastHeight = dem.Height[lonIndex, latIndex - 1] - seeHeight;

                v = 1 / dX[resultI];//更新v
                baseLine = startBaseLine;

                #region 用于查错的代码
                //if (resultI == 107)
                //{
                //    string currentPoints = "";
                //    for (int latJ = minYP, j = 0; latJ <= maxYP; latJ++, j++)
                //    {
                //        double k0 = dY[j] * v;
                //        double a0 = u * (dem.Height[lonIndex, latJ] - dem.Height[lonIndex, latJ - 1]);
                //        double b0 = v * (dem.Height[lonIndex, latJ] - standH) - a0 * k0;
                //        double s0 = (a0 * k0 + b0) / Math.Sqrt(k0 * k0 + 1);
                //        currentPoints += k0 + "," + s0 + "\r\n";
                //    }
                //    string refer_MainPoints = startBaseLine.getOSKeyPoints();
                //    string refer_Line = startBaseLine.getOSLine();
                //}
                #endregion


                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dY[resultJ] * v;
                g = currentHeight * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    baseLine = baseLine.Next;
                }
                startBaseLine = baseLine;
                while (baseLine.EndK < k)
                {
                    baseLine = baseLine.Next;
                }

                //这是当前起点轴斜率与参考线的差值
                d = g - baseLine.G + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (d >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLine(k, a, g);
                    result[resultI, resultJ] = 1;
                }
                else
                {
                    result[resultI, resultJ] = 0;
                }
                lastK = k;
                lastHeight = currentHeight;

                latIndex++; resultJ++;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; latIndex <= maxYP; latIndex++, resultJ++)
                {
                    k = dY[resultJ] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    g = currentHeight * v;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //求出轴斜率差值的增量
                        dTmp = d + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (Math.Abs(dTmp) < 1e-15)
                            ;

                        if (d >= 0)//当前是通视的
                        {
                            if (dTmp < 0)//变为不通视
                            {
                                if (d < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + d / (baseLine.A - a);
                                pde_cross_point_count++;

                                currentNewLinePart.LinkForword(new LinkedLine(crossK, a, a * (crossK - k) + g));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dTmp >= 0)//变为通视
                            {
                                if (d > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + d / (baseLine.A - a);
                                pde_cross_point_count++;

                                currentNewLinePart = new LinkedLine(crossK, baseLine.A, a * (crossK - k) + g);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                }
                            }
                        }
                        d = dTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dTmp = d + (k - minK) * (a - baseLine.A);
                    if (Math.Abs(dTmp) < 1e-15)
                        ;
                    if (d >= 0)//当前是通视的
                    {
                        if (dTmp < 0)//变为不通视
                        {
                            if (d < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + d / (baseLine.A - a);
                            pde_cross_point_count++;

                            currentNewLinePart.LinkForword(new LinkedLine(crossK, a, a * (crossK - k) + g));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dTmp >= 0)//变为通视
                        {
                            if (d > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + d / (baseLine.A - a);
                            pde_cross_point_count++;

                            currentNewLinePart = new LinkedLine(crossK, baseLine.A, a * (crossK - k) + g);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                            }
                        }
                    }
                    d = dTmp;

                    if (d >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLine(k, a, g));
                        currentNewLinePart = currentNewLinePart.Next;
                        result[resultI, resultJ] = 1;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }
                    recordG_refer[resultI, resultJ] = d;


                    lastK = k;
                    lastHeight = currentHeight;
                }
                int refer_count = startBaseLine.CountToLine(baseLine);//数到当前线或者结束位置
                if (pde_max_refer_length < refer_count)
                    pde_max_refer_length = refer_count;
                pde_all_refer_length += refer_count;
            }

            #endregion

            #region 左半面
            //以下算法认为横向间隔和纵向间隔相同
            //左半圆:从北往南，从东往西算
            lonIndex = centerXP;
            latIndex = maxYP;
            resultI = xCount - 1;
            resultJ = topResultJIndex;
            lastHeight = dem.Height[lonIndex, latIndex + 1] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;

            v = -1 / dX[resultI];//更新v
            //构造右半边初始参考线
            for (; latIndex >= minYP; latIndex--, resultJ--)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dY[resultJ] * v;
                g = currentHeight * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLine(k, a, g);
                    baseLine = startBaseLine;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLine(k, a, g));
                    baseLine = baseLine.Next;//前移一步
                }
                result[resultI, resultJ] = 1;
                lastHeight = currentHeight;
            }
            lonIndex--;
            resultI--;

            //逐层计算参考线与点的显隐性
            for (; lonIndex >= minXP; lonIndex--, resultI--)//从第二列开始
            {
                latIndex = maxYP;
                resultJ = topResultJIndex;
                lastHeight = dem.Height[lonIndex, latIndex + 1] - seeHeight;

                v = -1 / dX[resultI];//更新v
                baseLine = startBaseLine;

                #region 用于查错的代码
                //if (resultI == 34)
                //{
                //    string currentPoints = "";
                //    for (int latJ = minYP, j = 0; latJ <= maxYP; latJ++, j++)
                //    {
                //        double k0 = -dY[j] * v;
                //        double a0 = u * (dem.Height[lonIndex, latJ] - dem.Height[lonIndex, latJ + 1]);
                //        double b0 = v * (dem.Height[lonIndex, latJ] - standH) - a0 * k0;
                //        double s0 = (a0 * k0 + b0) / Math.Sqrt(k0 * k0 + 1);
                //        currentPoints += k0 + "," + s0 + "\r\n";
                //    }
                //    string refer_MainPoints = startBaseLine.getOSKeyPoints();
                //    string refer_Line = startBaseLine.getOSLine();
                //}
                #endregion

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dY[resultJ] * v;
                g = currentHeight * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    baseLine = baseLine.Next;
                }
                startBaseLine = baseLine;
                while (baseLine.EndK < k)
                {
                    baseLine = baseLine.Next;
                }

                //这是当前起点轴斜率与参考线的差值
                d = g - baseLine.G + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (d >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLine(k, a, g);
                    result[resultI, resultJ] = 1;
                }
                else
                {
                    result[resultI, resultJ] = 0;
                }
                lastK = k;
                lastHeight = currentHeight;

                latIndex--; resultJ--;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; latIndex >= minYP; latIndex--, resultJ--)
                {
                    k = -dY[resultJ] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    g = currentHeight * v;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //求出轴斜率差值的增量
                        dTmp = d + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (Math.Abs(dTmp) < 1e-15)
                            ;
                        if (d >= 0)//当前是通视的
                        {
                            if (dTmp < 0)//变为不通视
                            {
                                if (d < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + d / (baseLine.A - a);
                                pde_cross_point_count++;

                                currentNewLinePart.LinkForword(new LinkedLine(crossK, a, a * (crossK - k) + g));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dTmp >= 0)//变为通视
                            {
                                if (d > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + d / (baseLine.A - a);
                                pde_cross_point_count++;

                                currentNewLinePart = new LinkedLine(crossK, baseLine.A, a * (crossK - k) + g);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                }
                            }
                        }
                        d = dTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dTmp = d + (k - minK) * (a - baseLine.A);
                    if (Math.Abs(dTmp) < 1e-15)
                        ;
                    if (d >= 0)//当前是通视的
                    {
                        if (dTmp < 0)//变为不通视
                        {
                            if (d < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + d / (baseLine.A - a);
                            pde_cross_point_count++;

                            currentNewLinePart.LinkForword(new LinkedLine(crossK, a, a * (crossK - k) + g));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dTmp >= 0)//变为通视
                        {
                            if (d > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + d / (baseLine.A - a);
                            pde_cross_point_count++;

                            currentNewLinePart = new LinkedLine(crossK, baseLine.A, a * (crossK - k) + g);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                            }
                        }
                    }
                    d = dTmp;

                    if (d >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLine(k, a, g));
                        currentNewLinePart = currentNewLinePart.Next;
                        result[resultI, resultJ] = 1;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }
                    recordG_refer[resultI, resultJ] = d;


                    lastK = k;
                    lastHeight = currentHeight;
                }
                int refer_count = startBaseLine.CountToLine(baseLine);//数到当前线或者结束位置
                if (pde_max_refer_length < refer_count)
                    pde_max_refer_length = refer_count;
                pde_all_refer_length += refer_count;
            }
            #endregion

            u = 1 / (dem.DX * dem.Rdx);

            #region 上半面 用于清查
            //以下算法认为横向间隔和纵向间隔相同
            //上半圆:从南往北，从东往西算
            lonIndex = maxXP;
            latIndex = centerYP + 1;
            resultI = rightResultIIndex;
            resultJ = yCount;
            lastHeight = dem.Height[lonIndex + 1, latIndex] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;

            v = 1 / dY[resultJ];//更新v
            //构造右半边初始参考线
            for (; lonIndex >= minXP; lonIndex--, resultI--)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dX[resultI] * v;
                g = currentHeight * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLine(k, a, g);
                    baseLine = startBaseLine;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLine(k, a, g));
                    baseLine = baseLine.Next;//前移一步
                }
                lastHeight = currentHeight;
            }
            latIndex++;
            resultJ++;

            //逐层计算参考线与点的显隐性
            for (; latIndex <= maxYP; latIndex++, resultJ++)//从第二列开始
            {
                lonIndex = maxXP;
                resultI = rightResultIIndex;
                lastHeight = dem.Height[lonIndex + 1, latIndex] - seeHeight;

                v = 1 / dY[resultJ];//更新v
                baseLine = startBaseLine;

                #region 用于查错的代码
                //if (resultJ == 40)
                //{
                //    string currentPoints = "";
                //    for (int lonI = maxXP, i = rightResultIIndex; lonI >= minXP; lonI--, i--)
                //    {
                //        double k0 = -dX[i] * v;
                //        double a0 = u * (dem.Height[lonI, latIndex] - dem.Height[lonI + 1, latIndex]);
                //        double g0 = (dem.Height[lonI, latIndex] - standH) * v;
                //        currentPoints += k0 + "," + g0 + "\r\n";
                //    }
                //    string refer_MainPoints = startBaseLine.getOSKeyPoints();
                //}
                #endregion

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dX[resultI] * v;
                g = currentHeight * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    baseLine = baseLine.Next;
                }
                startBaseLine = baseLine;
                while (baseLine.EndK < k)
                {
                    baseLine = baseLine.Next;
                }

                //这是当前起点轴斜率与参考线的差值
                d = g - baseLine.G + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (d >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLine(k, a, g);
                }
                else
                {
                    result[resultI, resultJ] = 0;
                }

                lastK = k;
                lastHeight = currentHeight;


                lonIndex--; resultI--;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; lonIndex >= minXP; lonIndex--, resultI--)
                {
                    //if (resultI == 17 && resultJ == 40)
                    //{

                    //}

                    k = -dX[resultI] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    g = currentHeight * v;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //求出轴斜率差值的增量
                        dTmp = d + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (Math.Abs(dTmp) < 1e-15)
                            ;
                        if (d >= 0)//当前是通视的
                        {
                            if (dTmp < 0)//变为不通视
                            {
                                if (d < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + d / (baseLine.A - a);
                                pde_cross_point_count++;

                                currentNewLinePart.LinkForword(new LinkedLine(crossK, a, a * (crossK - k) + g));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dTmp >= 0)//变为通视
                            {
                                if (d > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + d / (baseLine.A - a);
                                pde_cross_point_count++;

                                currentNewLinePart = new LinkedLine(crossK, baseLine.A, a * (crossK - k) + g);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                }
                            }
                        }
                        d = dTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dTmp = d + (k - minK) * (a - baseLine.A);
                    if (Math.Abs(dTmp) < 1e-15)
                        ;
                    if (d >= 0)//当前是通视的
                    {
                        if (dTmp < 0)//变为不通视
                        {
                            if (d < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + d / (baseLine.A - a);
                            pde_cross_point_count++;

                            currentNewLinePart.LinkForword(new LinkedLine(crossK, a, a * (crossK - k) + g));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dTmp >= 0)//变为通视
                        {
                            if (d > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + d / (baseLine.A - a);
                            pde_cross_point_count++;

                            currentNewLinePart = new LinkedLine(crossK, baseLine.A, a * (crossK - k) + g);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                            }
                        }
                    }
                    d = dTmp;

                    if (d >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLine(k, a, g));
                        currentNewLinePart = currentNewLinePart.Next;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }
                    recordG_refer[resultI, resultJ] = d;


                    lastK = k;
                    lastHeight = currentHeight;
                }

                int refer_count = startBaseLine.CountToLine(baseLine);//数到当前线或者结束位置
                if (pde_max_refer_length < refer_count)
                    pde_max_refer_length = refer_count;
                pde_all_refer_length += refer_count;
            }

            #endregion

            #region 下半面 用于清查
            //以下算法认为横向间隔和纵向间隔相同
            //上半圆:从南往北，从东往西算
            lonIndex = minXP;
            latIndex = centerYP;
            resultI = 0;
            resultJ = yCount - 1;
            lastHeight = dem.Height[lonIndex - 1, latIndex] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;

            v = -1 / dY[resultJ];//更新v
            //构造右半边初始参考线
            for (; lonIndex <= maxXP; lonIndex++, resultI++)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dX[resultI] * v;
                g = currentHeight * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLine(k, a, g);
                    baseLine = startBaseLine;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLine(k, a, g));
                    baseLine = baseLine.Next;//前移一步
                }
                lastHeight = currentHeight;
            }
            latIndex--;
            resultJ--;

            //逐层计算参考线与点的显隐性
            for (; latIndex >= minYP; latIndex--, resultJ--)//从第二列开始
            {
                lonIndex = minXP;
                resultI = 0;
                lastHeight = dem.Height[lonIndex - 1, latIndex] - seeHeight;

                v = -1 / dY[resultJ];//更新v
                baseLine = startBaseLine;


                #region 用于查错的代码
                //if (resultJ == 1)
                //{
                //    string currentPoints = "";
                //    for (int lonI = minXP, i = 0; lonI <= maxXP; lonI++, i++)
                //    {
                //        double k0 = dX[i] * v;
                //        double a0 = u * (dem.Height[lonI, latIndex] - dem.Height[lonI - 1, latIndex]);
                //        double g0 = (dem.Height[lonI, latIndex] - standH) * v;
                //        currentPoints += k0 + "," + g0 + "\r\n";
                //    }
                //    string refer_MainPoints = startBaseLine.getOSKeyPoints();
                //}
                #endregion

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dX[resultI] * v;
                g = currentHeight * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    baseLine = baseLine.Next;
                }
                startBaseLine = baseLine;
                while (baseLine.EndK < k)
                {
                    baseLine = baseLine.Next;
                }

                //这是当前起点轴斜率与参考线的差值
                d = g - baseLine.G + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (d >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLine(k, a, g);
                }
                else
                {
                    result[resultI, resultJ] = 0;
                }

                lastK = k;
                lastHeight = currentHeight;

                lonIndex++; resultI++;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; lonIndex <= maxXP; lonIndex++, resultI++)
                {
                    k = dX[resultI] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    g = currentHeight * v;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //求出轴斜率差值的增量
                        dTmp = d + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (Math.Abs(dTmp) < 1e-15)
                            ;
                        if (d >= 0)//当前是通视的
                        {
                            if (dTmp < 0)//变为不通视
                            {
                                if (d < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + d / (baseLine.A - a);
                                pde_cross_point_count++;

                                currentNewLinePart.LinkForword(new LinkedLine(crossK, a, a * (crossK - k) + g));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dTmp >= 0)//变为通视
                            {
                                if (d > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + d / (baseLine.A - a);
                                pde_cross_point_count++;

                                currentNewLinePart = new LinkedLine(crossK, baseLine.A, a * (crossK - k) + g);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                }
                            }
                        }
                        d = dTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dTmp = d + (k - minK) * (a - baseLine.A);
                    if (Math.Abs(dTmp) < 1e-15)
                        ;
                    if (d >= 0)//当前是通视的
                    {
                        if (dTmp < 0)//变为不通视
                        {
                            if (d < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + d / (baseLine.A - a);
                            pde_cross_point_count++;

                            currentNewLinePart.LinkForword(new LinkedLine(crossK, a, a * (crossK - k) + g));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dTmp >= 0)//变为通视
                        {
                            if (d > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + d / (baseLine.A - a);
                            pde_cross_point_count++;

                            currentNewLinePart = new LinkedLine(crossK, baseLine.A, a * (crossK - k) + g);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                            }
                        }
                    }
                    d = dTmp;

                    if (d >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLine(k, a, g));
                        currentNewLinePart = currentNewLinePart.Next;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }
                    recordG_refer[resultI, resultJ] = d;


                    lastK = k;
                    lastHeight = currentHeight;
                }

                int refer_count = startBaseLine.CountToLine(baseLine);//数到当前线或者结束位置
                if (pde_max_refer_length < refer_count)
                    pde_max_refer_length = refer_count;
                pde_all_refer_length += refer_count;
            }
            #endregion
        }

        #endregion

        #region XPDERL算法
        /// <summary>
        /// 进行XPDERL通视分析（在经纬度坐标的基础上）
        /// </summary>
        /// <param name="centerLon">观测点中心的经度</param>
        /// <param name="centerLat">观测点中心的纬度</param>
        /// <param name="toEndPointLon">观测区域边缘的经度</param>
        /// <param name="toEndPointLat">观测区域边缘的纬度</param>
        /// <param name="standH">观测点相对地面高程</param>
        /// <param name="result">通视矩阵</param>
        /// <param name="demMinLon">通视矩阵最小经度</param>
        /// <param name="demMinLat">通视矩阵最小纬度</param>
        /// <param name="perLon">经差</param>
        /// <param name="perLat">纬差</param> 
        public void DoAnalysisByXPderlLonLat(double centerLon, double centerLat,
            double toEndPointLon, double toEndPointLat, double standH,
            out int[,] result, out double demMinLon, out double demMinLat,
            out double perLon, out double perLat) =>
            DoCommomAnalysisLonLat(DoAnalysisByXPderl, centerLon, centerLat,
             toEndPointLon, toEndPointLat, standH,
            out result, out demMinLon, out demMinLat,
            out perLon, out perLat);

        /// <summary>
        /// 进行XPDERL通视分析（在DEM文件投影的基础上）
        /// </summary>
        /// <param name="centerX">观测点中心的X坐标(UTM的X/经纬度的lon)</param>
        /// <param name="centerY">观测点中心的Y坐标(UTM的Y/经纬度的lat)</param>
        /// <param name="centerH">观测点的地面海拔高程</param>
        /// <param name="toEndPointX">观测区域边缘的X坐标</param>
        /// <param name="toEndPointY">观测区域边缘的Y坐标</param>
        /// <param name="standH">观测点相对地面高度</param>
        /// <param name="result">结果矩阵</param>
        /// <param name="demMinX">结果矩阵的X小值</param>
        /// <param name="demMinY">结果矩阵的Y小值</param>
        /// <param name="perX">结果矩阵横间距</param>
        /// <param name="perY">结果矩阵纵间距</param>
        public void DoAnalysisByXPderl(double centerX, double centerY, double centerH,
            double toEndPointX, double toEndPointY, double standH, out int[,] result,
            out double demMinX, out double demMinY, out double perX, out double perY)
        {
            /// <param name="demMinX">横轴最小值</param>
            /// <param name="demMinY">竖轴最大值</param>
            /// <param name="perX">横间距</param>
            /// <param name="perY">纵间距</param>
            /// <param name="xCount">计算的横格网数</param>
            /// <param name="yCount">计算的纵格网数</param>
            /// <param name="seeheight">观测视点的海拔高度</param>
            /// <param name="centerXP">观测点在第几个格网里</param>
            /// <param name="centerYP">观测点在第几个格网里</param>
            /// <param name="minXP">格网边界</param>
            /// <param name="maxXP">格网边界</param>
            /// <param name="minYP">格网边界</param>
            /// <param name="maxYP">格网边界</param>
            GetInitialParam(centerX, centerY, centerH, toEndPointX, toEndPointY, standH,
                out result, out demMinX, out demMinY, out perX, out perY,
                out int xCount, out int yCount, out double seeHeight, out int centerXP,
                out int centerYP, out int minXP, out int maxXP, out int minYP, out int maxYP);

            //Console.WriteLine(24);
            GetXYD(centerX, centerY, demMinX, demMinY, xCount, yCount, out double[] dX, out double[] dY);

            //Console.WriteLine(12);
            double u = 1 / (dem.DY * dem.Rdy);//纵向格网间距实地距离的倒数
            double v = 0;//用于存储某一深度的倒数
            double currentHeight = 0;
            double k = 0;
            double a = 0;
            double startBaseG = 0, currentBaseG = 0;

            LinkedLinePDE startBaseLine = null;//所有线的起始点
            LinkedLinePDE baseLine = null;//用于对比旧基准线的当前点
            double lastK = 0;//用于对比旧基准线上一次的当前点

            //以下算法认为横向间隔和纵向间隔相同
            //右半圆:从南往北，从西往东算
            int lonIndex = 0;
            int latIndex = 0;
            int resultI = 0;
            int resultJ = 0;
            double lastHeight = 0;

            int rightResultIIndex = 2 * xCount - 1;
            int topResultJIndex = 2 * yCount - 1;

            double dG = 0;//记录当前轴斜率与参考线的差值
            double dGTmp = 0;//记录差值的增量
            double crossK = 0;//临时记录当前的交点方向
            double minK = 0;//记录一次求交运算中交点区间的最小值
            LinkedLinePDE currentNewLinePart = null;//即将接入新参考线的一部分线段，在不确定该线段定义域范围时暂存于该变量
            
            //求出中心点在中心格网的相对位置
            double centerGridLon = (centerX - demMinX) % dem.DX;
            double centerGridLat = (centerY - demMinY) % dem.DY;
            bool isCenterInLeftTop = centerGridLat / centerGridLon - dem.DY / dem.DX >= 0;
            bool isCenterInLeftBottom = dem.DY * centerGridLon + dem.DX * centerGridLat <= dem.DX * dem.DY;

            #region 右半面
            //以下算法认为横向间隔和纵向间隔相同
            //右半圆:从南往北，从西往东算
            int startIndexAdjust = isCenterInLeftBottom ? -1 : 0;
            int endIndexAdjust = isCenterInLeftTop ? 1 : 0;

            //构造右半边初始参考线
            int startLatUtmIndex = centerYP + startIndexAdjust;
            int startResultJIndex = yCount - 1 + startIndexAdjust;
            int endLatUtmIndex = centerYP + 1 + endIndexAdjust;

            lonIndex = centerXP + 1;
            latIndex = startLatUtmIndex;
            resultI = xCount;
            resultJ = startResultJIndex;
            v = 1 / dX[resultI];
            lastHeight = dem.Height[lonIndex, latIndex - 1] - seeHeight;

            for (; latIndex <= endLatUtmIndex; latIndex++, resultJ++)
            {
                //Console.WriteLine(13);
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dY[resultJ] * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLinePDE(k, a);
                    baseLine = startBaseLine;
                    startBaseG = currentHeight * v;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLinePDE(k, a));
                    baseLine = baseLine.Next;//前移一步
                }
                result[resultI, resultJ] = 1;
                lastHeight = currentHeight;
            }
            //右边逐层计算参考线与点的显隐性
            startLatUtmIndex--;
            startResultJIndex--;
            endLatUtmIndex++;
            lonIndex++;
            resultI++;

            for (;
                lonIndex <= maxXP;
                lonIndex++, resultI++, startLatUtmIndex--, startResultJIndex--, endLatUtmIndex++)//从右边第二列开始
            {
                //Console.WriteLine(27);
                if (startResultJIndex < 0)//保证不超出
                {
                    startLatUtmIndex++;
                    startResultJIndex = 0;
                }


                resultJ = startResultJIndex;
                latIndex = startLatUtmIndex;
                lastHeight = dem.Height[lonIndex, latIndex - 1] - seeHeight;

                v = 1 / dX[resultI];//更新v
                baseLine = startBaseLine;

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dY[resultJ] * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    //Console.WriteLine(15);
                    baseLine = baseLine.Next;
                    startBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }
                startBaseLine = baseLine;
                currentBaseG = startBaseG;
                while (baseLine.EndK < k)
                {
                    //Console.WriteLine(16);
                    baseLine = baseLine.Next;
                    currentBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }

                //这是当前起点轴斜率与参考线的差值
                dG = currentHeight * v - currentBaseG + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (dG >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLinePDE(k, a);
                    startBaseG = currentHeight * v;

                    result[resultI, resultJ] = 1;
                }
                else
                {
                    result[resultI, resultJ] = 0;
                }
                lastK = k;
                lastHeight = currentHeight;

                latIndex++; resultJ++;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; latIndex <= endLatUtmIndex && latIndex <= maxYP;
                    latIndex++, resultJ++)
                {

                    //Console.WriteLine(17);
                    k = dY[resultJ] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //Console.WriteLine(18);
                        //求出轴斜率差值的增量
                        dGTmp = dG + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (dG >= 0)//当前是通视的
                        {
                            if (dGTmp < 0)//变为不通视
                            {
                                if (dG < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dGTmp >= 0)//变为通视
                            {
                                if (dG > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                    startBaseG = currentHeight * v - a * (k - crossK);
                                }
                            }
                        }
                        dG = dGTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dGTmp = dG + (k - minK) * (a - baseLine.A);
                    if (dG >= 0)//当前是通视的
                    {
                        if (dGTmp < 0)//变为不通视
                        {
                            if (dG < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dGTmp >= 0)//变为通视
                        {
                            if (dG > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                                startBaseG = currentHeight * v - a * (k - crossK);
                            }
                        }
                    }
                    dG = dGTmp;

                    if (dG >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLinePDE(k, a));
                        currentNewLinePart = currentNewLinePart.Next;
                        result[resultI, resultJ] = 1;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }


                    lastK = k;
                    lastHeight = currentHeight;
                }
            }
            #endregion


            u = 1 / (dem.DX * dem.Rdx);

            #region 上半面
            //上半圆:从南往北，从东往西算
            bool isCenterInRightBottom = centerGridLat / centerGridLon - dem.DY / dem.DX <= 0;

            startIndexAdjust = isCenterInRightBottom ? 1 : 0;
            endIndexAdjust = isCenterInLeftBottom ? -1 : 0;
            
            int startLonUtmIndex = centerXP + 1 + startIndexAdjust;
            int startResultIIndex = xCount + startIndexAdjust;
            int endLonUtmIndex = centerXP + endIndexAdjust;


            lonIndex = startLonUtmIndex;
            latIndex = centerYP + 1;
            resultI = startResultIIndex;
            resultJ = yCount;
            lastHeight = dem.Height[lonIndex + 1, latIndex] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;
            v = 1 / dY[resultJ];

            //构造初始参考线
            for (; lonIndex >= endLonUtmIndex; lonIndex--, resultI--)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dX[resultI] * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLinePDE(k, a);
                    baseLine = startBaseLine;
                    startBaseG = currentHeight * v;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLinePDE(k, a));
                    baseLine = baseLine.Next;//前移一步
                }
                lastHeight = currentHeight;
            }
            latIndex++;
            resultJ++;
            startLonUtmIndex++;
            startResultIIndex++;
            endLonUtmIndex--;

            //上边逐层计算参考线与点的显隐性
            int maxRightIndex = 2 * xCount - 1;
            for (; latIndex <= maxYP;
                latIndex++, resultJ++, startLonUtmIndex++, startResultIIndex++, endLonUtmIndex--)//从右边第二列开始
            {

                if (startResultIIndex > maxRightIndex)//保证不超出
                {
                    startLonUtmIndex--;
                    startResultIIndex = maxRightIndex;
                }
                resultI = startResultIIndex;
                lonIndex = startLonUtmIndex;
                lastHeight = dem.Height[lonIndex + 1, latIndex] - seeHeight;

                v = 1 / dY[resultJ];//更新v
                baseLine = startBaseLine;

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dX[resultI] * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    baseLine = baseLine.Next;
                    startBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }
                startBaseLine = baseLine;
                currentBaseG = startBaseG;
                while (baseLine.EndK < k)
                {
                    baseLine = baseLine.Next;
                    currentBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }

                //这是当前起点轴斜率与参考线的差值
                dG = currentHeight * v - currentBaseG + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (dG >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLinePDE(k, a);
                    startBaseG = currentHeight * v;

                }
                else
                {
                    result[resultI, resultJ] = 0;
                }
                lastK = k;
                lastHeight = currentHeight;


                lonIndex--; resultI--;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; lonIndex >= minXP && lonIndex >= endLonUtmIndex;
                    lonIndex--, resultI--)
                {
                    k = -dX[resultI] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //求出轴斜率差值的增量
                        dGTmp = dG + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (dG >= 0)//当前是通视的
                        {
                            if (dGTmp < 0)//变为不通视
                            {
                                if (dG < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dGTmp >= 0)//变为通视
                            {
                                if (dG > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                    startBaseG = currentHeight * v - a * (k - crossK);
                                }
                            }
                        }
                        dG = dGTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dGTmp = dG + (k - minK) * (a - baseLine.A);
                    if (dG >= 0)//当前是通视的
                    {
                        if (dGTmp < 0)//变为不通视
                        {
                            if (dG < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dGTmp >= 0)//变为通视
                        {
                            if (dG > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                                startBaseG = currentHeight * v - a * (k - crossK);
                            }
                        }
                    }
                    dG = dGTmp;

                    if (dG >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLinePDE(k, a));
                        currentNewLinePart = currentNewLinePart.Next;
                        result[resultI, resultJ] = 1;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }




                    lastK = k;
                    lastHeight = currentHeight;
                }
            }
            #endregion

            u = 1 / (dem.DY * dem.Rdy);//纵向格网间距实地距离的倒数

            #region 左半面
            //以下算法认为横向间隔和纵向间隔相同
            //右半圆:从北往南，从东往西算
            bool isCenterInRightTop = dem.DY * centerGridLon + dem.DX * centerGridLat >= dem.DX * dem.DY;
            startIndexAdjust = isCenterInRightTop ? +1 : 0;
            endIndexAdjust = isCenterInRightBottom ? -1 : 0;

            startLatUtmIndex = centerYP + 1 + startIndexAdjust;
            startResultJIndex = yCount + startIndexAdjust;
            endLatUtmIndex = centerYP + endIndexAdjust;

            lonIndex = centerXP;
            latIndex = startLatUtmIndex;
            resultI = xCount - 1;
            resultJ = startResultJIndex;
            lastHeight = dem.Height[lonIndex, latIndex + 1] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;

            v = -1 / dX[resultI];
            //构造左半边初始参考线
            for (; latIndex >= endLatUtmIndex; latIndex--, resultJ--)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dY[resultJ] * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLinePDE(k, a);
                    baseLine = startBaseLine;
                    startBaseG = currentHeight * v;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLinePDE(k, a));
                    baseLine = baseLine.Next;//前移一步
                }
                result[resultI, resultJ] = 1;
                lastHeight = currentHeight;
            }
            //右边逐层计算参考线与点的显隐性
            lonIndex--;
            resultI--;
            startLatUtmIndex++;
            startResultJIndex++;
            endLatUtmIndex--;
            int maxJIndex = 2 * yCount - 1;

            for (; lonIndex >= minXP;
                lonIndex--, resultI--, startLatUtmIndex++, startResultJIndex++, endLatUtmIndex--)//从左边第二列开始
            {
                if (startResultJIndex > maxJIndex)//保证不超出
                {
                    startLatUtmIndex--;
                    startResultJIndex = maxJIndex;
                }
                resultJ = startResultJIndex;
                latIndex = startLatUtmIndex;
                lastHeight = dem.Height[lonIndex, latIndex + 1] - seeHeight;


                v = -1 / dX[resultI];//更新，此处y应当时原值的负数
                baseLine = startBaseLine;

                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = -dY[resultJ] * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    //Console.WriteLine(21);
                    baseLine = baseLine.Next;
                    startBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }
                startBaseLine = baseLine;
                currentBaseG = startBaseG;
                while (baseLine.EndK < k)
                {
                    //Console.WriteLine(22);
                    baseLine = baseLine.Next;
                    currentBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }

                //这是当前起点轴斜率与参考线的差值
                dG = currentHeight * v - currentBaseG + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (dG >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLinePDE(k, a);
                    startBaseG = currentHeight * v;

                    result[resultI, resultJ] = 1;
                }
                else
                {
                    result[resultI, resultJ] = 0;
                }
                lastK = k;
                lastHeight = currentHeight;

                latIndex--; resultJ--;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; latIndex >= minYP && latIndex >= endLatUtmIndex;
                    latIndex--, resultJ--)
                {
                    k = -dY[resultJ] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //Console.WriteLine(23);
                        //求出轴斜率差值的增量
                        dGTmp = dG + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (dG >= 0)//当前是通视的
                        {
                            if (dGTmp < 0)//变为不通视
                            {
                                if (dG < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dGTmp >= 0)//变为通视
                            {
                                if (dG > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                    startBaseG = currentHeight * v - a * (k - crossK);
                                }
                            }
                        }
                        dG = dGTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dGTmp = dG + (k - minK) * (a - baseLine.A);
                    if (dG >= 0)//当前是通视的
                    {
                        if (dGTmp < 0)//变为不通视
                        {
                            if (dG < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dGTmp >= 0)//变为通视
                        {
                            if (dG > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                                startBaseG = currentHeight * v - a * (k - crossK);
                            }
                        }
                    }
                    dG = dGTmp;

                    if (dG >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLinePDE(k, a));
                        currentNewLinePart = currentNewLinePart.Next;
                        result[resultI, resultJ] = 1;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }


                    lastK = k;
                    lastHeight = currentHeight;
                }
            }
            #endregion

            u = 1 / (dem.DX * dem.Rdx);

            #region 下半面 用于清查
            //以下算法认为横向间隔和纵向间隔相同
            //上半圆:从南往北，从东往西算
            startIndexAdjust = isCenterInLeftTop ? -1 : 0;
            endIndexAdjust = isCenterInRightTop ? 1 : 0;
            
            startLonUtmIndex = centerXP + startIndexAdjust;
            startResultIIndex = xCount - 1 + startIndexAdjust;
            endLonUtmIndex = centerXP + 1 + endIndexAdjust;


            lonIndex = startLonUtmIndex;
            latIndex = centerYP;
            resultI = startResultIIndex;
            resultJ = yCount - 1;
            lastHeight = dem.Height[lonIndex - 1, latIndex] - seeHeight;
            currentHeight = 0;
            startBaseLine = null;

            v = -1 / dY[resultJ];
            //构造初始参考线
            for (; lonIndex <= endLonUtmIndex; lonIndex++, resultI++)
            {
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dX[resultI] * v;
                a = u * (currentHeight - lastHeight);

                if (startBaseLine == null)
                {
                    startBaseLine = new LinkedLinePDE(k, a);
                    baseLine = startBaseLine;
                    startBaseG = currentHeight * v;
                }
                else
                {
                    baseLine.LinkForword(new LinkedLinePDE(k, a));
                    baseLine = baseLine.Next;//前移一步
                }
                lastHeight = currentHeight;
            }

            //上边逐层计算参考线与点的显隐性
            startLonUtmIndex--;
            startResultIIndex--;
            endLonUtmIndex++;
            latIndex--;
            resultJ--;
            for (; latIndex >= minYP;
                latIndex--, resultJ--, startLonUtmIndex--, startResultIIndex--, endLonUtmIndex++)//从右边第二列开始
            {

                if (startResultIIndex < 0)//保证不超出
                {
                    startLonUtmIndex++;
                    startResultIIndex = 0;
                }
                resultI = startResultIIndex;
                lonIndex = startLonUtmIndex;
                lastHeight = dem.Height[lonIndex - 1, latIndex] - seeHeight;

                v = -1 / dY[resultJ];//更新，此处y应当时原值的负数
                baseLine = startBaseLine;


                /////////////////判断起点的显隐性///////////////////
                //起点方向
                currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                k = dX[resultI] * v;
                a = u * (currentHeight - lastHeight);

                while (baseLine.Next.EndK < k)
                {
                    baseLine = baseLine.Next;
                    startBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }
                startBaseLine = baseLine;
                currentBaseG = startBaseG;
                while (baseLine.EndK < k)
                {
                    baseLine = baseLine.Next;
                    currentBaseG += (baseLine.EndK - baseLine.Pre.EndK) * baseLine.A;
                }

                //这是当前起点轴斜率与参考线的差值
                dG = currentHeight * v - currentBaseG + baseLine.A * (baseLine.EndK - k);

                //遮挡判定式
                if (dG >= 0)
                {
                    //建立新线
                    startBaseLine = currentNewLinePart = new LinkedLinePDE(k, a);
                    startBaseG = currentHeight * v;

                }
                else
                {
                    result[resultI, resultJ] = 0;
                }

                lastK = k;
                lastHeight = currentHeight;

                lonIndex++; resultI++;
                ///////////////////////判断后续点的显隐性//////////////////
                for (; lonIndex <= maxXP && lonIndex <= endLonUtmIndex;
                    lonIndex++, resultI++)
                {
                    k = dX[resultI] * v;

                    currentHeight = dem.Height[lonIndex, latIndex] - seeHeight;
                    a = u * (currentHeight - lastHeight);
                    //至此可组成新的OS曲线

                    minK = lastK;
                    while (baseLine.EndK < k)
                    {
                        //求出轴斜率差值的增量
                        dGTmp = dG + (baseLine.EndK - minK) * (a - baseLine.A);
                        if (dG >= 0)//当前是通视的
                        {
                            if (dGTmp < 0)//变为不通视
                            {
                                if (dG < 5e-15)//d很小时算不准
                                    crossK = baseLine.EndK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                                currentNewLinePart = currentNewLinePart.Next;
                                currentNewLinePart.LinkForword(baseLine);
                            }
                        }
                        else
                        {
                            if (dGTmp >= 0)//变为通视
                            {
                                if (dG > -5e-15)//d很小时算不准
                                    crossK = minK;
                                else
                                    crossK = minK + dG / (baseLine.A - a);

                                currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                                if (baseLine.Pre != null)
                                {
                                    baseLine.Pre.LinkForword(currentNewLinePart);
                                }
                                else
                                {
                                    startBaseLine = currentNewLinePart;
                                    startBaseG = currentHeight * v - a * (k - crossK);
                                }
                            }
                        }
                        dG = dGTmp;
                        minK = baseLine.EndK;
                        baseLine = baseLine.Next;
                    }

                    //求出轴斜率差值的增量
                    dGTmp = dG + (k - minK) * (a - baseLine.A);
                    if (dG >= 0)//当前是通视的
                    {
                        if (dGTmp < 0)//变为不通视
                        {
                            if (dG < 5e-15)//d很小时算不准
                                crossK = k;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart.LinkForword(new LinkedLinePDE(crossK, a));
                            currentNewLinePart = currentNewLinePart.Next;
                            currentNewLinePart.LinkForword(baseLine);
                        }
                    }
                    else
                    {
                        if (dGTmp >= 0)//变为通视
                        {
                            if (dG > -5e-15)//d很小时算不准
                                crossK = minK;
                            else
                                crossK = minK + dG / (baseLine.A - a);

                            currentNewLinePart = new LinkedLinePDE(crossK, baseLine.A);
                            if (baseLine.Pre != null)
                            {
                                baseLine.Pre.LinkForword(currentNewLinePart);
                            }
                            else
                            {
                                startBaseLine = currentNewLinePart;
                                startBaseG = currentHeight * v - a * (k - crossK);
                            }
                        }
                    }
                    dG = dGTmp;

                    if (dG >= 0)//保持可见
                    {
                        currentNewLinePart.LinkForword(new LinkedLinePDE(k, a));
                        currentNewLinePart = currentNewLinePart.Next;
                        result[resultI, resultJ] = 1;
                    }
                    else
                    {
                        result[resultI, resultJ] = 0;
                    }


                    lastK = k;
                    lastHeight = currentHeight;
                }
            }


            #endregion
        }

        #endregion

        #region R3
        /// <summary>
        /// 进行R3通视分析（经纬度）
        /// </summary>
        /// <param name="centerLon">观测点中心的经度</param>
        /// <param name="centerLat">观测点中心的纬度</param>
        /// <param name="toEndPointLon">观测区域边缘的经度</param>
        /// <param name="toEndPointLat">观测区域边缘的纬度</param>
        /// <param name="standH">观测点相对地面高程</param>
        /// <param name="result">通视矩阵</param>
        /// <param name="demMinLon">通视矩阵最小经度</param>
        /// <param name="demMinLat">通视矩阵最小纬度</param>
        /// <param name="perLon">经差</param>
        /// <param name="perLat">纬差</param>
        public void DoAnalysisByR3LonLat(double centerLon, double centerLat,
            double toEndPointLon, double toEndPointLat, double standH,
            out int[,] result, out double demMinLon, out double demMinLat,
            out double perLon, out double perLat) =>
            DoCommomAnalysisLonLat(DoAnalysisByR3, centerLon, centerLat,
            toEndPointLon, toEndPointLat, standH,
            out result, out demMinLon, out demMinLat,
            out perLon, out perLat);

        /// <summary>
        /// 进行R3通视分析（在DEM文件投影的基础上）
        /// </summary>
        /// <param name="centerX">观测点中心的X坐标(UTM的X/经纬度的lon)</param>
        /// <param name="centerY">观测点中心的Y坐标(UTM的Y/经纬度的lat)</param>
        /// <param name="centerH">观测点的地面海拔高程</param>
        /// <param name="toEndPointX">观测区域边缘的X坐标</param>
        /// <param name="toEndPointY">观测区域边缘的Y坐标</param>
        /// <param name="standH">观测点相对地面高度</param>
        /// <param name="result">结果矩阵</param>
        /// <param name="demMinX">结果矩阵的X小值</param>
        /// <param name="demMinY">结果矩阵的Y小值</param>
        /// <param name="perX">结果矩阵横间距</param>
        /// <param name="perY">结果矩阵纵间距</param>
        public void DoAnalysisByR3(double centerX, double centerY, double centerH,
            double toEndPointX, double toEndPointY, double standH, out int[,] result,
            out double demMinX, out double demMinY, out double perX, out double perY)
        {
            GetInitialParam(centerX, centerY, centerH, toEndPointX, toEndPointY, standH,
               out result, out demMinX, out demMinY, out perX, out perY,
               out int xCount, out int yCount, out double seeHeight, out int centerXP,
               out int centerYP, out int minXP, out int maxXP, out int minYP, out int maxYP);
            GetXYD(centerX, centerY, demMinX, demMinY, xCount, yCount, out double[] dX, out double[] dY);

            int width = 2 * xCount;
            int height = 2 * yCount;
            double realDLon = dem.PrealDistance;
            double realDLat = dem.PrealDistance;

            //第一象限
            for (int i = xCount; i < width; i++)
            {
                for (int j = yCount; j < height; j++)
                {

                    double r = Math.Sqrt(dY[j] * dY[j] + dX[i] * dX[i]);
                    double s = (dem.Height[minXP + i, minYP + j] - seeHeight) / r;
                    double maxS = s;

                    int xCount1 = (int)Math.Floor(dX[i] / realDLon);
                    double dpLat = dY[j] / dX[i] * realDLon;
                    double dpL = Math.Sqrt(dpLat * dpLat + realDLon * realDLon);
                    for (int c = 1; c <= xCount1; c++)
                    {
                        double dy = dpLat * c;
                        double length = r - dpL * c;

                        int lonIndex = i - c + minXP;
                        double innerY = dy % realDLat;
                        int LatIndex = j - (int)Math.Floor(dy / realDLat) + minYP;

                        //线性插值
                        double h = innerY * (dem.Height[lonIndex, LatIndex - 1] - dem.Height[lonIndex, LatIndex]) / realDLat + dem.Height[lonIndex, LatIndex] - seeHeight;

                        double currentS = h / length;
                        if (currentS > maxS)
                            maxS = currentS;
                    }

                    int yCount1 = (int)Math.Floor(dY[j] / realDLat);
                    double dpLon = dX[i] / dY[j] * realDLat;
                    dpL = Math.Sqrt(dpLon * dpLon + realDLat * realDLat);
                    for (int c = 1; c <= yCount1; c++)
                    {
                        double dx = dpLon * c;
                        double length = r - dpL * c;

                        int LatIndex = j - c + minYP;// j - (int)Math.Floor(dy / realDLat) + minYP;
                        double innerX = dx % realDLon;
                        int lonIndex = i - (int)Math.Floor(dx / realDLon) + minXP;

                        //线性插值
                        double h = innerX * (dem.Height[lonIndex - 1, LatIndex] - dem.Height[lonIndex, LatIndex]) / realDLon + dem.Height[lonIndex, LatIndex] - seeHeight;

                        double currentS = h / length;
                        if (currentS > maxS)
                            maxS = currentS;
                    }

                    if (s >= maxS)
                        result[i, j] = 1;
                    else
                        result[i, j] = 0;
                }
            }

            //第二象限
            for (int i = 0; i < xCount; i++)
            {
                for (int j = yCount; j < height; j++)
                {
                    double r = Math.Sqrt(dY[j] * dY[j] + dX[i] * dX[i]);
                    double s = (dem.Height[minXP + i, minYP + j] - seeHeight) / r;
                    double maxS = s;

                    int xCount1 = (int)Math.Floor(-dX[i] / realDLon);
                    double dpLat = dY[j] / -dX[i] * realDLon;
                    double dpL = Math.Sqrt(dpLat * dpLat + realDLon * realDLon);
                    for (int c = 1; c <= xCount1; c++)
                    {
                        double dy = dpLat * c;
                        double length = r - dpL * c;

                        int lonIndex = i + c + minXP;
                        double innerY = dy % realDLat;
                        int LatIndex = j - (int)Math.Floor(dy / realDLat) + minYP;

                        //线性插值
                        double h = innerY * (dem.Height[lonIndex, LatIndex - 1] - dem.Height[lonIndex, LatIndex]) / realDLat + dem.Height[lonIndex, LatIndex] - seeHeight;

                        double currentS = h / length;
                        if (currentS > maxS)
                            maxS = currentS;
                    }

                    int yCount1 = (int)Math.Floor(dY[j] / realDLat);
                    double dpLon = -dX[i] / dY[j] * realDLat;
                    dpL = Math.Sqrt(dpLon * dpLon + realDLat * realDLat);
                    for (int c = 1; c <= yCount1; c++)
                    {
                        double dx = dpLon * c;
                        double length = r - dpL * c;

                        int LatIndex = j - c + minYP;// j - (int)Math.Floor(dy / realDLat) + minYP;
                        double innerX = dx % realDLon;
                        int lonIndex = i + (int)Math.Floor(dx / realDLon) + minXP;

                        //线性插值
                        double h = innerX * (dem.Height[lonIndex + 1, LatIndex] - dem.Height[lonIndex, LatIndex]) / realDLon + dem.Height[lonIndex, LatIndex] - seeHeight;

                        double currentS = h / length;
                        if (currentS > maxS)
                            maxS = currentS;
                    }

                    if (s >= maxS)
                        result[i, j] = 1;
                    else
                        result[i, j] = 0;
                }
            }

            //第三象限
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {

                    double r = Math.Sqrt(dY[j] * dY[j] + dX[i] * dX[i]);
                    double s = (dem.Height[minXP + i, minYP + j] - seeHeight) / r;
                    double maxS = s;

                    int xCount1 = (int)Math.Floor(-dX[i] / realDLon);
                    double dpLat = -dY[j] / -dX[i] * realDLon;
                    double dpL = Math.Sqrt(dpLat * dpLat + realDLon * realDLon);
                    for (int c = 1; c <= xCount1; c++)
                    {
                        double dy = dpLat * c;
                        double length = r - dpL * c;

                        int lonIndex = i + c + minXP;
                        double innerY = dy % realDLat;
                        int LatIndex = j + (int)Math.Floor(dy / realDLat) + minYP;

                        //线性插值
                        double h = innerY * (dem.Height[lonIndex, LatIndex + 1] - dem.Height[lonIndex, LatIndex]) / realDLat + dem.Height[lonIndex, LatIndex] - seeHeight;

                        double currentS = h / length;
                        if (currentS > maxS)
                            maxS = currentS;
                    }

                    int yCount1 = (int)Math.Floor(-dY[j] / realDLat);
                    double dpLon = -dX[i] / -dY[j] * realDLat;
                    dpL = Math.Sqrt(dpLon * dpLon + realDLat * realDLat);
                    for (int c = 1; c <= yCount1; c++)
                    {
                        double dx = dpLon * c;
                        double length = r - dpL * c;

                        int LatIndex = j + c + minYP;// j - (int)Math.Floor(dy / realDLat) + minYP;
                        double innerX = dx % realDLon;
                        int lonIndex = i + (int)Math.Floor(dx / realDLon) + minXP;

                        //线性插值
                        double h = innerX * (dem.Height[lonIndex + 1, LatIndex] - dem.Height[lonIndex, LatIndex]) / realDLon + dem.Height[lonIndex, LatIndex] - seeHeight;

                        double currentS = h / length;
                        if (currentS > maxS)
                            maxS = currentS;
                    }

                    if (s >= maxS)
                        result[i, j] = 1;
                    else
                        result[i, j] = 0;
                }
            }

            //第四象限
            for (int i = xCount; i < width; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    double r = Math.Sqrt(dY[j] * dY[j] + dX[i] * dX[i]);
                    double s = (dem.Height[minXP + i, minYP + j] - seeHeight) / r;
                    double maxS = s;

                    int xCount1 = (int)Math.Floor(dX[i] / realDLon);
                    double dpLat = -dY[j] / dX[i] * realDLon;
                    double dpL = Math.Sqrt(dpLat * dpLat + realDLon * realDLon);
                    for (int c = 1; c <= xCount1; c++)
                    {
                        double dy = dpLat * c;
                        double length = r - dpL * c;

                        int lonIndex = i - c + minXP;
                        double innerY = dy % realDLat;
                        int LatIndex = j + (int)Math.Floor(dy / realDLat) + minYP;

                        //线性插值
                        double h = innerY * (dem.Height[lonIndex, LatIndex + 1] - dem.Height[lonIndex, LatIndex]) / realDLat + dem.Height[lonIndex, LatIndex] - seeHeight;

                        double currentS = h / length;
                        if (currentS > maxS)
                            maxS = currentS;
                    }

                    int yCount1 = (int)Math.Floor(-dY[j] / realDLat);
                    double dpLon = dX[i] / -dY[j] * realDLat;
                    dpL = Math.Sqrt(dpLon * dpLon + realDLat * realDLat);
                    for (int c = 1; c <= yCount1; c++)
                    {
                        double dx = dpLon * c;
                        double length = r - dpL * c;

                        int LatIndex = j + c + minYP;// j - (int)Math.Floor(dy / realDLat) + minYP;
                        double innerX = dx % realDLon;
                        int lonIndex = i - (int)Math.Floor(dx / realDLon) + minXP;

                        //线性插值
                        double h = innerX * (dem.Height[lonIndex - 1, LatIndex] - dem.Height[lonIndex, LatIndex]) / realDLon + dem.Height[lonIndex, LatIndex] - seeHeight;

                        double currentS = h / length;
                        if (currentS > maxS)
                            maxS = currentS;
                    }

                    if (s >= maxS)
                        result[i, j] = 1;
                    else
                        result[i, j] = 0;
                }
            }
        }

        #endregion

        #region XDraw
        /// <summary>
        /// 进行XDraw通视分析（经纬度）
        /// </summary>
        /// <param name="centerLon">观测点中心的经度</param>
        /// <param name="centerLat">观测点中心的纬度</param>
        /// <param name="toEndPointLon">观测区域边缘的经度</param>
        /// <param name="toEndPointLat">观测区域边缘的纬度</param>
        /// <param name="standH">观测点相对地面高程</param>
        /// <param name="result">通视矩阵</param>
        /// <param name="demMinLon">通视矩阵最小经度</param>
        /// <param name="demMinLat">通视矩阵最小纬度</param>
        /// <param name="perLon">经差</param>
        /// <param name="perLat">纬差</param>
        public void DoAnalysisByXDrawLonLat(double centerLon, double centerLat,
            double toEndPointLon, double toEndPointLat, double standH,
            out int[,] result, out double demMinLon, out double demMinLat,
            out double perLon, out double perLat) =>
            DoCommomAnalysisLonLat(DoAnalysisByXDraw, centerLon, centerLat,
             toEndPointLon, toEndPointLat, standH,
            out result, out demMinLon, out demMinLat,
            out perLon, out perLat);

        /// <summary>
        /// 进行XDraw通视分析（在DEM文件投影的基础上）
        /// </summary>
        /// <param name="centerX">观测点中心的X坐标(UTM的X/经纬度的lon)</param>
        /// <param name="centerY">观测点中心的Y坐标(UTM的Y/经纬度的lat)</param>
        /// <param name="centerH">观测点的地面海拔高程</param>
        /// <param name="toEndPointX">观测区域边缘的X坐标</param>
        /// <param name="toEndPointY">观测区域边缘的Y坐标</param>
        /// <param name="standH">观测点相对地面高度</param>
        /// <param name="result">结果矩阵</param>
        /// <param name="demMinX">结果矩阵的X小值</param>
        /// <param name="demMinY">结果矩阵的Y小值</param>
        /// <param name="perX">结果矩阵横间距</param>
        /// <param name="perY">结果矩阵纵间距</param>
        public void DoAnalysisByXDraw(double centerX, double centerY, double centerH,
            double toEndPointX, double toEndPointY, double standH, out int[,] result,
            out double demMinX, out double demMinY, out double perX, out double perY)
        {
            GetInitialParam(centerX, centerY, centerH, toEndPointX, toEndPointY, standH,
                out result, out demMinX, out demMinY, out perX, out perY,
                out int xCount, out int yCount, out double seeHeight, out int centerXP,
                out int centerYP, out int minXP, out int maxXP, out int minYP, out int maxYP);

            GetXYD(centerX, centerY, demMinX, demMinY, xCount, yCount, out double[] dX, out double[] dY);

            double[,] CompareHeight = new double[2 * xCount, 2 * yCount];

            result[xCount, yCount] = 1;//直接判为可见
            CompareHeight[xCount, yCount] = dem.Height[xCount + minXP, yCount + minYP] - seeHeight;
            result[xCount - 1, yCount] = 1;//直接判为可见
            CompareHeight[xCount - 1, yCount] = dem.Height[xCount - 1 + minXP, yCount + minYP] - seeHeight;
            result[xCount, yCount - 1] = 1;//直接判为可见
            CompareHeight[xCount, yCount - 1] = dem.Height[xCount + minXP, yCount - 1 + minYP] - seeHeight;
            result[xCount - 1, yCount - 1] = 1;//直接判为可见
            CompareHeight[xCount - 1, yCount - 1] = dem.Height[xCount - 1 + minXP, yCount - 1 + minYP] - seeHeight;

            int width = 2 * xCount;
            int height = 2 * yCount;
            double realDLon = dem.PrealDistance;
            double realDLat = dem.PrealDistance;
            int circleMax = xCount;
            //分以下8个区
            for (int circleCount = 1; circleCount < circleMax; circleCount++)
            {
                int lonIndex = 0;
                int latIndex = 0;

                //1:x>0,y>0,    y=rlat,   y++,    x>=y
                int xp = xCount + circleCount;
                int yp = yCount;
                lonIndex = xp + minXP;
                for (; xp > yp; yp++)
                {
                    latIndex = yp + minYP;
                    double innerY = dY[yp] / dX[xp] * realDLon;
                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double Hcompare = CompareHeight[xp - 1, yp] - innerY * (CompareHeight[xp - 1, yp] - CompareHeight[xp - 1, yp - 1]) / realDLat;

                    double Ltarget = Math.Sqrt(dY[yp] * dY[yp] + dX[xp] * dX[xp]);
                    double Lcompare = dX[xp - 1] * Ltarget / dX[xp];

                    if (Hcompare / Lcompare > Htarget / Ltarget)
                    {
                        CompareHeight[xp, yp] = Hcompare / Lcompare * Ltarget;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //2:x>0,y>0,    x=rlon,   x++,    x<y
                xp = xCount;
                yp = yCount + circleCount;
                latIndex = yp + minYP;
                for (; xp <= yp; xp++)
                {
                    lonIndex = xp + minXP;
                    double innerX = dX[xp] / dY[yp] * realDLat;
                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double Hcompare = CompareHeight[xp, yp - 1] - innerX * (CompareHeight[xp, yp - 1] - CompareHeight[xp - 1, yp - 1]) / realDLon;

                    double Ltarget = Math.Sqrt(dY[yp] * dY[yp] + dX[xp] * dX[xp]);
                    double Lcompare = dY[yp - 1] * Ltarget / dY[yp];

                    if (Hcompare / Lcompare > Htarget / Ltarget)
                    {
                        CompareHeight[xp, yp] = Hcompare / Lcompare * Ltarget;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //3:x<0,y>0,    x=rlon-1, x--,    -x<=y
                xp = xCount - 1;
                yp = yCount + circleCount;
                latIndex = yp + minYP;
                for (; xp + yp > 2 * yCount - 1; xp--)
                {
                    lonIndex = xp + minXP;

                    double innerX = -dX[xp] / dY[yp] * realDLat;
                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double Hcompare = CompareHeight[xp, yp - 1] - innerX * (CompareHeight[xp, yp - 1] - CompareHeight[xp + 1, yp - 1]) / realDLon;

                    double Ltarget = Math.Sqrt(dY[yp] * dY[yp] + dX[xp] * dX[xp]);
                    double Lcompare = dY[yp - 1] * Ltarget / dY[yp];

                    if (Hcompare / Lcompare > Htarget / Ltarget)
                    {
                        CompareHeight[xp, yp] = Hcompare / Lcompare * Ltarget;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //4:x<0,y>0,    y=rlat,   y++,    -x>y
                xp = (xCount - 1) - circleCount;
                yp = yCount;
                lonIndex = xp + minXP;
                for (; xp + yp <= 2 * yCount - 1; yp++)
                {
                    latIndex = yp + minYP;

                    double innerY = dY[yp] / -dX[xp] * realDLon;
                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double Hcompare = CompareHeight[xp + 1, yp] - innerY * (CompareHeight[xp + 1, yp] - CompareHeight[xp + 1, yp - 1]) / realDLat;

                    double Ltarget = Math.Sqrt(dY[yp] * dY[yp] + dX[xp] * dX[xp]);
                    double Lcompare = dX[xp + 1] * Ltarget / dX[xp];

                    if (Hcompare / Lcompare > Htarget / Ltarget)
                    {
                        CompareHeight[xp, yp] = Hcompare / Lcompare * Ltarget;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //5:x<0,y<0,    y=rlat-1, y--,    -x>=-y
                xp = (xCount - 1) - circleCount;
                yp = yCount - 1;
                lonIndex = xp + minXP;
                for (; xp < yp; yp--)
                {
                    latIndex = yp + minYP;

                    double innerY = -dY[yp] / -dX[xp] * realDLon;
                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double Hcompare = CompareHeight[xp + 1, yp] - innerY * (CompareHeight[xp + 1, yp] - CompareHeight[xp + 1, yp + 1]) / realDLat;

                    double Ltarget = Math.Sqrt(dY[yp] * dY[yp] + dX[xp] * dX[xp]);
                    double Lcompare = dX[xp + 1] * Ltarget / dX[xp];

                    if (Hcompare / Lcompare > Htarget / Ltarget)
                    {
                        CompareHeight[xp, yp] = Hcompare / Lcompare * Ltarget;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //6:x<0,y<0,    x=rlon-1, x--,    -x<-y
                xp = xCount - 1;
                yp = (yCount - 1) - circleCount;
                latIndex = yp + minYP;
                for (; xp >= yp; xp--)
                {
                    lonIndex = xp + minXP;

                    double innerX = -dX[xp] / -dY[yp] * realDLat;
                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double Hcompare = CompareHeight[xp, yp + 1] - innerX * (CompareHeight[xp, yp + 1] - CompareHeight[xp + 1, yp + 1]) / realDLon;

                    double Ltarget = Math.Sqrt(dY[yp] * dY[yp] + dX[xp] * dX[xp]);
                    double Lcompare = dY[yp + 1] * Ltarget / dY[yp];

                    if (Hcompare / Lcompare > Htarget / Ltarget)
                    {
                        CompareHeight[xp, yp] = Hcompare / Lcompare * Ltarget;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //7:x>0,y<0,    x=rlon,   x++,    x<=-y
                xp = xCount;
                yp = (yCount - 1) - circleCount;
                latIndex = yp + minYP;
                for (; xp + yp < 2 * yCount - 1; xp++)
                {
                    lonIndex = xp + minXP;
                    double innerX = dX[xp] / -dY[yp] * realDLat;
                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double Hcompare = CompareHeight[xp, yp + 1] - innerX * (CompareHeight[xp, yp + 1] - CompareHeight[xp - 1, yp + 1]) / realDLon;

                    double Ltarget = Math.Sqrt(dY[yp] * dY[yp] + dX[xp] * dX[xp]);
                    double Lcompare = dY[yp + 1] * Ltarget / dY[yp];

                    if (Hcompare / Lcompare > Htarget / Ltarget)
                    {
                        CompareHeight[xp, yp] = Hcompare / Lcompare * Ltarget;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //8:x>0,y<0,    y=rlat-1, y--,    x>-y
                xp = xCount + circleCount;
                yp = yCount - 1;
                lonIndex = xp + minXP;
                for (; xp + yp >= 2 * yCount - 1; yp--)
                {
                    latIndex = yp + minYP;
                    double innerY = -dY[yp] / dX[xp] * realDLon;
                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double Hcompare = CompareHeight[xp - 1, yp] - innerY * (CompareHeight[xp - 1, yp] - CompareHeight[xp - 1, yp + 1]) / realDLat;

                    double Ltarget = Math.Sqrt(dY[yp] * dY[yp] + dX[xp] * dX[xp]);
                    double Lcompare = dX[xp - 1] * Ltarget / dX[xp];

                    if (Hcompare / Lcompare > Htarget / Ltarget)
                    {
                        CompareHeight[xp, yp] = Hcompare / Lcompare * Ltarget;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }
            }

        }

        #endregion

        #region 参考面
        /// <summary>
        /// 进行参考面通视分析（经纬度）
        /// </summary>
        /// <param name="centerLon">观测点中心的经度</param>
        /// <param name="centerLat">观测点中心的纬度</param>
        /// <param name="toEndPointLon">观测区域边缘的经度</param>
        /// <param name="toEndPointLat">观测区域边缘的纬度</param>
        /// <param name="standH">观测点相对地面高程</param>
        /// <param name="result">通视矩阵</param>
        /// <param name="demMinLon">通视矩阵最小经度</param>
        /// <param name="demMinLat">通视矩阵最小纬度</param>
        /// <param name="perLon">经差</param>
        /// <param name="perLat">纬差</param>
        public void DoAnalysisByRpLonLat(double centerLon, double centerLat,
            double toEndPointLon, double toEndPointLat, double standH,
            out int[,] result, out double demMinLon, out double demMinLat,
            out double perLon, out double perLat) =>
            DoCommomAnalysisLonLat(DoAnalysisByRp, centerLon, centerLat,
             toEndPointLon, toEndPointLat, standH,
            out result, out demMinLon, out demMinLat,
            out perLon, out perLat);

        /// <summary>
        /// 进行参考面通视分析（在DEM文件投影的基础上）
        /// </summary>
        /// <param name="centerX">观测点中心的X坐标(UTM的X/经纬度的lon)</param>
        /// <param name="centerY">观测点中心的Y坐标(UTM的Y/经纬度的lat)</param>
        /// <param name="centerH">观测点的地面海拔高程</param>
        /// <param name="toEndPointX">观测区域边缘的X坐标</param>
        /// <param name="toEndPointY">观测区域边缘的Y坐标</param>
        /// <param name="standH">观测点相对地面高度</param>
        /// <param name="result">结果矩阵</param>
        /// <param name="demMinX">结果矩阵的X小值</param>
        /// <param name="demMinY">结果矩阵的Y小值</param>
        /// <param name="perX">结果矩阵横间距</param>
        /// <param name="perY">结果矩阵纵间距</param>
        public void DoAnalysisByRp(double centerX, double centerY, double centerH,
            double toEndPointX, double toEndPointY, double standH, out int[,] result,
            out double demMinX, out double demMinY, out double perX, out double perY)
        {
            GetInitialParam(centerX, centerY, centerH, toEndPointX, toEndPointY, standH,
                out result, out demMinX, out demMinY, out perX, out perY,
                out int xCount, out int yCount, out double seeHeight, out int centerXP,
                out int centerYP, out int minXP, out int maxXP, out int minYP, out int maxYP);

            GetXYD(centerX, centerY, demMinX, demMinY, xCount, yCount, out double[] dX, out double[] dY);

            double[,] CompareHeight = new double[2 * xCount, 2 * yCount];


            result[xCount, yCount] = 1;//直接判为可见
            CompareHeight[xCount, yCount] = dem.Height[xCount + minXP, yCount + minYP] - seeHeight;
            result[xCount - 1, yCount] = 1;//直接判为可见
            CompareHeight[xCount - 1, yCount] = dem.Height[xCount - 1 + minXP, yCount + minYP] - seeHeight;
            result[xCount, yCount - 1] = 1;//直接判为可见
            CompareHeight[xCount, yCount - 1] = dem.Height[xCount + minXP, yCount - 1 + minYP] - seeHeight;
            result[xCount - 1, yCount - 1] = 1;//直接判为可见
            CompareHeight[xCount - 1, yCount - 1] = dem.Height[xCount - 1 + minXP, yCount - 1 + minYP] - seeHeight;

            int width = 2 * xCount;
            int height = 2 * yCount;
            double realDLon = dem.PrealDistance;
            double realDLat = dem.PrealDistance;
            int circleMax = xCount;


            //分以下8个区
            for (int circleCount = 1; circleCount < circleMax; circleCount++)
            {
                int lonIndex = 0;
                int latIndex = 0;

                //1:x>0,y>0,    y=rlat,   y++,    x>=y
                int xp = xCount + circleCount;
                int yp = yCount;
                lonIndex = xp + minXP;
                for (; xp > yp; yp++)
                {
                    latIndex = yp + minYP;

                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double x21 = 0, y21 = -realDLat, z21 = CompareHeight[xp - 1, yp - 1] - CompareHeight[xp - 1, yp];
                    double x31 = -dX[xp - 1], y31 = -dY[yp], z31 = -CompareHeight[xp - 1, yp];
                    double Z = CompareHeight[xp - 1, yp] - realDLon * (y21 * z31 - y31 * z21) / (x21 * y31 - x31 * y21);
                    if (Z > Htarget)
                    {
                        CompareHeight[xp, yp] = Z;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }


                //2:x>0,y>0,    x=rlon,   x++,    x<y
                xp = xCount;
                yp = yCount + circleCount;
                latIndex = yp + minYP;
                for (; xp <= yp; xp++)
                {
                    lonIndex = xp + minXP;

                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double x21 = -realDLon, y21 = 0, z21 = CompareHeight[xp - 1, yp - 1] - CompareHeight[xp, yp - 1];
                    double x31 = -dX[xp], y31 = -dY[yp - 1], z31 = -CompareHeight[xp, yp - 1];
                    double Z = CompareHeight[xp, yp - 1] - realDLat * (z21 * x31 - z31 * x21) / (x21 * y31 - x31 * y21);
                    if (Z > Htarget)
                    {
                        CompareHeight[xp, yp] = Z;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //3:x<0,y>0,    x=rlon-1, x--,    -x<=y
                xp = xCount - 1;
                yp = yCount + circleCount;
                latIndex = yp + minYP;
                for (; xp + yp > 2 * yCount - 1; xp--)
                {
                    lonIndex = xp + minXP;

                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double x21 = realDLon, y21 = 0, z21 = CompareHeight[xp + 1, yp - 1] - CompareHeight[xp, yp - 1];
                    double x31 = -dX[xp], y31 = -dY[yp - 1], z31 = -CompareHeight[xp, yp - 1];
                    double Z = CompareHeight[xp, yp - 1] - realDLat * (z21 * x31 - z31 * x21) / (x21 * y31 - x31 * y21);
                    if (Z > Htarget)
                    {
                        CompareHeight[xp, yp] = Z;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }


                //4:x<0,y>0,    y=rlat,   y++,    -x>y
                xp = (xCount - 1) - circleCount;
                yp = yCount;
                lonIndex = xp + minXP;
                for (; xp + yp <= 2 * yCount - 1; yp++)
                {
                    latIndex = yp + minYP;

                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double x21 = 0, y21 = -realDLat, z21 = CompareHeight[xp + 1, yp - 1] - CompareHeight[xp + 1, yp];
                    double x31 = -dX[xp + 1], y31 = -dY[yp], z31 = -CompareHeight[xp + 1, yp];
                    double Z = CompareHeight[xp + 1, yp] + realDLon * (y21 * z31 - y31 * z21) / (x21 * y31 - x31 * y21);
                    if (Z > Htarget)
                    {
                        CompareHeight[xp, yp] = Z;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //5:x<0,y<0,    y=rlat-1, y--,    -x>=-y
                xp = (xCount - 1) - circleCount;
                yp = yCount - 1;
                lonIndex = xp + minXP;
                for (; xp < yp; yp--)
                {
                    latIndex = yp + minYP;

                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double x21 = 0, y21 = realDLat, z21 = CompareHeight[xp + 1, yp + 1] - CompareHeight[xp + 1, yp];
                    double x31 = -dX[xp + 1], y31 = -dY[yp], z31 = -CompareHeight[xp + 1, yp];
                    double Z = CompareHeight[xp + 1, yp] + realDLon * (y21 * z31 - y31 * z21) / (x21 * y31 - x31 * y21);
                    if (Z > Htarget)
                    {
                        CompareHeight[xp, yp] = Z;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //6:x<0,y<0,    x=rlon-1, x--,    -x<-y
                xp = xCount - 1;
                yp = (yCount - 1) - circleCount;
                latIndex = yp + minYP;
                for (; xp >= yp; xp--)
                {
                    lonIndex = xp + minXP;

                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double x21 = realDLon, y21 = 0, z21 = CompareHeight[xp + 1, yp + 1] - CompareHeight[xp, yp + 1];
                    double x31 = -dX[xp], y31 = -dY[yp + 1], z31 = -CompareHeight[xp, yp + 1];
                    double Z = CompareHeight[xp, yp + 1] + realDLat * (z21 * x31 - z31 * x21) / (x21 * y31 - x31 * y21);
                    if (Z > Htarget)
                    {
                        CompareHeight[xp, yp] = Z;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //7:x>0,y<0,    x=rlon,   x++,    x<=-y
                xp = xCount;
                yp = (yCount - 1) - circleCount;
                latIndex = yp + minYP;
                for (; xp + yp < 2 * yCount - 1; xp++)
                {
                    lonIndex = xp + minXP;

                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double x21 = -realDLon, y21 = 0, z21 = CompareHeight[xp - 1, yp + 1] - CompareHeight[xp, yp + 1];
                    double x31 = -dX[xp], y31 = -dY[yp + 1], z31 = -CompareHeight[xp, yp + 1];
                    double Z = CompareHeight[xp, yp + 1] + realDLat * (z21 * x31 - z31 * x21) / (x21 * y31 - x31 * y21);
                    if (Z > Htarget)
                    {
                        CompareHeight[xp, yp] = Z;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }

                //8:x>0,y<0,    y=rlat-1, y--,    x>-y
                xp = xCount + circleCount;
                yp = yCount - 1;
                lonIndex = xp + minXP;
                for (; xp + yp >= 2 * yCount - 1; yp--)
                {
                    latIndex = yp + minYP;

                    double Htarget = dem.Height[lonIndex, latIndex] - seeHeight;
                    double x21 = 0, y21 = realDLat, z21 = CompareHeight[xp - 1, yp + 1] - CompareHeight[xp - 1, yp];
                    double x31 = -dX[xp - 1], y31 = -dY[yp], z31 = -CompareHeight[xp - 1, yp];
                    double Z = CompareHeight[xp - 1, yp] - realDLon * (y21 * z31 - y31 * z21) / (x21 * y31 - x31 * y21);
                    if (Z > Htarget)
                    {
                        CompareHeight[xp, yp] = Z;
                        result[xp, yp] = 0;
                    }
                    else
                    {
                        CompareHeight[xp, yp] = Htarget;
                        result[xp, yp] = 1;
                    }
                }
            }
        }

        #endregion

        #region 辐射状分析

        /// <summary>
        /// 采用传统方法的分析方式
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="centerH"></param>
        /// <param name="toEndPointX"></param>
        /// <param name="toEndPointY"></param>
        /// <param name="seeHeight"></param>
        /// <param name="pCount"></param>
        /// <returns></returns>
        public IEnumerable<AnalysisResultPoint> DoAnalysisByTranditionalLonLat(double centerLon, double centerLat,
            double centerH, double toEndPointX, double toEndPointY, double seeHeight, int pCount)
        {
            //var ps = new double[] { centerLon, centerLat, centerH, toEndPointX, toEndPointY, 0 };
            //dem.Transform2DemProj.TransformPoint(ps);
			//return DoAnalysisByTranditionalUtm(ps[0], ps[1], ps[2], ps[3], ps[4], seeHeight, pCount);
			return DoAnalysisByTranditionalUtm(centerLon, centerLat, centerH, toEndPointX, toEndPointY, seeHeight, pCount);
		}

        public IEnumerable<AnalysisResultPoint> DoAnalysisByTranditionalUtm(double centerX, double centerY,
            double centerH, double toEndPointX, double toEndPointY, double seeHeight, int pointCount)
        {
            //格网间距
            double dr = Math.Min(dem.DY, dem.DX);
            //可计算总距离
            double sum = dr * pointCount;
            //采集区域半径
            var r = (new Point2(centerX, centerY) - new Point2(toEndPointX, toEndPointY)).Length;
            //计算辐射线条数
            int k = (int)(sum / r);
            //可分布角度
            double pr = 2 * Math.PI / k;
            //每条辐射线的点数
            int pPcount = (int)pointCount / k;

            List<AnalysisResultPoint> result = new List<AnalysisResultPoint>();
            var centerPoint = new Point3(centerX, centerY, centerH + seeHeight);
            //对于每个角度进行计算(角度以向东方向为0，顺时针为正)
            for (double angle = 0; angle < 2 * Math.PI; angle += pr)
            {
                foreach (var x in GetTranditionalResult(centerPoint, angle, dr, pPcount))
                    yield return x;
            }
        }

        /// <summary>
        /// 获取某个方向的分析结果
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <returns></returns>
        public IEnumerable<AnalysisResultPoint> GetTranditionalResult(Point3 pStart, double angle, double dr, int count)
        {
            var lonStart = pStart.X;
            var latStart = pStart.Y;
            var h0 = pStart.Z;
            double dX = dr * Math.Sin(angle);
            double dY = dr * Math.Cos(angle);
            double currentK = -double.MaxValue;
            for (int i = 1; i <= count; i++)
            {
                var x = lonStart + dX * i;
                var y = latStart + dY * i;
                var z = dem[x, y];
                var distance = dem.PrealDistance * i;
                var k = (z - h0) / distance;
                if (currentK > k)
                    yield return new AnalysisResultPoint(x, y, z)
                    {
                        Visible = 0,
                    };
                else
                {
                    currentK = k;
                    yield return new AnalysisResultPoint(x, y, z) { Visible = 1 };
                }
            }
        }

        #endregion

        #region 工具方法
        /// <summary>
        /// 通用分析转换方法
        /// </summary>
        /// <param name="analysisOnDEM">具体的分析方法</param>
        /// <param name="centerLon">观测点中心的经度</param>
        /// <param name="centerLat">观测点中心的纬度</param>
        /// <param name="toEndPointLon">观测区域边缘的经度</param>
        /// <param name="toEndPointLat">观测区域边缘的纬度</param>
        /// <param name="standH">观测点相对地面高程</param>
        /// <param name="result">通视矩阵</param>
        /// <param name="demMinLon">通视矩阵最小经度</param>
        /// <param name="demMinLat">通视矩阵最小纬度</param>
        /// <param name="perLon">经差</param>
        /// <param name="perLat">纬差</param>
        public void DoCommomAnalysisLonLat(DoAnalysisOnDEM analysisOnDEM, double centerLon, double centerLat,
            double toEndPointLon, double toEndPointLat, double standH,
            out int[,] result, out double demMinLon, out double demMinLat,
            out double perLon, out double perLat)
        {
			//var ps = new double[] { centerLon, centerLat, GetHeight(centerLon, centerLat), toEndPointLon, toEndPointLat, 0 };
			//dem.Transform2DemProj.TransformPoint(ps);//转换投影方式（转换到DEM文件的投影方式）
			//analysisOnDEM(ps[0], ps[1], ps[2], ps[3], ps[4], standH,
			//    out result, out double demMinX, out double demMinY, out double perX, out double perY);
			//ps = new double[] { demMinX, demMinY, 0, demMinX + perX, demMinY + perY, 0 };
			//dem.Transform2LonLatProj.TransformPoint(ps);
			//demMinLon = ps[0];
			//demMinLat = ps[1];
			//perLon = ps[3] - ps[0];
			//perLat = ps[4] - ps[1];

			analysisOnDEM(centerLon, centerLat, GetHeight(centerLon, centerLat), toEndPointLon, toEndPointLat, standH,
				out result, out demMinLon, out demMinLat, out perLon, out perLat);
		}

        /// <summary>
        /// 获取中心点距离各格网线的距离数组(在DEM定义的投影中)
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="demMinX"></param>
        /// <param name="demMinY"></param>
        /// <param name="xCount"></param>
        /// <param name="yCount"></param>
        /// <param name="dX">横轴距离数组</param>
        /// <param name="dY">纵轴距离数组</param>
        private void GetXYD(double centerX, double centerY, double demMinX,
            double demMinY, int xCount, int yCount, out double[] dX, out double[] dY)
        {
            dX = new double[2 * xCount];
            dY = new double[2 * yCount];
            //求相对于中心点的经差纬差
            for (int i = 0; i < 2 * xCount; i++)
            {
                dX[i] = (demMinX - centerX + i * dem.DX) * dem.Rdx;//实地距离
            }
            //纬线差
            for (int j = 0; j < 2 * yCount; j++)
            {
                dY[j] = (demMinY - centerY + j * dem.DY) * dem.Rdy;//实地距离
            }
        }

        /// <summary>
        /// 获取各类参数
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="centerH">观测点地面海拔高程</param>
        /// <param name="toEndPointX"></param>
        /// <param name="toEndPointY"></param>
        /// <param name="standH">观测者距离地面的相对高度</param>
        /// <param name="result">用于填充的矩阵</param>
        /// <param name="demMinX">横轴最小值</param>
        /// <param name="demMinY">竖轴最大值</param>
        /// <param name="perX">横间距</param>
        /// <param name="perY">纵间距</param>
        /// <param name="xCount">计算的横格网数</param>
        /// <param name="yCount">计算的纵格网数</param>
        /// <param name="seeheight">观测视点的海拔高度</param>
        /// <param name="centerXP">观测点在第几个格网里</param>
        /// <param name="centerYP">观测点在第几个格网里</param>
        /// <param name="minXP">格网边界</param>
        /// <param name="maxXP">格网边界</param>
        /// <param name="minYP">格网边界</param>
        /// <param name="maxYP">格网边界</param>
        private void GetInitialParam(double centerX, double centerY, double centerH, double toEndPointX, double toEndPointY,
            double standH, out int[,] result, out double demMinX, out double demMinY, out double perX, out double perY,
            out int xCount, out int yCount, out double seeheight, out int centerXP, out int centerYP, out int minXP,
            out int maxXP, out int minYP, out int maxYP)
        {
            perX = dem.DX;
            perY = dem.DY;

            //获得半径
            var r = (new Point2(centerX, centerY) - new Point2(toEndPointX, toEndPointY)).Length;
            var rDistance = r * dem.Rdx;//半径实地距离
            var rDistance2 = rDistance * rDistance;//半径实地距离平方

            //需要的格网数量
            xCount = (int)(r / dem.DX);
            yCount = (int)(r / dem.DY);
            seeheight = centerH + standH;
            //求站立点的格网位置
            dem.GetPointLocation(centerX, centerY, out centerXP, out centerYP, out double innerLon, out double innerLat);
            minXP = centerXP - (xCount - 1);
            maxXP = centerXP + xCount;
            minYP = centerYP - (yCount - 1);
            maxYP = centerYP + yCount;
            if (minXP < 0 || minYP < 0 || maxXP >= dem.XSize || maxYP >= dem.YSize) throw new Exception("数据范围不在求解区域");

            demMinX = centerX - innerLon - (xCount - 1) * dem.DX;//第一个格网点的经度
            demMinY = centerY - innerLat - (yCount - 1) * dem.DY;//第一个格网点的纬度

            //结果矩阵，1代表不遮挡，0代表遮挡，默认为0
            result = new int[2 * xCount, 2 * yCount];
            for (int i = 0; i < 2 * xCount; i++)
                for (int j = 0; j < 2 * yCount; j++)
                    result[i, j] = 0;
        }

        /// <summary>
        /// 横竖都反转
        /// </summary>
        /// <param name="src"></param>
        /// <param name="result"></param>
        public void ReverseLL<T>(T[,] src, out T[,] result)
        {
            var latPCount = src.GetLength(1);
            var lonPCount = src.GetLength(0);
            result = new T[lonPCount, latPCount];
            for (int j = 0; j < latPCount; j++)
            {
                for (int i = 0; i < lonPCount; i++)
                {
                    result[lonPCount - 1 - i, latPCount - 1 - j] = src[i, j];
                }
            }
        }

        /// <summary>
        /// 反转竖坐标
        /// </summary>
        public void ReverseLat<T>(T[,] src, out T[,] result)
        {
            var latPCount = src.GetLength(1);
            var lonPCount = src.GetLength(0);
            result = new T[lonPCount, latPCount];
            for (int j = 0; j < latPCount; j++)
            {
                for (int i = 0; i < lonPCount; i++)
                {
                    result[i, latPCount - 1 - j] = src[i, j];
                }
            }
        }

        /// <summary>
        /// 反转竖坐标
        /// </summary>
        public void ReverseLon<T>(T[,] src, out T[,] result)
        {
            var latPCount = src.GetLength(1);
            var lonPCount = src.GetLength(0);
            result = new T[lonPCount, latPCount];
            for (int i = 0; i < lonPCount; i++)
            {
                for (int j = 0; j < latPCount; j++)
                {
                    result[lonPCount - 1 - i, j] = src[i, j];
                }
            }
        }

        /// <summary>
        /// 内插出高程
        /// </summary>
        /// <param name="centerLon"></param>
        /// <param name="centerLat"></param>
        /// <returns></returns>
        public double GetHeight(double centerLon, double centerLat)
        {
            //var ps = new double[] { centerLon, centerLat };
            //dem.Transform2DemProj.TransformPoint(ps);
            //dem.GetCoordinateRowCloumn(ps[0], ps[1], out double pR, out double pC);
            //return dem[pR, pC];

            dem.GetCoordinateRowCloumn(centerLon, centerLat, out double pR, out double pC);
            return dem[pR, pC];
        }

        #endregion
    }

    /// <summary>
    /// 通视结果点
    /// </summary>
    public struct AnalysisResultPoint
    {
        public AnalysisResultPoint(double lon, double lat, double h)
        {
            this.Lon = lon;
            Lat = lat;
            H = h;
            Visible = -1;
        }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public double H { get; set; }
        public int Visible { get; set; }
    }
}
