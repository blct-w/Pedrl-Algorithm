using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PderlTest
{
    public class DemAnalysisService
    {
        public DemAnalysisService()
        {
            Analysis = new DemAnalysisHandle(DemData);
        }

        private DEM _dem;
        private string filePath;
        /// <summary>
        /// 当前系统的DEM
        /// </summary>
        public DEM DemData
        {
            get
            {
                if (_dem == null)
                {
                    filePath = "../../DEM/ASTGTM2_N41E119_dem.tif";
                    if (System.IO.File.Exists(filePath))
                        _dem = new DEM(filePath, 30);
                    else
                        throw new Exception("没有找到文件");
                }
                return _dem;
            }
        }

        Dictionary<string, DEM> DemDic = new Dictionary<string, DEM>();

        public DemAnalysisHandle Analysis { get => analysis; set => analysis = value; }
        public string FilePath
        {
            get => filePath;
            set
            {
                if (System.IO.File.Exists(value))
                {
                    filePath = value;
                    if (DemDic.ContainsKey(value))
                    {
                        _dem = DemDic[value];
                        Analysis.Dem = _dem;
                    }
                    else
                    {
                        _dem = new DEM(filePath, 30);
                        DemDic[value] = _dem;
                        Analysis.Dem = _dem;
                    }
                }
                else
                    throw new Exception("没有找到文件");
            }
        }
        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
        }

        private DemAnalysisHandle analysis;
    }

    public static class JObjectExtends
    {
        public static T Get<T>(this Newtonsoft.Json.Linq.JObject objectInstance, string propertyName, bool ignore = true)
        {
            if (objectInstance.ContainsKey(propertyName))
            {
                return objectInstance[propertyName].ToObject<T>();
            }
            else if (ignore)
                return default(T);
            else
                throw new ArgumentException("自动设置参数值失败。参数化变量名称" + propertyName + "必须和对象中的属性名称一样。");
        }
    }
}
