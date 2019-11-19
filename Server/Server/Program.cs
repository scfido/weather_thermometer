﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace WeatherStation.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        // public static void Main(string[] args)
        // {
        //     CreateWebHostBuilder(args).Build().Run();
        // }

        // public static IHostBuilder CreateWebHostBuilder(string[] args)
        // {
        //     return Host.CreateDefaultBuilder(args)
        //         .ConfigureWebHostDefaults(webBuilder =>
        //         {
        //             webBuilder.UseStartup<Startup>();
        //         });


        // var config = new ConfigurationBuilder()
        //     .AddEnvironmentVariables()
        //     .AddUserSecrets<Program>()
        //     .Build();

        //var certPath = Path.Join(Directory.GetCurrentDirectory(), "https.pfx");
        // if (!File.Exists(certPath))
        //     throw new FileNotFoundException($"没有找到https证书 {certPath}");

        // return WebHost.CreateDefaultBuilder(args)
        //     .UseKestrel(options =>
        //     {
        //         options.Listen(IPAddress.Any, 80);
        // options.Listen(IPAddress.Any, 443, listOptions =>
        // {
        //     listOptions.UseHttps(certPath, config["HttpsCertPassword"]);
        // });
        //     })
        //     .UseStartup<Startup>();
        //}
    }
}
