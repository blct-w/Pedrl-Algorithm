using OSGeo.GDAL;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDERLTest
{
    /// <summary>
    /// 代表一个地形数据
    /// </summary>
    [Serializable]
    public class DEM
    {
        public DEM(string filePath, double realDistance)
        {
            this.pRealDistance = realDistance;
            this.filePath = filePath;
            Gdal.AllRegister();
            dataset = Gdal.Open(filePath, Access.GA_Update);
            xSize = dataset.RasterXSize;
            ySize = dataset.RasterYSize;         // 获取栅格数据的长和宽
            int count = dataset.RasterCount;         // 获取栅格数据的点的数量
            demband = dataset.GetRasterBand(1); // 获取第一个band         
            gt = new double[6];
            dataset.GetGeoTransform(gt);             // 获取屏幕坐标转换到实际地理坐标的参数
            demband.GetNoDataValue(out double nodatavalue, out int hasval);    // 获取没有数据的点的值
            double[] databuf = new double[xSize * ySize];
            var x = demband.ReadRaster(0, 0, dataset.RasterXSize, dataset.RasterYSize, databuf, dataset.RasterXSize, dataset.RasterYSize, 0, 0); // 读取数据到缓冲区中


            startX = gt[0];
            dx = gt[1];
            Height = new double[xSize, ySize];
            if (gt[5] > 0)//Y数据是正序排列的
            {
                for (int j = 0; j < ySize; j++)//以左下角为原点
                {
                    for (int i = 0; i < xSize; i++)
                    {
                        Height[i, j] = databuf[j * xSize + i];
                        if (Height[i, j] > maxHeight) maxHeight = Height[i, j];
                        if (Height[i, j] < minHeight) minHeight = Height[i, j];
                    }
                }
                startY = gt[3];
                dy = gt[5];
            }
            else//Y数据是倒序排列的
            {
                for (int j = 0; j < ySize; j++)//以左下角为原点
                {
                    for (int i = 0; i < xSize; i++)
                    {
                        Height[i, j] = databuf[(YSize - 1 - j) * xSize + i];
                        if (Height[i, j] > maxHeight) maxHeight = Height[i, j];
                        if (Height[i, j] > 1 && Height[i, j] < minHeight) minHeight = Height[i, j];
                    }
                }

                startY = gt[3] + ySize * gt[5];
                dy = -gt[5];
            }

            //projStr = dataset.GetProjectionRef();
            ////DEM自己的坐标系统
            //demSpatialReference = new SpatialReference(projStr);
            ////获取地理坐标系统
            //lonLatSpatialReference = demSpatialReference.CloneGeogCS();
            ////构造转换关系
            //Dem2LonLatProj = new CoordinateTransformation(demSpatialReference, lonLatSpatialReference);
            //ll2DemProj = new CoordinateTransformation(lonLatSpatialReference, demSpatialReference);//如果DEM本身就是以经纬度记录的就不会发生任何转换


            if (Math.Abs(gt[1]) < 1)
            {
                rdx = realDistance / Math.Abs(dx);
                rdy = realDistance / Math.Abs(dy);
            }
        }

        /// <summary>
        /// 平滑，拔高
        /// </summary>
        public void Soft(double k, int r)
        {
            double[,] newHeight = new double[xSize, ySize];
            for (int lonP = 0; lonP < xSize; lonP++)
            {
                for (int latP = 0; latP < ySize; latP++)
                {
                    if (lonP < r || lonP > xSize - r - 1 || latP < r || latP > ySize - r - 1)
                    {
                        newHeight[lonP, latP] = (Height[lonP, latP] - minHeight) * k + minHeight;
                    }
                    else
                    {
                        var sum = 0.0;
                        for (int i = lonP - r; i < lonP + r; i++)
                        {
                            for (int j = latP - r; j < latP + r; j++)
                            {
                                sum += Height[i, j];
                            }
                        }
                        newHeight[lonP, latP] = (sum / (4 * r * r) - minHeight) * k + minHeight;
                    }
                }
            }

            double[] databuf = new double[xSize * ySize];
            if (gt[5] < 0)//Y数据是正序排列的
            {
                for (int j = 0; j < ySize; j++)//以左下角为原点
                {
                    for (int i = 0; i < xSize; i++)
                    {
                        databuf[(YSize - 1 - j) * xSize + i] = newHeight[i, j];
                    }
                }
                demband.WriteRaster(0, 0, dataset.RasterXSize, dataset.RasterYSize, databuf, dataset.RasterXSize, dataset.RasterYSize, 0, 0); // 读取数据到缓冲区中
                demband.FlushCache();
                Dataset.FlushCache();
            }

            //var driver = Gdal.GetDriverByName("GTiff");
            //var datasetnew = driver.Create("new.tif", lonSize, latSize, 1, DataType.GDT_Float32,new string[] { });
            //if (datasetnew != null)
            //{
            //    datasetnew.SetGeoTransform(gt);
            //    datasetnew.SetProjection(projStr);
            //    datasetnew.GetRasterBand(1).WriteRaster(0, 0, lonSize, latSize, databuf, lonSize, latSize, 0, 0);
            //    datasetnew.FlushCache();
            //}
        }

        #region 成员
        /// <summary>
        /// 实地网格间距
        /// </summary>
        private readonly double pRealDistance;
        private readonly string filePath;
        private readonly Dataset dataset;
        private readonly string projStr;
        private readonly double[] gt;
        private readonly double minHeight = 20000;
        private readonly double maxHeight;
        private readonly Band demband;
        private readonly int xSize;
        private readonly int ySize;
        private readonly SpatialReference demSpatialReference;
        private readonly SpatialReference lonLatSpatialReference;
        //private readonly CoordinateTransformation Dem2LonLatProj;
        //private readonly CoordinateTransformation ll2DemProj;
        /// <summary>
        /// 左下角原点的X值
        /// </summary>
        private readonly double startX;
        /// <summary>
        /// 左下角原点的Y值
        /// </summary>
        private readonly double startY;
        /// <summary>
        /// 横间隔(如果是经纬度坐标，这里是经纬度差)
        /// </summary>
        private readonly double dx;
        /// <summary>
        /// 横间隔(无论是经纬度还是UTM坐标，乘以这个系数均为实地距离)
        /// </summary>
        private readonly double rdx = 1;
        /// <summary>
        /// 纵间隔
        /// </summary>
        private readonly double dy;
        /// <summary>
        /// 纵间隔(无论是经纬度还是UTM坐标，乘以这个系数均为实地距离)
        /// </summary>
        private readonly double rdy = 1;

        #endregion

        /// <summary>
        /// 获取指定位置的高程
        /// </summary>
        /// <param name="pRow"></param>
        /// <param name="pColumn"></param>
        /// <returns></returns>
        public double this[double pRow, double pColumn]
        {
            get
            {
                var rL = (int)pRow;
                var rR = (int)Math.Ceiling(pRow);
                var cL = (int)pColumn;
                var cR = (int)Math.Ceiling(pColumn);
                return InterpolationHeight(new Point3(rL, cL, Height[rL, cL]),
                    new Point3(rR, cL, Height[rR, cL]),
                    new Point3(rL, cR, Height[rL, cR]),
                    new Point3(rR, cR, Height[rR, cR]),
                    new Point2(pRow, pColumn));
            }
        }


        /// <summary>
        /// 内插高程（用反距离加权平均法）
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        private double InterpolationHeight(Point3 p1, Point3 p2, Point3 p3, Point3 p4, Point2 p)
        {
            //反距离加权平均法
            var rd1 = (p1 - p).Length;
            var rd2 = (p2 - p).Length;
            var rd3 = (p3 - p).Length;
            var rd4 = (p4 - p).Length;
            if (rd1 == 0) return p1.Z;
            if (rd2 == 0) return p2.Z;
            if (rd3 == 0) return p3.Z;
            if (rd4 == 0) return p4.Z;

            return (p1.Z / rd1 + p2.Z / rd2 + p3.Z / rd3 + p4.Z / rd4) / (1 / rd1 + 1 / rd2 + 1 / rd3 + 1 / rd4);
        }

        /// <summary>
        /// 获取指定行列的坐标
        /// </summary>
        /// <param name="pLon"></param>
        /// <param name="pLat"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        public void GetRowCloumnCoordinate(int pLon, int pLat, out double lon, out double lat)
        {
            lon = startX + pLon * dx;
            lat = startY + pLat * dy;
        }

        /// <summary>
        /// 获取指定坐标在DEM中的行列位置
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="lonP">因为坐标不一定刚好落在格网上，所以值是小数</param>
        /// <param name="latP">因为坐标不一定刚好落在格网上，所以值是小数</param>
        public void GetCoordinateRowCloumn(double lon, double lat, out double lonP, out double latP)
        {
            lonP = (lon - StartX) / DX;
            latP = (lat - StartY) / DY;
        }

        /// <summary>
        /// 获取指定坐标所在格网的行列起始值
        /// </summary>
        /// <param name="utmLon"></param>
        /// <param name="utmLat"></param>
        /// <param name="xP"></param>
        /// <param name="yP">考察点的竖轴坐标</param>
        /// <param name="innerLon">考察点在最初格网内的横轴遗项</param>
        /// <param name="innerLat">考察点在最初格网内的竖轴遗项</param>
        public void GetPointLocation(double utmLon, double utmLat, out int xP, out int yP, out double innerLon, out double innerLat)
        {
            //此处需要以后改善
            this.GetCoordinateRowCloumn(utmLon, utmLat, out double xLon, out double yLat);

            xP = (int)xLon;
            yP = (int)yLat;

            innerLon = DX * (xLon - xP);
            innerLat = DY * (yLat - yP);
        }

        /// <summary>
        /// 存储的高程列表
        /// Height[X,Y]的第一位（即Height[0,0]）代表区域左下角的高程，
        /// 在北半球经度越大，X越大，纬度越大，Y越大
        /// </summary>
        public double[,] Height;

        public double DX => dx;

        public double DY => dy;

        public double StartX => startX;

        public double MaxLon => startX + xSize * dx;

        public double StartY => startY;

        public double MaxY => startY + ySize * dy;

        public int XSize => xSize;

        public int YSize => ySize;

        public double[] Gt => gt;

        public Dataset Dataset => dataset;

        public string ProjStr => projStr;

        ///// <summary>
        ///// 从DEM文件定义坐标转为经纬度坐标
        ///// </summary>
        //public CoordinateTransformation Transform2LonLatProj => Dem2LonLatProj;
        ///// <summary>
        ///// 从经纬度坐标转换为UTM坐标
        ///// </summary>
        //public CoordinateTransformation Transform2DemProj => ll2DemProj;

        public double Rdx => rdx;

        public double Rdy => rdy;

        /// <summary>
        /// 实地格网间距
        /// </summary>
        public double PrealDistance => pRealDistance;

        /// <summary>
        /// 获取一部分
        /// </summary>
        /// <param name="startLon"></param>
        /// <param name="endLon"></param>
        /// <param name="startLat"></param>
        /// <param name="endLat"></param>
        /// <returns></returns>
        public void GetPart(ref double startLon, ref double endLon, ref double startLat, ref double endLat,
            out double[,] height, out double dlon, out double dlat, out string projStr)
        {
            if (startLon < StartX || endLon > MaxLon || startLat < StartY || endLat > MaxY)
                throw new Exception("截取区域超过数据范围");

            dlon = DX;
            dlat = DY;
            this.GetCoordinateRowCloumn(startLon, startLat, out double xLon, out double yLat);
            int lonP = (int)Math.Ceiling(xLon);
            int latP = (int)Math.Ceiling(yLat);

            startLon = startLon - (StartX - startLon) % dlon + dlon;
            startLat = startLat - (StartY - startLat) % dlat + dlat;

            int lonCount = (int)Math.Floor((endLon - startLon) / dlon) + 1;
            int latCount = (int)Math.Floor((endLat - startLat) / dlat) + 1;


            height = new double[lonCount, latCount];
            double lon = startLon, lat = startLat;
            for (int i = 0; lon < endLon; i++, lon += dlon)
            {
                lat = startLat;
                for (int j = 0; lat < endLat; j++, lat += dlat)
                {
                    height[i, j] = this.Height[lonP + i, latP + j];
                }
            }
            projStr = ProjStr;
        }

        /// <summary>
        /// 保存自己
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            Save(this, path);
        }

        /// <summary>
        /// 从本地读取DEM
        /// </summary>
        public static DEM ReadDEM(string path)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                fs.Position = 0;
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter deserializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                object newobj = deserializer.Deserialize(fs);
                return newobj as DEM;
            }
        }

        /// <summary>
        /// 序列化到指定位置
        /// </summary>
        /// <param name="dem"></param>
        /// <param name="path"></param>
        public static void Save(DEM dem, string path)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                serializer.Serialize(fs, dem);
            }
        }
    }
}
