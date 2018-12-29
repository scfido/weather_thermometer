using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace WeatherStation.Server.Testing
{
    public class TestWebHost
    {

        public TestWebHost()
        {
        }

        public TestServer Create(
            Action<IServiceCollection> services = null,
            Action<IApplicationBuilder> app = null,
            Action<MockHttpMessageHandler> mockHttp = null
            )
        {

            var builder = WebHost.CreateDefaultBuilder()
                .UseContentRoot("../../../../Server")
                .UseEnvironment("Testing")
                .UseStartup<TestStartup>();

            if (services != null)
                builder.ConfigureTestServices(services);

            if (app != null)
                builder.ConfigureTestContainer(app);

            if (mockHttp != null)
            {
                // ConfigureTestServices与ConfigureServices的区别是执行顺序不同，顺序如下：
                // ConfigureServices -> TestStartup -> ConfigureTestServices
                builder.ConfigureTestServices(svr =>
                {
                    var mock = new MockHttpMessageHandler();
                    mockHttp(mock);

                    //如果自定义了拦截Http请求，则移除所有的HttpClient。
                    svr.Where(s => s.ServiceType == typeof(HttpClient))
                        .ToList()
                        .ForEach(item => svr.Remove(item));

                    svr.AddTransient(s =>
                    {
                        return new HttpClient(mock);

                    });
                });
            }

            return new TestServer(builder);
        }
    }
}
