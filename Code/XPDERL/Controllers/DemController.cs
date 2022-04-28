using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blct;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;
using System.Text;

namespace PDERLTest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DemController : ControllerBase
	{
		public DemController(DemAnalysisService _demService)
		{
			demAnalysisService = _demService;
		}
		readonly DemAnalysisService demAnalysisService;

		#region 基础服务

		/// <summary>
		/// 列举当前有哪些DEM文件
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult<object> Get()
		{
			return
				new
				{
					Current = System.IO.Path.GetFileNameWithoutExtension(demAnalysisService.FilePath),
					AllFiles = from p in System.IO.Directory.GetFiles("../../DEM/", "*.tif", SearchOption.TopDirectoryOnly)
							   select System.IO.Path.GetFileNameWithoutExtension(p)
				};

		}

		/// <summary>
		/// 设置当前要计算的DEM
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[HttpGet("setdem/{path}")]
		public ActionResult<bool> SetDem(string path)
		{
			demAnalysisService.FilePath = "../../DEM/" + path + ".tif";
			return true;
		}

		/// <summary>
		/// PDERL算法求通视区域(直接调用的默认方法)
		/// </summary>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <param name="toLon"></param>
		/// <param name="toLat"></param>
		/// <param name="dh">距离地面相对高度</param>
		/// <returns></returns>
		[HttpGet("analysis/{lon}/{lat}/{toLon}/{toLat}/{dh}")]
		public ActionResult<object> GetAnalysisDefaultByPderl(double lon, double lat, double toLon, double toLat, double dh)
		{
			var date = DateTime.Now;

			demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
			var PDERLTime = DateTime.Now - date;

			//获取通视率
			double VisibleRate = GetVisibleRate(result_PDERL);

			//可见区域绘制的线
			//var VisibleLines = GetDrawLines(result_PDERL, perX, perY, demMinX, demMinY);
			float dlon = (float)perX * result_PDERL.GetLength(0);
			float dlat = (float)perY * result_PDERL.GetLength(1);
			float startLon = (float)(lon - dlon / 2);
			float startLat = (float)(lat - dlat / 2);

			var VisiblePoints = new
			{
				hierarchy = new float[] {
					startLon,startLat,
					startLon+dlon,startLat,
					startLon+dlon,startLat+dlat,
					startLon,startLat+dlat,
					startLon,startLat,
				},
				values = result_PDERL,
				x = result_PDERL.GetLength(0),
				y = result_PDERL.GetLength(1)
			};

			return new
			{
				VisiblePoints,
				Time = PDERLTime,
				AllCount = result_PDERL.GetLength(0) * result_PDERL.GetLength(1),
			};
		}

		[HttpGet("analysis_pderl/{lon}/{lat}/{toLon}/{toLat}/{dh}")]
		public ActionResult<object> GetAnalysisDefault_Pderl(double lon, double lat, double toLon, double toLat, double dh)
		{
			var date = DateTime.Now;

			demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
			var PDERLTime = DateTime.Now - date;

			//获取通视率
			double VisibleRate = GetVisibleRate(result_PDERL);

			//可见区域绘制的线
			//var VisibleLines = GetDrawLines(result_PDERL, perX, perY, demMinX, demMinY);
			float dlon = (float)perX * result_PDERL.GetLength(0);
			float dlat = (float)perY * result_PDERL.GetLength(1);
			float startLon = (float)(lon - dlon / 2);
			float startLat = (float)(lat - dlat / 2);

			var VisiblePoints = new
			{
				hierarchy = new float[] {
					startLon,startLat,
					startLon+dlon,startLat,
					startLon+dlon,startLat+dlat,
					startLon,startLat+dlat,
					startLon,startLat,
				},
				values = result_PDERL,
				x = result_PDERL.GetLength(0),
				y = result_PDERL.GetLength(1)
			};

			return new
			{
				VisiblePoints,
				Time = PDERLTime,
				AllCount = result_PDERL.GetLength(0) * result_PDERL.GetLength(1),
			};
		}


		[HttpGet("analysis_xpderl/{lon}/{lat}/{toLon}/{toLat}/{dh}")]
		public ActionResult<object> GetAnalysisDefault_xPderl(double lon, double lat, double toLon, double toLat, double dh)
		{
			var date = DateTime.Now;

			demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
			var PDERLTime = DateTime.Now - date;

			//获取通视率
			double VisibleRate = GetVisibleRate(result_PDERL);

			//可见区域绘制的线
			//var VisibleLines = GetDrawLines(result_PDERL, perX, perY, demMinX, demMinY);
			float dlon = (float)perX * result_PDERL.GetLength(0);
			float dlat = (float)perY * result_PDERL.GetLength(1);
			float startLon = (float)(lon - dlon / 2);
			float startLat = (float)(lat - dlat / 2);

			var VisiblePoints = new
			{
				hierarchy = new float[] {
					startLon,startLat,
					startLon+dlon,startLat,
					startLon+dlon,startLat+dlat,
					startLon,startLat+dlat,
					startLon,startLat,
				},
				values = result_PDERL,
				x = result_PDERL.GetLength(0),
				y = result_PDERL.GetLength(1)
			};

			return new
			{
				VisiblePoints,
				Time = PDERLTime,
				AllCount = result_PDERL.GetLength(0) * result_PDERL.GetLength(1),
			};
		}


		#endregion

		#region PDERL自动测试

		[HttpGet("analysis_auto_test")]
		public ActionResult<string> DoAnalysis_AutoTest()
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{
				SetDem("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");

				for (double i = 0; i < 100; i++)
				{
					DoAnalysisGetNeighborErrWithR3(0.00925, 2, 500, "Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
				}

			})).Start();
			return "开始测试";
		}


		/// <summary>
		/// 批量测试精度(与R3对比)
		/// </summary>
		[HttpGet("analysis_auto_test_accuracy")]
		public ActionResult<string> DoAnalysis_AutoTest_Accuracy()
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{
				Random r = new Random();

				double step = 0;

				SetDem("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 10);

				SetDem("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 10);

				SetDem("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 10);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 10);
				for (double h = 25; h < 5001; h += 25)
				{
					SetDem("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(119.1, 41.3 + step, 119.9, 41.1 + step, h, 10);

					SetDem("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(114.1, 34.3 + step, 114.9, 34.1 + step, h, 10);

					SetDem("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, h, 10);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe(097.1, 28.3 + step, 097.9, 28.1 + step, h, 10);
				}

			})).Start();
			return "开始测试";
		}


		/// <summary>
		/// 自动测试邻域错误（一个进程只能有一个测试）
		/// </summary>
		[HttpGet("analysis_auto_test_neighbor_err")]
		public ActionResult<string> DoAnalysis_AutoTest_NeighborErr(int p = 0)
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{
				string demFile = "";
				if (p == 1)
					demFile = ("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
				else if (p == 2)
					demFile = ("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
				else if (p == 3)
					demFile = ("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
				else
					return;

				SetDem(demFile);

				for (double h = 2; h < 54; h += 2)
				{
					DoAnalysisGetNeighborErrWithR3(0.051, h, 500, demFile);
				}


			})).Start();
			return "开始测试";
		}

		/// <summary>
		/// 自动测试PDERL、XDraw、参考面算法在不同高度的速度对比
		/// </summary>
		[HttpGet("analysis_auto_test_time_without_r3")]
		public ActionResult<string> DoAnalysis_AutoTest_TimeWithoutR3()
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{
				double h = 5000;
				//for (double h = 1; h < 5000; h += 1)
				{
					SetDem("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
					DoAnalysisTimeRodomRepeateWithoutR3(0.1, h, 1);

					SetDem("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
					DoAnalysisTimeRodomRepeateWithoutR3(0.1, h, 1);

					SetDem("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
					DoAnalysisTimeRodomRepeateWithoutR3(0.1, h, 1);
				}

			})).Start();
			return "开始测试";
		}

		#endregion

		#region XPDERL自动测试

		/// <summary>
		/// 3.1 Experiment 1: speed
		/// 自动测试PDERL、XDraw、参考面算法在不同高度的速度对比
		/// </summary>
		[HttpGet("x_analysis_auto_test_time_without_r3")]
		public ActionResult<string> X_DoAnalysis_AutoTest_TimeWithoutR3()
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{

				SetDem("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
				DoAnalysisTimeRodomRepeateWithoutR3(0.1, 1, 100);

				SetDem("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
				DoAnalysisTimeRodomRepeateWithoutR3(0.1, 1, 100);

				SetDem("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
				DoAnalysisTimeRodomRepeateWithoutR3(0.1, 1, 100);
				for (double h = 10; h <= 5000; h+=10)
				{
					SetDem("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
					DoAnalysisTimeRodomRepeateWithoutR3(0.1, h, 100);

					SetDem("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
					DoAnalysisTimeRodomRepeateWithoutR3(0.1, h, 100);

					SetDem("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
					DoAnalysisTimeRodomRepeateWithoutR3(0.1, h, 100);
				}

			})).Start();
			return "开始测试,由于测试量巨大，可能需要等待8小时以上，请关注报告的数据量大小，直到不再增长为止";
		}

		/// <summary>
		/// 3.1 Experiment 2: accuracy
		/// 批量测试精度(与PDERL对比)
		/// </summary>
		[HttpGet("x_analysis_auto_test_accuracy")]
		public ActionResult<string> X_DoAnalysis_AutoTest_Accuracy()
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{
				Random r = new Random();

				double step = 0;//用于随机范围

				SetDem("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 100);//在指定范围内随机半径进行测试
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, 1, 100);

				SetDem("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, 1, 100);

				SetDem("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 100);
				step = r.NextDouble() * 0.6;
				DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, 1, 100);
				for (double h = 50; h < 5001; h += 50)
				{
					SetDem("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(119.1, 41.3 + step, 119.9, 41.1 + step, h, 100);

					SetDem("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(114.1, 34.3 + step, 114.9, 34.1 + step, h, 100);

					SetDem("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, h, 100);
					step = r.NextDouble() * 0.6;
					DoAnalysisAllPrecRe_PDERL(097.1, 28.3 + step, 097.9, 28.1 + step, h, 100);
				}

			})).Start();
			return "开始测试,由于测试量巨大，可能需要等待8小时以上，请关注报告的数据量大小，直到不再增长为止";
			
		}


		/// <summary>
		/// 3.3 Experiment 3: aggregation of error points
		/// 自动测试邻域错误（一个进程只能有一个测试）
		/// </summary>
		[HttpGet("x_analysis_auto_test_neighbor_err")]
		public ActionResult<string> X_DoAnalysis_AutoTest_NeighborErr(int p = 0)
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{
				string demFile = "";
				if (p == 1)
					demFile = ("Copernicus_DSM_COG_10_N41_00_E119_00_DEM");
				else if (p == 2)
					demFile = ("Copernicus_DSM_COG_10_N34_00_E114_00_DEM");
				else if (p == 3)
					demFile = ("Copernicus_DSM_COG_10_N28_00_E097_00_DEM");
				else
					return;

				SetDem(demFile);

				for (double h = 3; h < 76; h += 3)
				{
					//Console.WriteLine(demFile);
					DoAnalysisGetNeighborErrWithPDERL(0.1, h, 1000, demFile);
				}

				//进行半径和聚集程度测试

				for (double r = 0.01; r < 0.11; r += 0.01)
				{
					DoAnalysisGetNeighborErrWithPDERL(r, 3, 1000, demFile + "[半径聚集程度测试]");
				}

			})).Start();
			return "开始测试,由于测试量巨大，可能需要等待8小时以上，请关注报告的数据量大小，直到不再增长为止";
		}

		/// <summary>
		/// get the histogram of each dem
		/// copy the result to excel to get a histogram
		/// </summary>
		/// <returns></returns>
		[HttpGet("histogram")]
		public ActionResult<object> GetHistogramValues()
		{

			var AllFiles = from p in System.IO.Directory.GetFiles("../../DEM/", "*.tif", SearchOption.TopDirectoryOnly)
						   select p;

			StringBuilder report = new StringBuilder();
			report.Append("图号\t最低\t最高\t");
			List<string> id = new List<string>();
			Dictionary<string, int> dic = new Dictionary<string, int>();
			for (int i = 0; i < 10000; i += 100)//对于地形10000米已经足够高了
			{
				string si = i + "-" + (i + 100);
				dic[si] = 0;
				id.Add(si);
				report.Append(si);
				report.Append("\t");
			}
			report.Append("\n");
			foreach (var filePath in AllFiles)
			{
				DEM dem = new DEM(filePath, 30);
				double min = dem.Height[0, 0];
				double max = dem.Height[0, 0];

				report.Append(System.IO.Path.GetFileNameWithoutExtension(filePath) + "\t");
				for (int i = 0; i < dem.XSize; i++)
					for (int j = 0; j < dem.YSize; j++)
					{
						dic[id[((int)(dem.Height[i, j] / 100))]]++;

						if(dem.Height[i, j] > max)
						{
							max = dem.Height[i, j];
						}
						if(dem.Height[i, j] < min)
						{
							min = dem.Height[i, j];
						}
					}
				report.Append(min + "\t" + max + "\t");
				foreach (string i in id)
				{
					report.Append(dic[i] + "\t");
					dic[i] = 0;
				}
				report.Append("\n");
			}

			return report.ToString();

		}


		/// <summary>
		/// FAB对比
		/// </summary>
		/// <returns></returns>
		[HttpGet("x_analysis_auto_test_time_without_r3_fab_test")]
		public ActionResult<string> X_DoAnalysis_AutoTest_TimeWithoutR3_Fab()
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{

				SetDem("Copernicus_DSM_COG_10_N34_00_E113_00_DEM");
				DoAnalysisTimeRodomRepeateWithoutR3(0.1, 1, 100);

				SetDem("N34E113_FABDEM_V1-0");
				DoAnalysisTimeRodomRepeateWithoutR3(0.1, 1, 100);

				for (double h = 10; h <= 5000; h += 10)
				{
					SetDem("Copernicus_DSM_COG_10_N34_00_E113_00_DEM");
					DoAnalysisTimeRodomRepeateWithoutR3(0.1, h, 100);

					SetDem("N34E113_FABDEM_V1-0");
					DoAnalysisTimeRodomRepeateWithoutR3(0.1, h, 100);
				}

			})).Start();
			return "开始测试";
		}

		/// <summary>
		/// FAB对比
		/// 批量测试精度(与PDERL对比)
		/// </summary>
		[HttpGet("x_analysis_auto_test_accuracy_fab_test")]
		public ActionResult<string> X_DoAnalysis_AutoTest_Accuracy_FAB()
		{
			new Thread(new ParameterizedThreadStart((ob) =>
			{
				Random r = new Random();


				SetDem("Copernicus_DSM_COG_10_N34_00_E113_00_DEM");
				DoAnalysisAllPrecRe_PDERL(113.526111, 34.878333333, 113.81194444, 34.66916667, 1, 500);//在指定范围内随机半径进行测试

				SetDem("N34E113_FABDEM_V1-0"); 
				DoAnalysisAllPrecRe_PDERL(113.526111, 34.878333333, 113.81194444, 34.66916667, 1, 500);//在指定范围内随机半径进行测试


				for (double h = 50; h < 5001; h += 50)
				{
					SetDem("Copernicus_DSM_COG_10_N34_00_E113_00_DEM");
					DoAnalysisAllPrecRe_PDERL(113.526111, 34.878333333, 113.81194444, 34.66916667, h, 500);//在指定范围内随机半径进行测试

					SetDem("N34E113_FABDEM_V1-0");
					DoAnalysisAllPrecRe_PDERL(113.526111, 34.878333333, 113.81194444, 34.66916667, h, 500);//在指定范围内随机半径进行测试
				}

			})).Start();
			return "开始测试";
		}

		#endregion

		#region 各种算法与R3的单独精度比较

		[HttpGet("analysis_osd/{lon}/{lat}/{toLon}/{toLat}/{dh}")]
		public ActionResult<object> DoAnalysisPDERL(double lon, double lat, double toLon, double toLat, double dh)
		{
			//return DoAnalysis(demAnalysisService.Analysis.DoAnalysisByOsdLL_Refer, lon, lat, toLon, toLat, dh);
			return DoAnalysis(demAnalysisService.Analysis.DoAnalysisByPedrlLonLat, lon, lat, toLon, toLat, dh);
		}

		[HttpGet("analysis_fast_osd/{lon}/{lat}/{toLon}/{toLat}/{dh}")]
		public ActionResult<object> DoAnalysisXPDERL(double lon, double lat, double toLon, double toLat, double dh)
		{
			return DoAnalysis(demAnalysisService.Analysis.DoAnalysisByXPderlLonLat, lon, lat, toLon, toLat, dh);
		}

		[HttpGet("analysis_xdraw/{lon}/{lat}/{toLon}/{toLat}/{dh}")]
		public ActionResult<object> DoAnalysisXDraw(double lon, double lat, double toLon, double toLat, double dh)
		{
			return DoAnalysis(demAnalysisService.Analysis.DoAnalysisByXDrawLonLat, lon, lat, toLon, toLat, dh);
		}

		[HttpGet("analysis_reff/{lon}/{lat}/{toLon}/{toLat}/{dh}")]
		public ActionResult<object> DoAnalysisRp(double lon, double lat, double toLon, double toLat, double dh)
		{
			return DoAnalysis(demAnalysisService.Analysis.DoAnalysisByRpLonLat, lon, lat, toLon, toLat, dh);
		}

		/// <summary>
		/// 指定方法与R3的对比
		/// </summary>
		/// <param name="DoAnalysisLL">指定的算法</param>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <param name="toLon"></param>
		/// <param name="toLat"></param>
		/// <param name="dh"></param>
		/// <returns>
		/// {
		///    VisibleLines,//可见区域绘制的线
		///    Time,//计算时间
		///    R3Time,//R3耗时
		///    AllPointCount,//共计算了多少点量
		///    VisibleRate,//计算区域的通视率
		///    VisibleErrPoints,//将本来可见的判错
		///    UnVisibleErrPoints,//将本来不可见的判错
		///    VisibleErrCount,//可见点判错数量
		///    UnVisibleErrCount,//不可见点判错数量
		///    VisibleErrRate,//可见点错误率
		///    UnVisibleErrRate,//不可见点错误率
		///    ErrRate//整体错误率
		/// }
		/// </returns>
		private object DoAnalysis(DemAnalysisHandle.DoAnalysisLonLat DoAnalysisLL, double lon, double lat, double toLon, double toLat, double dh)
		{
			var date = DateTime.Now;
			//PDERL算法
			DoAnalysisLL(lon, lat, toLon, toLat, dh,
				out int[,] result, out double demMinX, out double demMinY, out double perX, out double perY);
			var Time = DateTime.Now - date;
			date = DateTime.Now;

			//R3算法
			demAnalysisService.Analysis.DoAnalysisByR3LonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_R3, out double utmLeftLon4, out double utmBottomLat4, out double perX4, out double perY4);
			var R3Time = DateTime.Now - date;
			date = DateTime.Now;

			int lonCount = result_R3.GetLength(0);
			double halfLon = lonCount / 2.0;
			int latCount = result_R3.GetLength(1);
			double halfLat = latCount / 2.0;
			int AllPointCount = lonCount * latCount;

			//获取通视率
			double VisibleRate = GetVisibleRate(result_R3);

			//错点数量
			int VisibleErrCount = 0;
			int UnVisibleErrCount = 0;
			List<object> VisibleErrPoints = new List<object>();
			List<object> UnVisibleErrPoints = new List<object>();
			for (int i = 0; i < lonCount; i++)
				for (int j = 0; j < latCount; j++)
				{
					if (result_R3[i, j] == 1 && result[i, j] == 0)//本来能看见的判为看不见
					{
						VisibleErrPoints.Add(new { i, j });
						VisibleErrCount++;
					}
					else if (result_R3[i, j] == 0 && result[i, j] == 1)//本来看不见的判为能看见
					{
						UnVisibleErrPoints.Add(new { i, j });
						UnVisibleErrCount++;
					}
				}
			//错点率
			double ErrRate = (double)(UnVisibleErrCount + VisibleErrCount) / AllPointCount;
			double VisibleErrRate = (double)(VisibleErrCount) / AllPointCount;
			double UnVisibleErrRate = (double)(UnVisibleErrCount) / AllPointCount;

			float dlon = (float)perX * result.GetLength(0);
			float dlat = (float)perY * result.GetLength(1);
			float startLon = (float)(lon - dlon / 2);
			float startLat = (float)(lat - dlat / 2);
			//可见区
			var VisiblePoints = new
			{
				hierarchy = new float[] {
					startLon,startLat,
					startLon+dlon,startLat,
					startLon+dlon,startLat+dlat,
					startLon,startLat+dlat,
					startLon,startLat,
				},
				values = result,
				x = result.GetLength(0),
				y = result.GetLength(1)
			};

			return new
			{
				Time,//计算时间
				R3Time,//R3计算时间
				AllPointCount,//共计算了多少点量
				VisibleRate,//计算区域的通视率
				VisibleErrPoints,//将本来可见的判错
				UnVisibleErrPoints,//将本来不可见的判错
				VisibleErrCount,//可见点判错数量
				UnVisibleErrCount,//不可见点判错数量
				VisibleErrRate,//可见点错误率
				UnVisibleErrRate,//不可见点错误率
				ErrRate,//整体错误率
				VisiblePoints,//可见区域绘制的线
			};
		}
		#endregion

		#region 实时测试，这几个方法不会记录日志，主要是给前端实时测试准备的
		/// <summary>
		/// 通过在当前DEM文件的最大范围进行计算，来比较PDERL、XPDERL、XDraw、参考面算法的速度
		/// </summary>
		/// <returns>
		/// {
		///   AllCount,//总点数
		///   PDERLTime,//PDERL方法时间
		///   XPDERLTime,//快速PDERL方法时间
		///   RefTime,//参考面方法时间
		///   XDrawTime,//XDraw方法时间
		/// }
		/// </returns>
		[HttpGet("CompareMax")]
		public ActionResult<object> GetCompareByLargeAmountPoints()
		{
			var date = DateTime.Now;
			double lon = 119.51, lat = 41.5123, toLon = 119.95, toLat = 41.5, dh = 2;

			demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
			var PDERLTime = DateTime.Now - date;
			date = DateTime.Now;

			demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_XPDERL, out double utmLeftLon2, out double utmBottomLat2, out double perX2, out double perY2);
			var XPDERLTime = DateTime.Now - date;
			date = DateTime.Now;


			demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_RefF, out double utmLeftLon5, out double utmBottomLat5, out double perX5, out double perY5);
			var RefTime = DateTime.Now - date;
			date = DateTime.Now;


			demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_XDraw, out double utmLeftLon3, out double utmBottomLat3, out double perX3, out double perY3);
			var XDrawTime = DateTime.Now - date;
			date = DateTime.Now;

			return new
			{
				AllCount = (int)(result_PDERL.GetLength(0) * result_PDERL.GetLength(1)),
				PDERLTime,
				XPDERLTime,
				RefTime,
				XDrawTime
			};
		}


		/// <summary>
		/// 对比各种算法的耗时、错误点数、错误率等（以R3为对比）
		/// </summary>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <param name="toLon"></param>
		/// <param name="toLat"></param>
		/// <param name="dh"></param>
		/// <returns>
		/// </returns>
		[HttpGet("analysis_all/{lon}/{lat}/{toLon}/{toLat}/{dh}")]
		public ActionResult<object> GetAnalysisAll(double lon, double lat,
			double toLon, double toLat, double dh)
		{
			var date = DateTime.Now;

			#region 计算

			//PDERL算法
			demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
			var PDERLTime = DateTime.Now - date;
			date = DateTime.Now;

			//XPDERL算法
			demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_XPDERL, out double utmLeftLon2, out double utmBottomLat2, out double perX2, out double perY2);
			var XPDERLTime = DateTime.Now - date;
			date = DateTime.Now;

			//参考面算法
			demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_RefF, out double utmLeftLon5, out double utmBottomLat5, out double perX5, out double perY5);
			var RefTime = DateTime.Now - date;
			date = DateTime.Now;

			//XDraw算法
			demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_XDraw, out double utmLeftLon3, out double utmBottomLat3, out double perX3, out double perY3);
			var XDrawTime = DateTime.Now - date;
			date = DateTime.Now;

			//R3算法
			demAnalysisService.Analysis.DoAnalysisByR3LonLat(lon, lat, toLon, toLat, dh,
				out int[,] result_R3, out double utmLeftLon4, out double utmBottomLat4, out double perX4, out double perY4);
			var R3Time = DateTime.Now - date;

			#endregion

			#region 获取通视率、比较错点数量及错误比率

			//获取通视率
			double VisibleRate = GetVisibleRate(result_R3);

			int lonCount = result_PDERL.GetLength(0);
			double halfLon = lonCount / 2.0;
			int latCount = result_PDERL.GetLength(1);
			double halfLat = latCount / 2.0;
			int AllPointCount = lonCount * latCount;

			GetErrRate(result_PDERL, result_R3, lonCount, latCount, AllPointCount, out int PDERLErrPointCount, out double PDERLErrRate);
			GetErrRate(result_XPDERL, result_R3, lonCount, latCount, AllPointCount, out int XPDERLErrPointCount, out double XPDERLErrRate);
			GetErrRate(result_XDraw, result_R3, lonCount, latCount, AllPointCount, out int XDrawErrPointCount, out double XDrawErrRate);
			GetErrRate(result_RefF, result_R3, lonCount, latCount, AllPointCount, out int RefFErrPointCount, out double RefFErrRate);

			#endregion

			return new
			{
				//计算总点数：代表计算量
				AllPointCount,
				//通视率
				VisibleRate,
				//耗时
				R3Time,
				PDERLTime,
				XPDERLTime,
				XDrawTime,
				RefTime,
				//错误点数
				PDERLErrPointCount,
				XPDERLErrPointCount,
				XDrawErrPointCount,
				RefFErrPointCount,
				//错误率
				PDERLErrRate,
				XPDERLErrRate,
				XDrawErrRate,
				RefFErrRate
			};
		}


		/// <summary>
		/// 计算各种算法同时进行等半径范围比较（精度参照R3）
		/// </summary>
		/// <param name="DoAnalysisLL"></param>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <param name="r"></param>
		/// <param name="dh"></param>
		/// <param name="max">最多计算多少个点，大于0有效</param>
		/// <returns></returns>
		[HttpGet("analysis_all_fixed_r/{r}/{dh}/{max}")]
		public object DoAnalysisRodomRepeateAll(double r, double dh, int max)
		{
			Random radom = new Random();
			List<object> items = new List<object>();

			double dlon = demAnalysisService.DemData.DX;
			double dlat = demAnalysisService.DemData.DY;
			var startLon = demAnalysisService.DemData.StartX + r + dlon + 0.00181;//避免正好在格网上
			var startLat = demAnalysisService.DemData.StartY + r + dlat + 0.00181;
			var maxLon = demAnalysisService.DemData.MaxLon - r - dlon - 0.00181;
			var maxLat = demAnalysisService.DemData.MaxY - r - dlat - 0.00181;

			double count = 0;
			for (double lon = startLon; lon < maxLon; lon += dlon)
			{
				for (double lat = startLat; lat < maxLat; lat += dlat)
				{
					if (max > 0 && count > max)
						break;
					else
						count++;

					var toLon = lon + r;
					var toLat = lat;

					var date = DateTime.Now;
					//PDERL算法
					demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
					var PDERLTime = DateTime.Now - date;
					date = DateTime.Now;

					demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XPDERL, out double utmLeftLon2, out double utmBottomLat2, out double perX2, out double perY2);
					var XPDERLTime = DateTime.Now - date;
					date = DateTime.Now;


					demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_RefF, out double utmLeftLon5, out double utmBottomLat5, out double perX5, out double perY5);
					var RefFTime = DateTime.Now - date;
					date = DateTime.Now;


					demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XDraw, out double utmLeftLon3, out double utmBottomLat3, out double perX3, out double perY3);
					var XDrawTime = DateTime.Now - date;
					date = DateTime.Now;

					//R3算法
					demAnalysisService.Analysis.DoAnalysisByR3LonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_R3, out double utmLeftLon4, out double utmBottomLat4, out double perX4, out double perY4);
					var R3Time = DateTime.Now - date;
					date = DateTime.Now;

					int lonCount = result_R3.GetLength(0);
					int latCount = result_R3.GetLength(1);
					int AllPointCount = lonCount * latCount;

					//获取通视率
					double VisibleRate = GetVisibleRate(result_R3);

					getErrCount(result_PDERL, result_R3, lonCount, latCount, AllPointCount,
						out int PDERLVisibleErrCount, out int PDERLUnVisibleErrCount, out double PDERLErrRate, out double PDERLVisibleErrRate, out double PDERLUnVisibleErrRate);
					getErrCount(result_XPDERL, result_R3, lonCount, latCount, AllPointCount,
						out int XPDERLVisibleErrCount, out int XPDERLUnVisibleErrCount, out double XPDERLErrRate, out double XPDERLVisibleErrRate, out double XPDERLUnVisibleErrRate);
					getErrCount(result_RefF, result_R3, lonCount, latCount, AllPointCount,
						out int RefFVisibleErrCount, out int RefFUnVisibleErrCount, out double RefFErrRate, out double RefFVisibleErrRate, out double RefFUnVisibleErrRate);
					getErrCount(result_XDraw, result_R3, lonCount, latCount, AllPointCount,
						out int XDrawVisibleErrCount, out int XDrawUnVisibleErrCount, out double XDrawErrRate, out double XDrawVisibleErrRate, out double XDrawUnVisibleErrRate);

					items.Add(new
					{
						R3Time = R3Time.TotalSeconds,//R3计算时间
						R = lonCount,//半径
						AllPointCount,//共计算了多少点量
						VisibleRate,//计算区域的通视率

						PDERLTime = PDERLTime.TotalSeconds,//计算时间
						PDERLPerPointTime = PDERLTime.TotalSeconds / AllPointCount * 1000000000,//单点耗时（单位纳秒）
						PDERLVisibleErrCount,//可见点判错数量
						PDERLUnVisibleErrCount,//不可见点判错数量
						PDERLVisibleErrRate,//可见点错误率
						PDERLUnVisibleErrRate,//不可见点错误率
						PDERLErrRate,//整体错误率

						XPDERLTime = XPDERLTime.TotalSeconds,//计算时间
						XPDERLPerPointTime = XPDERLTime.TotalSeconds / AllPointCount * 1000000000,//单点耗时（单位纳秒）
						XPDERLVisibleErrCount,//可见点判错数量
						XPDERLUnVisibleErrCount,//不可见点判错数量
						XPDERLVisibleErrRate,//可见点错误率
						XPDERLUnVisibleErrRate,//不可见点错误率
						XPDERLErrRate,//整体错误率

						RefFTime = RefFTime.TotalSeconds,//计算时间
						RefFPerPointTime = RefFTime.TotalSeconds / AllPointCount * 1000000000,//单点耗时（单位纳秒）
						RefFVisibleErrCount,//可见点判错数量
						RefFUnVisibleErrCount,//不可见点判错数量
						RefFVisibleErrRate,//可见点错误率
						RefFUnVisibleErrRate,//不可见点错误率
						RefFErrRate,//整体错误率

						XDrawTime = XDrawTime.TotalSeconds,//计算时间
						XDrawPerPointTime = XDrawTime.TotalSeconds / AllPointCount * 1000000000,//单点耗时（单位纳秒）
						XDrawVisibleErrCount,//可见点判错数量
						XDrawUnVisibleErrCount,//不可见点判错数量
						XDrawVisibleErrRate,//可见点错误率
						XDrawUnVisibleErrRate,//不可见点错误率
						XDrawErrRate,//整体错误率
					});
				}

				if (max > 0 && count > max)
					break;
			}

			return items;
		}
		#endregion

		#region 记录性测试，这几个方法会记录日志，主要是给自动测试准备的，可以组合调用
		/// <summary>
		/// 重复各高度各算法精度速度随机测试（精度参照R3）
		/// </summary>
		/// <param name="count">重复次数</param>
		/// <returns></returns>
		[HttpGet("analysis_osd_r/{minlon}/{maxlat}/{maxLon}/{minLat}/{dh}/{count}")]
		public ActionResult<object> DoAnalysisAllPrecRe(double minLon, double maxLat,
			double maxLon, double minLat, double dh, int count)
		{
			Random radom = new Random();
			List<object> items = new List<object>();
			if (maxLat - minLat < 0.027 || maxLon - minLon < 0.027)
				throw new Exception("运算范围太小");
			//using (var fs = new FileStream("../../RunningLog/各算法与R3算法精度比较_"
			//    + demAnalysisService.FileName + "_" + dh.ToString() + "_[" + minLon + "：" + maxLon + "]_[" + minLat + "：" + maxLat + "]_" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString().Replace(":", "：") + ".csv", FileMode.Create, FileAccess.ReadWrite))
			if (!System.IO.File.Exists("../../RunningLog/各高度各算法精度速度随机测试.csv"))
			{
				if (!Directory.Exists("../../RunningLog"))
					Directory.CreateDirectory("../../RunningLog");
				using (var fs = new FileStream("../../RunningLog/各高度各算法精度速度随机测试.csv", FileMode.Create, FileAccess.ReadWrite))
				using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
				{
					sw.WriteLine("数据文件,位置,计算点量,通视率,视点高度," +
					"最长参考线长度,有效参考线总长度,交叉点总数," +
					"R3耗时,PDERL耗时,XPDERL耗时,XDraw耗时,RefF耗时," +
					"PDERL第一类错误,PDERL第二类错误,PDERL总错误," +
					"PDERL第一类错误率,PDERL第二类错误率,PDERL整体错误率," +
					"XPDERL第一类错误,XPDERL第二类错误,XPDERL总错误," +
					"XPDERL第一类错误率,XPDERL第二类错误率,XPDERL整体错误率," +
					"XDraw第一类错误,XDraw第二类错误,XDraw总错误," +
					"XDraw第一类错误率,XDraw第二类错误率,XDraw整体错误率," +
					"RefF第一类错误,RefF第二类错误,RefF总错误," +
					"RefF第一类错误率,RefF第二类错误率,RefF整体错误率,"
					);
					sw.Flush();
				}
			}
			using (var fs = new FileStream("../../RunningLog/各高度各算法精度速度随机测试.csv", FileMode.Append, FileAccess.Write))
			using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
			{
				for (int k = 0; k < count; k++)
				{
					var lon1 = radom.NextDouble() * (maxLon - minLon) + minLon;
					var lon2 = radom.NextDouble() * (maxLon - minLon) + minLon;
					var rlon = Math.Abs(lon1 - lon2) / 2;
					if (rlon < 0.0027)
					{
						k--;
						continue;
					}
					var lon = (Math.Min(lon1, lon2) + Math.Max(lon1, lon2)) / 2;

					var lat1 = radom.NextDouble() * (maxLat - minLat) + minLat;
					var lat2 = radom.NextDouble() * (maxLat - minLat) + minLat;
					var rlat = Math.Abs(lat1 - lat2) / 2;
					if (rlat < 0.0027)
					{
						k--;
						continue;
					}
					var lat = (Math.Min(lat1, lat2) + Math.Max(lat1, lat2)) / 2;

					double toLon = 0, toLat = 0;

					if (rlat > rlon)
					{
						toLon = lon + rlon;
						toLat = lat;
					}
					else
					{
						toLon = lon;
						toLat = lat + rlat;
					}

					var date = DateTime.Now;

					//PDERL
					demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
					var PDERLTime = DateTime.Now - date;
					date = DateTime.Now;

					//PDERL参照（主要求交叉点数量等信息，为评价地形做辅助工作）
					demAnalysisService.Analysis.DoAnalysisByPderlLonLat_Refer(lon, lat, toLon, toLat, dh,
						out int[,] result1, out demMinX, out demMinY, out perX, out perY);
					var TimeRefer = DateTime.Now - date;
					date = DateTime.Now;

					//XPDERL
					demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XPDERL, out demMinX, out demMinY, out perX, out perY);
					var TimeXPDERL = DateTime.Now - date;
					date = DateTime.Now;

					//XDraw
					demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XDraw, out demMinX, out demMinY, out perX, out perY);
					var TimeXDraw = DateTime.Now - date;
					date = DateTime.Now;

					//Rff
					demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_RefF, out demMinX, out demMinY, out perX, out perY);
					var TimeRefF = DateTime.Now - date;
					date = DateTime.Now;

					//R3算法
					demAnalysisService.Analysis.DoAnalysisByR3LonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_R3, out demMinX, out demMinY, out perX, out perY);
					var R3Time = DateTime.Now - date;
					date = DateTime.Now;

					int lonCount = result_R3.GetLength(0);
					int latCount = result_R3.GetLength(1);
					int AllPointCount = lonCount * latCount;

					//获取通视率
					double VisibleRate = GetVisibleRate(result_R3);

					//错点率
					GetErrRate(result_PDERL, result_R3, lonCount, latCount, AllPointCount,
						out int PDERLVisibleErrCount, out int PDERLUnVisibleErrCount, out int PDERLAllErrCount, out double PDERLVisibleErrRate, out double PDERLUnVisibleErrRate, out double PDERLErrRate);

					GetErrRate(result_XPDERL, result_R3, lonCount, latCount, AllPointCount,
						out int XPDERLVisibleErrCount, out int XPDERLUnVisibleErrCount, out int XPDERLAllErrCount, out double XPDERLVisibleErrRate, out double XPDERLUnVisibleErrRate, out double XPDERLErrRate);

					GetErrRate(result_RefF, result_R3, lonCount, latCount, AllPointCount,
						out int RefFVisibleErrCount, out int RefFUnVisibleErrCount, out int RefFAllErrCount, out double RefFVisibleErrRate, out double RefFUnVisibleErrRate, out double RefFErrRate);
					GetErrRate(result_XDraw, result_R3, lonCount, latCount, AllPointCount,
						out int XDrawVisibleErrCount, out int XDrawUnVisibleErrCount, out int XDrawAllErrCount, out double XDrawVisibleErrRate, out double XDrawUnVisibleErrRate, out double XDrawErrRate);

					string Location = "E" + lon.ToString() + "°，N" + lat.ToString() + "°";
					double PerPointTime = PDERLTime.TotalSeconds / AllPointCount * 1000000000;

					int MaxReferLength = demAnalysisService.Analysis.pde_max_refer_length;
					int AllReferLength = demAnalysisService.Analysis.pde_all_refer_length;
					int CrossPointCount = demAnalysisService.Analysis.pde_cross_point_count;
					items.Add(new
					{
						Location,//坐标
						R3Time = R3Time.TotalSeconds,//R3计算时间
						Time = PDERLTime.TotalSeconds,//计算时间
						PerPointTime,//单点耗时（单位纳秒）
						AllPointCount,//共计算了多少点量
						VisibleRate,//计算区域的通视率
						PDERLVisibleErrCount,//可见点判错数量
						PDERLUnVisibleErrCount,//不可见点判错数量
						PDERLAllErrCount,//总错误点数
						PDERLVisibleErrRate,//可见点错误率
						PDERLUnVisibleErrRate,//不可见点错误率
						PDERLErrRate,//整体错误率
						MaxReferLength,//交叉点数量
						AllReferLength,//交叉点计算数量
						CrossPointCount //交叉点计算失败数量
					});

					sw.WriteLine(demAnalysisService.FileName + "," + Location + "," + AllPointCount + "," + VisibleRate + "," + dh + ","
						+ MaxReferLength + "," + AllReferLength + "," + CrossPointCount + ","
						+ R3Time.TotalSeconds + "," + PDERLTime.TotalSeconds + "," + TimeXPDERL.TotalSeconds + "," + TimeXDraw.TotalSeconds + "," + TimeRefF.TotalSeconds + ","
						+ PDERLVisibleErrCount + "," + PDERLUnVisibleErrCount + "," + PDERLAllErrCount + ","
						+ PDERLVisibleErrRate + "," + PDERLUnVisibleErrRate + "," + PDERLErrRate + ","
						+ XPDERLVisibleErrCount + "," + XPDERLUnVisibleErrCount + "," + XPDERLAllErrCount + ","
						+ XPDERLVisibleErrRate + "," + XPDERLUnVisibleErrRate + "," + XPDERLErrRate + ","
						+ XDrawVisibleErrCount + "," + XDrawUnVisibleErrCount + "," + XDrawAllErrCount + ","
						+ XDrawVisibleErrRate + "," + XDrawUnVisibleErrRate + "," + XDrawErrRate + ","
						+ RefFVisibleErrCount + "," + RefFUnVisibleErrCount + "," + RefFAllErrCount + ","
						+ RefFVisibleErrRate + "," + RefFUnVisibleErrRate + "," + RefFErrRate + ","
						);
				}


			}
			return items;
		}

		/// <summary>
		/// 重复各高度各算法精度速度随机测试（精度参照PDERL）
		/// </summary>
		/// <param name="count">重复次数</param>
		/// <returns></returns>
		[HttpGet("analysis_osd_r/{minlon}/{maxlat}/{maxLon}/{minLat}/{dh}/{count}")]
		public ActionResult<object> DoAnalysisAllPrecRe_PDERL(double minLon, double maxLat,
			double maxLon, double minLat, double dh, int count)
		{
			Random radom = new Random();
			List<object> items = new List<object>();
			if (maxLat - minLat < 0.027 || maxLon - minLon < 0.027)
				throw new Exception("运算范围太小");
			//using (var fs = new FileStream("../../RunningLog/各算法与R3算法精度比较_"
			//    + demAnalysisService.FileName + "_" + dh.ToString() + "_[" + minLon + "：" + maxLon + "]_[" + minLat + "：" + maxLat + "]_" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString().Replace(":", "：") + ".csv", FileMode.Create, FileAccess.ReadWrite))
			if (!System.IO.File.Exists("../../RunningLog/各高度各算法精度速度随机测试.csv"))
			{
				if (!Directory.Exists("../../RunningLog"))
					Directory.CreateDirectory("../../RunningLog");
				using (var fs = new FileStream("../../RunningLog/各高度各算法精度速度随机测试.csv", FileMode.Create, FileAccess.ReadWrite))
				using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
				{
					sw.WriteLine("数据文件,位置,计算点量,通视率,视点高度," +
					"最长参考线长度,有效参考线总长度,交叉点总数," +
					"PDERL耗时,PDERL耗时,XPDERL耗时,XDraw耗时,RefF耗时," +
					"PDERL第一类错误,PDERL第二类错误,PDERL总错误," +
					"PDERL第一类错误率,PDERL第二类错误率,PDERL整体错误率," +
					"XPDERL第一类错误,XPDERL第二类错误,XPDERL总错误," +
					"XPDERL第一类错误率,XPDERL第二类错误率,XPDERL整体错误率," +
					"XDraw第一类错误,XDraw第二类错误,XDraw总错误," +
					"XDraw第一类错误率,XDraw第二类错误率,XDraw整体错误率," +
					"RefF第一类错误,RefF第二类错误,RefF总错误," +
					"RefF第一类错误率,RefF第二类错误率,RefF整体错误率,"
					);
					sw.Flush();
				}
			}
			using (var fs = new FileStream("../../RunningLog/各高度各算法精度速度随机测试.csv", FileMode.Append, FileAccess.Write))
			using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
			{
				for (int k = 0; k < count; k++)
				{
					var lon1 = radom.NextDouble() * (maxLon - minLon) + minLon;
					var lon2 = radom.NextDouble() * (maxLon - minLon) + minLon;
					var rlon = Math.Abs(lon1 - lon2) / 2;
					if (rlon < 0.0027)
					{
						k--;
						continue;
					}
					var lon = (Math.Min(lon1, lon2) + Math.Max(lon1, lon2)) / 2;

					var lat1 = radom.NextDouble() * (maxLat - minLat) + minLat;
					var lat2 = radom.NextDouble() * (maxLat - minLat) + minLat;
					var rlat = Math.Abs(lat1 - lat2) / 2;
					if (rlat < 0.0027)
					{
						k--;
						continue;
					}
					var lat = (Math.Min(lat1, lat2) + Math.Max(lat1, lat2)) / 2;

					double toLon = 0, toLat = 0;

					if (rlat > rlon)
					{
						toLon = lon + rlon;
						toLat = lat;
					}
					else
					{
						toLon = lon;
						toLat = lat + rlat;
					}

					var date = DateTime.Now;

					//PDERL
					demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
					var PDERLTime = DateTime.Now - date;
					date = DateTime.Now;

					//PDERL参照（主要求交叉点数量等信息，为评价地形做辅助工作）
					demAnalysisService.Analysis.DoAnalysisByPderlLonLat_Refer(lon, lat, toLon, toLat, dh,
						out int[,] result1, out demMinX, out demMinY, out perX, out perY);
					var TimeRefer = DateTime.Now - date;
					date = DateTime.Now;

					//XPDERL
					demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XPDERL, out demMinX, out demMinY, out perX, out perY);
					var TimeXPDERL = DateTime.Now - date;
					date = DateTime.Now;

					//XDraw
					demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XDraw, out demMinX, out demMinY, out perX, out perY);
					var TimeXDraw = DateTime.Now - date;
					date = DateTime.Now;

					//Rff
					demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_RefF, out demMinX, out demMinY, out perX, out perY);
					var TimeRefF = DateTime.Now - date;
					date = DateTime.Now;

					////R3算法
					//demAnalysisService.Analysis.DoAnalysisByR3LonLat(lon, lat, toLon, toLat, dh,
					//    out int[,] result_R3, out demMinX, out demMinY, out perX, out perY);
					//var R3Time = DateTime.Now - date;
					//date = DateTime.Now;

					int lonCount = result_PDERL.GetLength(0);
					int latCount = result_PDERL.GetLength(1);
					int AllPointCount = lonCount * latCount;

					//获取通视率
					double VisibleRate = GetVisibleRate(result_PDERL);

					//错点率
					GetErrRate(result_PDERL, result_PDERL, lonCount, latCount, AllPointCount,
						out int PDERLVisibleErrCount, out int PDERLUnVisibleErrCount, out int PDERLAllErrCount, out double PDERLVisibleErrRate, out double PDERLUnVisibleErrRate, out double PDERLErrRate);

					GetErrRate(result_XPDERL, result_PDERL, lonCount, latCount, AllPointCount,
						out int XPDERLVisibleErrCount, out int XPDERLUnVisibleErrCount, out int XPDERLAllErrCount, out double XPDERLVisibleErrRate, out double XPDERLUnVisibleErrRate, out double XPDERLErrRate);

					GetErrRate(result_RefF, result_PDERL, lonCount, latCount, AllPointCount,
						out int RefFVisibleErrCount, out int RefFUnVisibleErrCount, out int RefFAllErrCount, out double RefFVisibleErrRate, out double RefFUnVisibleErrRate, out double RefFErrRate);
					GetErrRate(result_XDraw, result_PDERL, lonCount, latCount, AllPointCount,
						out int XDrawVisibleErrCount, out int XDrawUnVisibleErrCount, out int XDrawAllErrCount, out double XDrawVisibleErrRate, out double XDrawUnVisibleErrRate, out double XDrawErrRate);

					string Location = "E" + lon.ToString() + "°，N" + lat.ToString() + "°";
					double PerPointTime = PDERLTime.TotalSeconds / AllPointCount * 1000000000;

					int MaxReferLength = demAnalysisService.Analysis.pde_max_refer_length;
					int AllReferLength = demAnalysisService.Analysis.pde_all_refer_length;
					int CrossPointCount = demAnalysisService.Analysis.pde_cross_point_count;
					items.Add(new
					{
						Location,//坐标
						R3Time = PDERLTime.TotalSeconds,//R3计算时间
						Time = PDERLTime.TotalSeconds,//计算时间
						PerPointTime,//单点耗时（单位纳秒）
						AllPointCount,//共计算了多少点量
						VisibleRate,//计算区域的通视率
						PDERLVisibleErrCount,//可见点判错数量
						PDERLUnVisibleErrCount,//不可见点判错数量
						PDERLAllErrCount,//总错误点数
						PDERLVisibleErrRate,//可见点错误率
						PDERLUnVisibleErrRate,//不可见点错误率
						PDERLErrRate,//整体错误率
						MaxReferLength,//交叉点数量
						AllReferLength,//交叉点计算数量
						CrossPointCount //交叉点计算失败数量
					});

					sw.WriteLine(demAnalysisService.FileName + "," + Location + "," + AllPointCount + "," + VisibleRate + "," + dh + ","
						+ MaxReferLength + "," + AllReferLength + "," + CrossPointCount + ","
						+ PDERLTime.TotalSeconds + "," + PDERLTime.TotalSeconds + "," + TimeXPDERL.TotalSeconds + "," + TimeXDraw.TotalSeconds + "," + TimeRefF.TotalSeconds + ","
						+ PDERLVisibleErrCount + "," + PDERLUnVisibleErrCount + "," + PDERLAllErrCount + ","
						+ PDERLVisibleErrRate + "," + PDERLUnVisibleErrRate + "," + PDERLErrRate + ","
						+ XPDERLVisibleErrCount + "," + XPDERLUnVisibleErrCount + "," + XPDERLAllErrCount + ","
						+ XPDERLVisibleErrRate + "," + XPDERLUnVisibleErrRate + "," + XPDERLErrRate + ","
						+ XDrawVisibleErrCount + "," + XDrawUnVisibleErrCount + "," + XDrawAllErrCount + ","
						+ XDrawVisibleErrRate + "," + XDrawUnVisibleErrRate + "," + XDrawErrRate + ","
						+ RefFVisibleErrCount + "," + RefFUnVisibleErrCount + "," + RefFAllErrCount + ","
						+ RefFVisibleErrRate + "," + RefFUnVisibleErrRate + "," + RefFErrRate + ","
						);
				}


			}
			return items;
		}



		/// <summary>
		/// 各类算法时间比较计算随机实验（精度参照PDERL）
		/// </summary>
		/// <param name="r"></param>
		/// <param name="dh"></param>
		/// <param name="maxcount"></param>
		/// <returns></returns>
		[HttpGet("analysis_time_fixed_r/{r}/{dh}/{maxcount}")]
		public object DoAnalysisTimeRodomRepeateAll(double r, double dh, int maxcount)
		{
			Random radom = new Random();
			List<object> items = new List<object>();

			double dlon = demAnalysisService.DemData.DX;
			double dlat = demAnalysisService.DemData.DY;
			var startLon = demAnalysisService.DemData.StartX + r + dlon + 0.00181;//避免正好在格网上
			var startLat = demAnalysisService.DemData.StartY + r + dlat + 0.00181;
			var maxLon = demAnalysisService.DemData.MaxLon - r - dlon - 0.00181;
			var maxLat = demAnalysisService.DemData.MaxY - r - dlat - 0.00181;

			if (!Directory.Exists("../../RunningLog"))
				Directory.CreateDirectory("../../RunningLog");
			using (var fs = new FileStream("../../RunningLog/各类算法时间比较计算_"
				+ demAnalysisService.FileName + "_" + r.ToString() + "_" + dh.ToString() + "_" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString().Replace(":", "：") + ".csv", FileMode.Create, FileAccess.ReadWrite))
			using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
			{

				sw.WriteLine("数据文件,经度,纬度,半径,总点量,视点高度,通视率,参考线交点量,参考线求交计算量,PDERL算法耗时(秒),XPDERL算法耗时(秒),参考面算法耗时(秒),XDraw算法耗时(秒),XPDERL算法错点量,参考面算法错点量,XDraw算法错点量,XPDERL算法错点率,参考面算法错点率,XDraw算法错点率");
				sw.Flush();

				double count = 0;
				while (count < maxcount)
				{
					double lon = startLon + radom.NextDouble() * (maxLon - startLon);
					double lat = startLat + radom.NextDouble() * (maxLat - startLat);

					var toLon = lon + r;
					var toLat = lat;

					var date = DateTime.Now;

					//PDERL算法参照
					demAnalysisService.Analysis.DoAnalysisByPderlLonLat_Refer(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL_refer, out double utmLeftLon_refer, out double utmBottomLat_refer, out double perX_refer, out double perY_refer);
					var PDERLTime_refer = DateTime.Now - date;
					date = DateTime.Now;

					//PDERL算法
					demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
					var PDERLTime = DateTime.Now - date;
					date = DateTime.Now;

					demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XPDERL, out double utmLeftLon2, out double utmBottomLat2, out double perX2, out double perY2);
					var XPDERLTime = DateTime.Now - date;
					date = DateTime.Now;


					demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_RefF, out double utmLeftLon5, out double utmBottomLat5, out double perX5, out double perY5);
					var RefFTime = DateTime.Now - date;
					date = DateTime.Now;


					demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XDraw, out double utmLeftLon3, out double utmBottomLat3, out double perX3, out double perY3);
					var XDrawTime = DateTime.Now - date;
					date = DateTime.Now;


					int lonCount = result_PDERL.GetLength(0);
					int latCount = result_PDERL.GetLength(1);
					int AllPointCount = lonCount * latCount;

					//获取通视率
					double VisibleRate = GetVisibleRate(result_PDERL);

					GetErrRate(result_XPDERL, result_PDERL, lonCount, latCount, AllPointCount, out int XPDERLErrPointCount, out double XPDERLErrRate);
					GetErrRate(result_XDraw, result_PDERL, lonCount, latCount, AllPointCount, out int XDrawErrPointCount, out double XDrawErrRate);
					GetErrRate(result_RefF, result_PDERL, lonCount, latCount, AllPointCount, out int RefFErrPointCount, out double RefFErrRate);

					items.Add(new
					{
						Lontitude = lon,
						Lattitude = lat,
						R = lonCount,//半径
						SeeHeight = dh,//视点高度
						AllPointCount,//共计算了多少点量
						VisibleRate,//计算区域的通视率
						MaxReferLength = demAnalysisService.Analysis.pde_max_refer_length,
						AllReferLength = demAnalysisService.Analysis.pde_all_refer_length,//交叉点计算数量

						PDERLTime = PDERLTime.TotalSeconds,//计算时间
						PDERLPerPointTime = PDERLTime.TotalSeconds / AllPointCount * 1000000000,//单点耗时（单位纳秒）

						XPDERLTime = XPDERLTime.TotalSeconds,//计算时间
						XPDERLPerPointTime = XPDERLTime.TotalSeconds / AllPointCount * 1000000000,//单点耗时（单位纳秒）
						XPDERLErrPointCount,
						XPDERLErrRate,

						RefFTime = RefFTime.TotalSeconds,//计算时间
						RefFPerPointTime = RefFTime.TotalSeconds / AllPointCount * 1000000000,//单点耗时（单位纳秒）
						RefFErrPointCount,
						RefFErrRate,

						XDrawTime = XDrawTime.TotalSeconds,//计算时间
						XDrawPerPointTime = XDrawTime.TotalSeconds / AllPointCount * 1000000000,//单点耗时（单位纳秒）
						XDrawErrPointCount,
						XDrawErrRate,
					});

					sw.WriteLine(demAnalysisService.FileName + "," + lon + "," + lat + "," + lonCount + "," + AllPointCount + "," + dh + "," + VisibleRate + ","
						+ demAnalysisService.Analysis.pde_max_refer_length + "," + demAnalysisService.Analysis.pde_all_refer_length + "," +
						+PDERLTime.TotalSeconds + "," + XPDERLTime.TotalSeconds + "," + RefFTime.TotalSeconds + "," + XDrawTime.TotalSeconds
						+ "," + XPDERLErrPointCount + "," + RefFErrPointCount + "," + XDrawErrPointCount
						+ "," + XPDERLErrRate + "," + RefFErrRate + "," + XDrawErrRate);

					count++;

				}
				sw.Flush();
				fs.Flush();
			}

			return items;
		}

		/// <summary>
		/// 各种算法比较速度（不包括R3算法），指定半径，位置随机
		/// </summary>
		/// <param name="DoAnalysisLL"></param>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <param name="r"></param>
		/// <param name="dh"></param>
		/// <param name="max">最多计算多少个点，大于0有效</param>
		/// <returns></returns>
		[HttpGet("analysis_all_fixed_withoutr3/{r}/{dh}/{max}")]
		public void DoAnalysisTimeRodomRepeateWithoutR3(double r, double dh, int max)
		{
			if (max < 1) return;
			Random radom = new Random();

			double dlon = demAnalysisService.DemData.DX;
			double dlat = demAnalysisService.DemData.DY;
			var startLon = demAnalysisService.DemData.StartX + r + dlon + 0.00181;//避免正好在格网上
			var startLat = demAnalysisService.DemData.StartY + r + dlat + 0.00181;
			var maxLon = demAnalysisService.DemData.MaxLon - r - dlon - 0.00181;
			var maxLat = demAnalysisService.DemData.MaxY - r - dlat - 0.00181;

			if (!System.IO.File.Exists("../../RunningLog/固定半径速度测试.csv"))
			{
				if (!Directory.Exists("../../RunningLog"))
					Directory.CreateDirectory("../../RunningLog");
				using (var fs = new FileStream("../../RunningLog/固定半径速度测试.csv", FileMode.Create, FileAccess.ReadWrite))
				using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
				{
					sw.WriteLine("数据文件,经度,纬度,半径网格数,总点量,视点高度,通视率,求交点次数,参考线最大节点数,参考线总节点数,PDERL算法耗时(秒),XPDERL算法耗时(秒),参考面算法耗时(秒),XDraw算法耗时(秒)");
					sw.Flush();
				}
			}
			using (var fs = new FileStream("../../RunningLog/固定半径速度测试.csv", FileMode.Append, FileAccess.Write))
			using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
			{
				double count = 0;

				while (true)
				{
					double lon = startLon + radom.NextDouble() * (maxLon - startLon);
					double lat = startLat + radom.NextDouble() * (maxLat - startLat);

					var toLon = lon + r;
					var toLat = lat;

					var date = DateTime.Now;
					//PDERL算法
					demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
					var PDERLTime = DateTime.Now - date;
					date = DateTime.Now;

					//PDERL算法
					demAnalysisService.Analysis.DoAnalysisByPderlLonLat_Refer(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL_Refer, out double utmLeftLon_Refer, out double utmBottomLat_Refer, out double perX_Refer, out double perY_Refer);
					var PDERLTime_Refer = DateTime.Now - date;
					date = DateTime.Now;

					demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XPDERL, out double utmLeftLon2, out double utmBottomLat2, out double perX2, out double perY2);
					var XPDERLTime = DateTime.Now - date;
					date = DateTime.Now;


					demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_RefF, out double utmLeftLon5, out double utmBottomLat5, out double perX5, out double perY5);
					var RefFTime = DateTime.Now - date;
					date = DateTime.Now;


					demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XDraw, out double utmLeftLon3, out double utmBottomLat3, out double perX3, out double perY3);
					var XDrawTime = DateTime.Now - date;
					date = DateTime.Now;


					int lonCount = result_PDERL.GetLength(0);
					int latCount = result_PDERL.GetLength(1);
					int AllPointCount = lonCount * latCount;
					//获取通视率
					double VisibleRate = GetVisibleRate(result_PDERL);

					sw.WriteLine(demAnalysisService.FileName + "," + lon + "," + lat + "," + lonCount / 2 + "," + AllPointCount + "," + dh + "," + VisibleRate + ","
					+ demAnalysisService.Analysis.pde_cross_point_count + "," + demAnalysisService.Analysis.pde_max_refer_length + "," + demAnalysisService.Analysis.pde_all_refer_length + "," +
					+PDERLTime.TotalSeconds + "," + XPDERLTime.TotalSeconds + "," + RefFTime.TotalSeconds + "," + XDrawTime.TotalSeconds);
					sw.Flush();

					count++;
					if (count > max)
						return;
				}
			}
		}


		/// <summary>
		/// 比较各种算法的邻域错误数（与R3算法相比），指定半径，位置随机
		/// </summary>
		/// <param name="DoAnalysisLL"></param>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <param name="r"></param>
		/// <param name="dh"></param>
		/// <param name="max">最多计算多少个点，大于0有效</param>
		/// <returns></returns>
		[HttpGet("analysis_all_fixed_neighbor_err/{r}/{dh}/{max}")]
		public void DoAnalysisGetNeighborErrWithR3(double r, double dh, int max, string fileNameExtend = "")
		{
			if (max < 1) return;
			Random radom = new Random();

			double dlon = demAnalysisService.DemData.DX;
			double dlat = demAnalysisService.DemData.DY;
			var startLon = demAnalysisService.DemData.StartX + r + dlon + 0.00181;//避免正好在格网上
			var startLat = demAnalysisService.DemData.StartY + r + dlat + 0.00181;
			var maxLon = demAnalysisService.DemData.MaxLon - r - dlon - 0.00181;
			var maxLat = demAnalysisService.DemData.MaxY - r - dlat - 0.00181;

			if (!Directory.Exists("../../RunningLog"))
				Directory.CreateDirectory("../../RunningLog");
			string fileName = "../../RunningLog/测试各邻域错误数" + fileNameExtend + ".csv";

			if (!System.IO.File.Exists(fileName))
			{
				using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
				using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
				{
					sw.WriteLine("数据文件,经度,纬度,半径网格数,总点量,视点高度,通视率,PDERL错误率,XPDERL错误率,XDraw错误率,参考面错误率," +
						"1邻域（PDERL）,2邻域（PDERL）,3邻域（PDERL）,4邻域（PDERL）,5邻域（PDERL）,6邻域（PDERL）,7邻域（PDERL）,8邻域（PDERL）,9邻域（PDERL）,10邻域（PDERL）," +
						"1邻域（XPDERL）,2邻域（XPDERL）,3邻域（XPDERL）,4邻域（XPDERL）,5邻域（XPDERL）,6邻域（XPDERL）,7邻域（XPDERL）,8邻域（XPDERL）,9邻域（XPDERL）,10邻域（XPDERL）," +
						"1邻域（XDraw）,2邻域（XDraw）,3邻域（XDraw）,4邻域（XDraw）,5邻域（XDraw）,6邻域（XDraw）,7邻域（XDraw）,8邻域（XDraw）,9邻域（XDraw）,10邻域（XDraw）," +
						"1邻域（RP）,2邻域（RP）,3邻域（RP）,4邻域（RP）,5邻域（RP）,6邻域（RP）,7邻域（RP）,8邻域（RP）,9邻域（RP）,10邻域（RP）");
					sw.Flush();
				}
			}
			using (var fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
			using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
			{
				double count = 0;

				while (true)
				{
					double lon = startLon + radom.NextDouble() * (maxLon - startLon);
					double lat = startLat + radom.NextDouble() * (maxLat - startLat);

					var toLon = lon + r;
					var toLat = lat;

					var date = DateTime.Now;

					//R3
					demAnalysisService.Analysis.DoAnalysisByR3LonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_R3, out double utmLeftLon_Refer, out double utmBottomLat_Refer, out double perX_Refer, out double perY_Refer);
					var PDERLTime_R3 = DateTime.Now - date;
					date = DateTime.Now;

					//PDERL算法
					demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
					var PDERLTime = DateTime.Now - date;
					date = DateTime.Now;

					demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XPDERL, out double utmLeftLon2, out double utmBottomLat2, out double perX2, out double perY2);
					var XPDERLTime = DateTime.Now - date;
					date = DateTime.Now;

					demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_RefF, out double utmLeftLon5, out double utmBottomLat5, out double perX5, out double perY5);
					var RefFTime = DateTime.Now - date;
					date = DateTime.Now;


					demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XDraw, out double utmLeftLon3, out double utmBottomLat3, out double perX3, out double perY3);
					var XDrawTime = DateTime.Now - date;
					date = DateTime.Now;


					int lonCount = result_PDERL.GetLength(0);
					int latCount = result_PDERL.GetLength(1);
					int AllPointCount = lonCount * latCount;
					//获取通视率
					double VisibleRate = GetVisibleRate(result_PDERL);

					GetNeighborErr(result_PDERL, result_R3, 10, 0.9, out Dictionary<int, int> neighborErrCount_PDERL, out double errRate_PDERL, out double visibleErrRate_PDERL, out double unvisibleErrRate_PDERL);
					GetNeighborErr(result_XPDERL, result_R3, 10, 0.9, out Dictionary<int, int> neighborErrCount_XPDERL, out double errRate_XPDERL, out double visibleErrRate_XPDERL, out double unvisibleErrRate_XPDERL);
					GetNeighborErr(result_XDraw, result_R3, 10, 0.9, out Dictionary<int, int> neighborErrCount_XDraw, out double errRate_XDraw, out double visibleErrRate_XDraw, out double unvisibleErrRate_XDraw);
					GetNeighborErr(result_RefF, result_R3, 10, 0.9, out Dictionary<int, int> neighborErrCount_RP, out double errRate_RP, out double visibleErrRate_RP, out double unvisibleErrRate_RP);

					sw.WriteLine(demAnalysisService.FileName + "," + lon + "," + lat + "," + lonCount / 2 + "," + AllPointCount + "," + dh + "," + VisibleRate + "," + errRate_PDERL + "," + errRate_XPDERL + "," + errRate_XDraw + "," + errRate_RP + ","
					+ neighborErrCount_PDERL[1] + "," + neighborErrCount_PDERL[2] + "," + neighborErrCount_PDERL[3] + "," + neighborErrCount_PDERL[4] + "," + neighborErrCount_PDERL[5] + ","
					+ neighborErrCount_PDERL[6] + "," + neighborErrCount_PDERL[7] + "," + neighborErrCount_PDERL[8] + "," + neighborErrCount_PDERL[9] + "," + neighborErrCount_PDERL[10] + ","
					+ neighborErrCount_XPDERL[1] + "," + neighborErrCount_XPDERL[2] + "," + neighborErrCount_XPDERL[3] + "," + neighborErrCount_XPDERL[4] + "," + neighborErrCount_XPDERL[5] + ","
					+ neighborErrCount_XPDERL[6] + "," + neighborErrCount_XPDERL[7] + "," + neighborErrCount_XPDERL[8] + "," + neighborErrCount_XPDERL[9] + "," + neighborErrCount_XPDERL[10] + ","
					+ neighborErrCount_XDraw[1] + "," + neighborErrCount_XDraw[2] + "," + neighborErrCount_XDraw[3] + "," + neighborErrCount_XDraw[4] + "," + neighborErrCount_XDraw[5] + ","
					+ neighborErrCount_XDraw[6] + "," + neighborErrCount_XDraw[7] + "," + neighborErrCount_XDraw[8] + "," + neighborErrCount_XDraw[9] + "," + neighborErrCount_XDraw[10] + ","
					+ neighborErrCount_RP[1] + "," + neighborErrCount_RP[2] + "," + neighborErrCount_RP[3] + "," + neighborErrCount_RP[4] + "," + neighborErrCount_RP[5] + ","
					+ neighborErrCount_RP[6] + "," + neighborErrCount_RP[7] + "," + neighborErrCount_RP[8] + "," + neighborErrCount_RP[9] + "," + neighborErrCount_RP[10]
					);
					sw.Flush();

					count++;
					if (count > max)
						return;
				}
			}
		}

		/// <summary>
		/// 比较各种算法的邻域错误数（与PDERL算法相比），指定半径，位置随机
		/// </summary>
		/// <param name="DoAnalysisLL"></param>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <param name="r"></param>
		/// <param name="dh"></param>
		/// <param name="max">最多计算多少个点，大于0有效</param>
		/// <returns></returns>
		[HttpGet("analysis_all_fixed_neighbor_err/{r}/{dh}/{max}")]
		public void DoAnalysisGetNeighborErrWithPDERL(double r, double dh, int max, string fileNameExtend = "")
		{
			if (max < 1) return;
			Random radom = new Random();

			double dlon = demAnalysisService.DemData.DX;
			double dlat = demAnalysisService.DemData.DY;
			var startLon = demAnalysisService.DemData.StartX + r + dlon + 0.00181;//避免正好在格网上
			var startLat = demAnalysisService.DemData.StartY + r + dlat + 0.00181;
			var maxLon = demAnalysisService.DemData.MaxLon - r - dlon - 0.00181;
			var maxLat = demAnalysisService.DemData.MaxY - r - dlat - 0.00181;

			if (!Directory.Exists("../../RunningLog"))
				Directory.CreateDirectory("../../RunningLog");
			string fileName = "../../RunningLog/测试各邻域错误数" + fileNameExtend + ".csv";

			if (!System.IO.File.Exists(fileName))
			{
				using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
				using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
				{
					sw.WriteLine("数据文件,经度,纬度,半径网格数,总点量,视点高度,通视率,PDERL错误率,XPDERL错误率,XDraw错误率,参考面错误率," +
						"1邻域（PDERL）,2邻域（PDERL）,3邻域（PDERL）,4邻域（PDERL）,5邻域（PDERL）,6邻域（PDERL）,7邻域（PDERL）,8邻域（PDERL）,9邻域（PDERL）,10邻域（PDERL）," +
						"1邻域（XPDERL）,2邻域（XPDERL）,3邻域（XPDERL）,4邻域（XPDERL）,5邻域（XPDERL）,6邻域（XPDERL）,7邻域（XPDERL）,8邻域（XPDERL）,9邻域（XPDERL）,10邻域（XPDERL）," +
						"1邻域（XDraw）,2邻域（XDraw）,3邻域（XDraw）,4邻域（XDraw）,5邻域（XDraw）,6邻域（XDraw）,7邻域（XDraw）,8邻域（XDraw）,9邻域（XDraw）,10邻域（XDraw）," +
						"1邻域（RP）,2邻域（RP）,3邻域（RP）,4邻域（RP）,5邻域（RP）,6邻域（RP）,7邻域（RP）,8邻域（RP）,9邻域（RP）,10邻域（RP）");
					sw.Flush();
				}
			}
			using (var fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
			using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
			{
				double count = 0;

				while (true)
				{
					Console.WriteLine(1);

					double lon = startLon + radom.NextDouble() * (maxLon - startLon);
					double lat = startLat + radom.NextDouble() * (maxLat - startLat);

					var toLon = lon + r;
					var toLat = lat;

					var date = DateTime.Now;

					////R3
					//demAnalysisService.Analysis.DoAnalysisByR3LonLat(lon, lat, toLon, toLat, dh,
					//    out int[,] result_R3, out double utmLeftLon_Refer, out double utmBottomLat_Refer, out double perX_Refer, out double perY_Refer);
					//var PDERLTime_R3 = DateTime.Now - date;
					//date = DateTime.Now;

					Console.WriteLine(2);
					//PDERL算法
					demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);
					var PDERLTime = DateTime.Now - date;
					date = DateTime.Now;
					Console.WriteLine(3);

					demAnalysisService.Analysis.DoAnalysisByXPderlLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XPDERL, out double utmLeftLon2, out double utmBottomLat2, out double perX2, out double perY2);
					var XPDERLTime = DateTime.Now - date;
					date = DateTime.Now;
					Console.WriteLine(4);

					demAnalysisService.Analysis.DoAnalysisByRpLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_RefF, out double utmLeftLon5, out double utmBottomLat5, out double perX5, out double perY5);
					var RefFTime = DateTime.Now - date;
					date = DateTime.Now;
					Console.WriteLine(5);


					demAnalysisService.Analysis.DoAnalysisByXDrawLonLat(lon, lat, toLon, toLat, dh,
						out int[,] result_XDraw, out double utmLeftLon3, out double utmBottomLat3, out double perX3, out double perY3);
					var XDrawTime = DateTime.Now - date;
					date = DateTime.Now;
					Console.WriteLine(6);


					int lonCount = result_PDERL.GetLength(0);
					int latCount = result_PDERL.GetLength(1);
					int AllPointCount = lonCount * latCount;
					//获取通视率
					double VisibleRate = GetVisibleRate(result_PDERL);
					Console.WriteLine(7);

					GetNeighborErr(result_PDERL, result_PDERL, 10, 0.9, out Dictionary<int, int> neighborErrCount_PDERL, out double errRate_PDERL, out double visibleErrRate_PDERL, out double unvisibleErrRate_PDERL);
					GetNeighborErr(result_XPDERL, result_PDERL, 10, 0.9, out Dictionary<int, int> neighborErrCount_XPDERL, out double errRate_XPDERL, out double visibleErrRate_XPDERL, out double unvisibleErrRate_XPDERL);
					GetNeighborErr(result_XDraw, result_PDERL, 10, 0.9, out Dictionary<int, int> neighborErrCount_XDraw, out double errRate_XDraw, out double visibleErrRate_XDraw, out double unvisibleErrRate_XDraw);
					GetNeighborErr(result_RefF, result_PDERL, 10, 0.9, out Dictionary<int, int> neighborErrCount_RP, out double errRate_RP, out double visibleErrRate_RP, out double unvisibleErrRate_RP);

					Console.WriteLine(8);
					sw.WriteLine(demAnalysisService.FileName + "," + lon + "," + lat + "," + lonCount / 2 + "," + AllPointCount + "," + dh + "," + VisibleRate + "," + errRate_PDERL + "," + errRate_XPDERL + "," + errRate_XDraw + "," + errRate_RP + ","
					+ neighborErrCount_PDERL[1] + "," + neighborErrCount_PDERL[2] + "," + neighborErrCount_PDERL[3] + "," + neighborErrCount_PDERL[4] + "," + neighborErrCount_PDERL[5] + ","
					+ neighborErrCount_PDERL[6] + "," + neighborErrCount_PDERL[7] + "," + neighborErrCount_PDERL[8] + "," + neighborErrCount_PDERL[9] + "," + neighborErrCount_PDERL[10] + ","
					+ neighborErrCount_XPDERL[1] + "," + neighborErrCount_XPDERL[2] + "," + neighborErrCount_XPDERL[3] + "," + neighborErrCount_XPDERL[4] + "," + neighborErrCount_XPDERL[5] + ","
					+ neighborErrCount_XPDERL[6] + "," + neighborErrCount_XPDERL[7] + "," + neighborErrCount_XPDERL[8] + "," + neighborErrCount_XPDERL[9] + "," + neighborErrCount_XPDERL[10] + ","
					+ neighborErrCount_XDraw[1] + "," + neighborErrCount_XDraw[2] + "," + neighborErrCount_XDraw[3] + "," + neighborErrCount_XDraw[4] + "," + neighborErrCount_XDraw[5] + ","
					+ neighborErrCount_XDraw[6] + "," + neighborErrCount_XDraw[7] + "," + neighborErrCount_XDraw[8] + "," + neighborErrCount_XDraw[9] + "," + neighborErrCount_XDraw[10] + ","
					+ neighborErrCount_RP[1] + "," + neighborErrCount_RP[2] + "," + neighborErrCount_RP[3] + "," + neighborErrCount_RP[4] + "," + neighborErrCount_RP[5] + ","
					+ neighborErrCount_RP[6] + "," + neighborErrCount_RP[7] + "," + neighborErrCount_RP[8] + "," + neighborErrCount_RP[9] + "," + neighborErrCount_RP[10]
					);
					sw.Flush();
					Console.WriteLine(9);

					count++;
					if (count > max)
						return;
				}
			}
		}

		#endregion

		#region 工具方法

		/// <summary>
		/// 查找错误点数量与比率
		/// </summary>
		/// <param name="result_PDERL"></param>
		/// <param name="result_R3"></param>
		/// <param name="lonCount"></param>
		/// <param name="latCount"></param>
		/// <param name="AllPointCount"></param>
		/// <param name="PDERLVisibleErrCount"></param>
		/// <param name="PDERLUnVisibleErrCount"></param>
		/// <param name="PDERLErrRate"></param>
		/// <param name="PDERLVisibleErrRate"></param>
		/// <param name="PDERLUnVisibleErrRate"></param>
		private static void getErrCount(int[,] result_PDERL, int[,] result_R3, int lonCount, int latCount, int AllPointCount, out int PDERLVisibleErrCount, out int PDERLUnVisibleErrCount, out double PDERLErrRate, out double PDERLVisibleErrRate, out double PDERLUnVisibleErrRate)
		{
			//错点数量
			PDERLVisibleErrCount = 0;
			PDERLUnVisibleErrCount = 0;
			for (int i = 0; i < lonCount; i++)
				for (int j = 0; j < latCount; j++)
				{
					if (result_R3[i, j] == 1 && result_PDERL[i, j] == 0)//本来能看见的判为看不见
					{
						PDERLVisibleErrCount++;
					}
					else if (result_R3[i, j] == 0 && result_PDERL[i, j] == 1)//本来看不见的判为能看见
					{
						PDERLUnVisibleErrCount++;
					}
				}
			//错点率
			PDERLErrRate = (double)(PDERLUnVisibleErrCount + PDERLVisibleErrCount) / AllPointCount;
			PDERLVisibleErrRate = (double)(PDERLVisibleErrCount) / AllPointCount;
			PDERLUnVisibleErrRate = (double)(PDERLUnVisibleErrCount) / AllPointCount;
		}

		/// <summary>
		/// 获取错误率及数量
		/// </summary>
		/// <param name="result_Test"></param>
		/// <param name="result_Refer"></param>
		/// <param name="lonCount"></param>
		/// <param name="latCount"></param>
		/// <param name="AllPointCount"></param>
		/// <param name="ErrPointCount"></param>
		/// <param name="ErrRate"></param>
		private void GetErrRate(int[,] result_Test, int[,] result_Refer, int lonCount, int latCount, int AllPointCount, out int ErrPointCount, out double ErrRate)
		{
			//错点数量
			ErrPointCount = 0;
			for (int i = 0; i < lonCount; i++)
				for (int j = 0; j < latCount; j++)
				{
					if (result_Test[i, j] != result_Refer[i, j])
					{
						ErrPointCount++;

					}
				}
			//错点率
			ErrRate = (double)ErrPointCount / AllPointCount;
		}

		private void GetErrRate(int[,] result_Test, int[,] result_Refer, int lonCount, int latCount, int AllPointCount
			, out int VisibleErrCount, out int UnVisibleErrCount, out int ErrPointCount, out double VisibleErrRate, out double UnvisibleErrRate, out double ErrRate)
		{
			//错点数量
			ErrPointCount = 0;
			VisibleErrCount = 0;
			UnVisibleErrCount = 0;
			for (int i = 0; i < lonCount; i++)
				for (int j = 0; j < latCount; j++)
				{
					if (result_Refer[i, j] == 1 && result_Test[i, j] == 0)//本来能看见的判为看不见
					{
						VisibleErrCount++;

					}
					else if (result_Refer[i, j] == 0 && result_Test[i, j] == 1)//本来看不见的判为能看见
					{
						UnVisibleErrCount++;

					}
				}
			ErrPointCount = UnVisibleErrCount + VisibleErrCount;
			//错点率
			ErrRate = (double)ErrPointCount / AllPointCount;
			VisibleErrRate = (double)VisibleErrCount / AllPointCount;
			UnvisibleErrRate = (double)UnVisibleErrCount / AllPointCount;
		}

		/// <summary>
		/// 查找邻域错误点（以指定角为准，向右向下各maxTestNeighbor领域内，错误类型一致的，达到rate比率的数量）
		/// </summary>
		/// <param name="result_Test"></param>
		/// <param name="result_Refer"></param>
		/// <param name="maxTestNeighbor"></param>
		/// <param name="rate"></param>
		private void GetNeighborErr(int[,] result_Test, int[,] result_Refer, int maxTestNeighbor, double rate,
			out Dictionary<int, int> neighborErrCount, out double errRate, out double visibleErrRate, out double unvisibleErrRate)
		{
			int lonCount = result_Refer.GetLength(0);
			int latCount = result_Refer.GetLength(1);
			double allCount = lonCount * latCount;
			int visibleErrCount = 0;
			int unvisibleErrCount = 0;
			errRate = 0;
			visibleErrRate = 0;
			unvisibleErrRate = 0;

			neighborErrCount = new Dictionary<int, int>();
			if (maxTestNeighbor < 2) return;
			for (int i = 0; i <= maxTestNeighbor; i++)
			{
				neighborErrCount[i] = 0;//初始化
			}

			int InErrorCount = 0;
			int OutErrorCount = 0;
			//错点数量
			for (int i = 0; i < lonCount; i++)
				for (int j = 0; j < latCount; j++)
				{

					if (result_Refer[i, j] == 1 && result_Test[i, j] == 0)//本来能看见的判为看不见
					{
						visibleErrCount++;
						int count = 1;
						int tmpNeighbor = 1;
						while (tmpNeighbor <= maxTestNeighbor)
						{
							int LonS = i - tmpNeighbor;
							int LonE = i + tmpNeighbor;
							int LatS = j - tmpNeighbor;
							int LatE = j + tmpNeighbor;
							if (LonS < 0 || LatS < 0 || LonE >= lonCount || LatE >= latCount)
							{
								tmpNeighbor++;
								continue;
							}
							for (int tmpLat = LatS; tmpLat <= LatE; tmpLat++)
							{
								if (result_Refer[LonS, tmpLat] == 1 && result_Test[LonS, tmpLat] == 0)
									count++;

								if (result_Refer[LonE, tmpLat] == 1 && result_Test[LonE, tmpLat] == 0)
									count++;
							}
							for (int tmpLon = LonS + 1; tmpLon < LonE; tmpLon++)
							{
								if (result_Refer[tmpLon, LatS] == 1 && result_Test[tmpLon, LatS] == 0)
									count++;

								if (result_Refer[tmpLon, LatE] == 1 && result_Test[tmpLon, LatE] == 0)
									count++;
							}
							if (count >= Math.Ceiling(rate * (tmpNeighbor * 2 + 1) * (tmpNeighbor * 2 + 1)))
							{
								neighborErrCount[tmpNeighbor]++;
								InErrorCount++;
							}

							tmpNeighbor++;
						}

					}
					else if (result_Refer[i, j] == 0 && result_Test[i, j] == 1)//本来看不见的判为能看见
					{
						unvisibleErrCount++;
						int count = 1;
						int tmpNeighbor = 1;
						while (tmpNeighbor <= maxTestNeighbor)
						{
							int LonS = i - tmpNeighbor;
							int LonE = i + tmpNeighbor;
							int LatS = j - tmpNeighbor;
							int LatE = j + tmpNeighbor;
							if (LonS < 0 || LatS < 0 || LonE >= lonCount || LatE >= latCount)
							{
								tmpNeighbor++;
								continue;
							}
							for (int tmpLat = LatS; tmpLat <= LatE; tmpLat++)
							{
								if (result_Refer[LonS, tmpLat] == 0 && result_Test[LonS, tmpLat] == 1)
									count++;

								if (result_Refer[LonE, tmpLat] == 0 && result_Test[LonE, tmpLat] == 1)
									count++;
							}
							for (int tmpLon = LonS + 1; tmpLon < LonE; tmpLon++)
							{
								if (result_Refer[tmpLon, LatS] == 0 && result_Test[tmpLon, LatS] == 1)
									count++;

								if (result_Refer[tmpLon, LatE] == 0 && result_Test[tmpLon, LatE] == 1)
									count++;
							}
							if (count >= Math.Ceiling(rate * (tmpNeighbor * 2 + 1) * (tmpNeighbor * 2 + 1)))
							{
								neighborErrCount[tmpNeighbor]++;
								OutErrorCount++;
							}

							tmpNeighbor++;
						}

					}
				}
			errRate = (visibleErrCount + unvisibleErrCount) / allCount;
			visibleErrRate = visibleErrCount / allCount;
			unvisibleErrRate = unvisibleErrCount / allCount;

			if (InErrorCount > 2 && OutErrorCount > 2)
				;
		}

		/// <summary>
		/// 根据结果矩阵变成可视线
		/// </summary>
		/// <param name="resultArr"></param>
		/// <param name="perX"></param>
		/// <param name="perY"></param>
		/// <param name="leftLon"></param>
		/// <param name="bottomLat"></param>
		/// <returns></returns>
		private List<Line> GetDrawLines(int[,] resultArr, double perX, double perY, double leftLon, double bottomLat)
		{
			int lonCount = resultArr.GetLength(0);
			double halfLon = lonCount / 2.0;
			int latCount = resultArr.GetLength(1);
			double halfLat = latCount / 2.0;
			int AllPointCount = lonCount * latCount;

			List<Line> lines = new List<Line>();
			for (int i = 0; i < lonCount; i++)
			{
				bool inLine = false;
				var list = new List<LinePoint>();
				for (int j = 0; j < latCount; j++)
				{
					if (resultArr[i, j] == 1)
					{
						if (inLine == false)
						{
							if (j > halfLat)
							{
								list.Add(new LinePoint()
								{
									Lon = perX * i + leftLon,
									Lat = perY * (j - 0.5) + bottomLat
								});
							}
						}

						inLine = true;
						list.Add(new LinePoint()
						{
							Lon = perX * i + leftLon,
							Lat = perY * j + bottomLat
						});
					}
					else if (inLine)
					{
						if (list.Count > 0)
						{
							if (j < halfLat)
								list.Add(new LinePoint()
								{
									Lon = perX * i + leftLon,
									Lat = perY * (j + 0.5) + bottomLat
								});

							lines.Add(new Line() { Points = list });
							list = new List<LinePoint>();
						}
					}
				}
				if (list.Count > 0)
					lines.Add(new Line() { Points = list });
			}

			for (int j = 0; j < latCount; j++)
			{
				bool inLine = false;
				var list = new List<LinePoint>();
				for (int i = 0; i < lonCount; i++)
				{
					if (resultArr[i, j] == 1)
					{
						if (inLine == false)
						{
							if (i > halfLon)
							{
								list.Add(new LinePoint()
								{
									Lon = perX * (i - 0.5) + leftLon,
									Lat = perY * j + bottomLat
								});
							}
						}

						inLine = true;
						list.Add(new LinePoint()
						{
							Lon = perX * i + leftLon,
							Lat = perY * j + bottomLat
						});
					}
					else if (inLine)
					{
						if (list.Count > 0)
						{
							if (i < halfLon)
								list.Add(new LinePoint()
								{
									Lon = perX * (i + 0.5) + leftLon,
									Lat = perY * j + bottomLat
								});

							lines.Add(new Line() { Points = list });
							list = new List<LinePoint>();
						}
					}
				}
				if (list.Count > 0)
					lines.Add(new Line() { Points = list });
			}
			return lines;
		}

		/// <summary>
		/// 获取通视率
		/// </summary>
		/// <param name="result_R3"></param>
		private static double GetVisibleRate(int[,] result_R3)
		{
			int lonCount = result_R3.GetLength(0);
			int latCount = result_R3.GetLength(1);
			int AllPointCount = lonCount * latCount;
			//计算通视率
			int visibleCount = 0;
			for (int i = 0; i < lonCount; i++)
				for (int j = 0; j < latCount; j++)
				{
					if (result_R3[i, j] == 1)
						visibleCount++;
				}
			//通视率
			return (double)visibleCount / AllPointCount;
		}

		/// <summary>
		/// 获取指定经纬度的高度
		/// </summary>
		/// <param name="lon"></param>
		/// <param name="lat"></param>
		/// <returns></returns>
		[HttpGet("height/{lon}/{lat}")]
		public ActionResult<double> GetHeight(double lon, double lat)
			=> demAnalysisService.Analysis.GetHeight(lon, lat);

		/// <summary>
		/// 获取指定区域的DEM
		/// </summary>
		/// <param name="startLon"></param>
		/// <param name="endLon"></param>
		/// <param name="startLat"></param>
		/// <param name="endLat"></param>
		/// <returns></returns>
		[HttpGet("data/{startLon}/{endLon}/{startLat}/{endLat}")]
		public ActionResult<object> GetDemData(double startLon, double endLon, double startLat, double endLat)
		{
			demAnalysisService.DemData.GetPart(ref startLon, ref endLon, ref startLat, ref endLat,
				out double[,] Height, out double Dlon, out double Dlat, out string ProjStr);
			return new
			{
				Name = demAnalysisService.FileName,
				ProjStr,
				startLon,
				endLon,
				startLat,
				endLat,
				Dlon,
				Dlat,
				Height
			};
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}

		// GET api/values
		//[HttpGet]
		//public ActionResult<bool> Soft()
		//{
		//    demAnalysisService.DemData.Soft(2,6);
		//    //GetAnalysis(119.5, 41.5, 119.53, 41.53, 2);
		//    return true;
		//}
		#endregion
	}
}
