using System;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using RichardSzalay.MockHttp;

namespace WeatherStation.Server.Testing
{
    public class AccountControllerTest : IClassFixture<TestWebHost>
    {
        private readonly HttpClient client;

        public AccountControllerTest(TestWebHost host)
        {
            client = host.Create(mockHttp: mock =>
            {
                //拦截服务端请求微信api的请求
                mock.When("https://api.weixin.qq.com/sns/jscode2session").Respond("application/json", "{\"session_key\":\"Pw2PbhyOuHiuz7W4QfimKw== \",\"openid\":\"oxihd5c4EBDVEUNCLRJhvkS6l1Xg\"}");
            })
            .CreateClient();
        }

        [Fact]
        public async Task LoginTest()
        {
            var actual = await client.GetStringAsync("/api/account/login/abcdef");
            Assert.Equal("oxihd5c4EBDVEUNCLRJhvkS6l1Xg", actual);
        }
    }
}
