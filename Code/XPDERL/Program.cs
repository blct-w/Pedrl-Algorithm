using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;

namespace PDERLTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var x = 1.0 / 3;
            //var x2 = 1.0 / 6;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseUrls("http://*:8000")
                .UseStartup<Startup>();
    }
}
