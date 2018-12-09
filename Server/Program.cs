using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Temp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

            return WebHost.CreateDefaultBuilder(args)
              .UseKestrel(options =>
              {
                  options.Listen(IPAddress.Any, 443, listOptions =>
                  {
                      listOptions.UseHttps("./https.pfx", config["HttpsCertPassword"]);
                  });
              })
              .UseStartup<Startup>();
        }
    }
}
