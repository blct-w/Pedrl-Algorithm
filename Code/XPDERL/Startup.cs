using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PDERLTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        DemAnalysisService demAnalysisService;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            demAnalysisService = new DemAnalysisService();
            services.AddSingleton(demAnalysisService);

            string[] urls = new string[] { "http://localhost:8080" };
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin", builder =>
                {
                    builder.WithOrigins(urls)
                    .AllowAnyMethod()
                    .AllowAnyHeader().AllowAnyOrigin().AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            app.UseCors("AllowAllOrigin");//必须位于UserMvc之前 

			var fe = new FileExtensionContentTypeProvider();
			fe.Mappings[".terrain"] = "application/octet-stream";
			app.UseStaticFiles(new StaticFileOptions() { ContentTypeProvider = fe });
            //app.UseStaticFiles();

            app.UseWebSockets();
            
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (IServiceScope scope = app.ApplicationServices.CreateScope())
                    {

                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        Console.WriteLine("getconnect:" + context.Request.HttpContext.Connection.RemoteIpAddress.ToString());
                        //The AcceptWebSocketAsync method upgrades the TCP connection to a WebSocket connection and provides a WebSocket object. Use the WebSocket object to send and receive messages
                        await Echo(context, webSocket);
                    }
                }
                else
                {
                    //Hand over to the next middleware
                    await next();
                }
            });

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Add("index.html");    //将index.html改为需要默认起始页的文件名.
            app.UseDefaultFiles(options);
            app.UseStaticFiles();
            app.UseMvc();
        }

        private async Task Echo(HttpContext context, WebSocket webSocket) //Echo 响应的意思
        {

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string receiveText;
            while (!result.CloseStatus.HasValue)
            {
                receiveText = System.Text.Encoding.Default.GetString(buffer).Trim();
                try
                {
                    string[] res = receiveText.Split(",");
                    var lon = Convert.ToDouble(res[0]);
                    var lat = Convert.ToDouble(res[1]);
                    var lon2 = Convert.ToDouble(res[2]);
                    var lat2 = Convert.ToDouble(res[3]);
                    var height = Convert.ToDouble(res[4]);
                    demAnalysisService.Analysis.DoAnalysisByPedrlLonLat(lon, lat, lon2, lat2, height,
                    out int[,] result_PDERL, out double demMinX, out double demMinY, out double perX, out double perY);

                    var x = result_PDERL.GetLength(0);
                    var y = result_PDERL.GetLength(1);
                    float dlon = (float)perX * result_PDERL.GetLength(0);
                    float dlat = (float)perY * result_PDERL.GetLength(1);
                    float startLon = (float)(lon - dlon / 2);
                    float startLat = (float)(lat - dlat / 2);
                    string start = $"{startLon},{ startLat},{startLon + dlon},{startLat},{startLon + dlon}," +
                        $"{ startLat + dlat},{startLon},{ startLat + dlat},{startLon},{startLat}|{x},{y}|";
                    byte[] con = System.Text.Encoding.UTF8.GetBytes(start);

                    byte[] bts = new byte[con.Length + x * y];
                    con.CopyTo(bts, 0);

                    for (int i = 0; i < x; i++)
                        for (int j = 0; j < y; j++)
                        {
                            bts[con.Length + i * x + j] = (byte)result_PDERL[i, j];
                        }


                    await webSocket.SendAsync(new ArraySegment<byte>(bts, 0, bts.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch { }
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            Console.WriteLine($"客户端断开链接");
        }

    }
}
