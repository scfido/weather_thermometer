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
using System.Net;
using Newtonsoft.Json;

namespace WeatherStation.Server.Testing
{
    public class ThermometerControllerTest : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;
        private readonly string session;

        public ThermometerControllerTest(TestWebApplicationFactory<Startup> factory)
        {
            client = factory.Create(mock =>
            {
                //拦截服务端请求微信api的请求
                mock.When("https://api.weixin.qq.com/sns/jscode2session").Respond("application/json", "{\"session_key\":\"Pw2PbhyOuHiuz7W4QfimKw== \",\"openid\":\"oxihd5c4EBDVEUNCLRJhvkS6l1Xg\"}");
            });

            var json = client.GetStringAsync("/api/account/login/abcdef").Result;
            var actual = new { Session = string.Empty };
            actual = JsonConvert.DeserializeAnonymousType(json, actual);
            session = actual.Session;
        }

        [Fact]
        public async Task GetTest()
        {
            var actual = await client.GetStringAsync($"api/thermometer/{session}");
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task PostTest()
        {
            var actual = await client.PostAsJsonAsync<object>($"/api/thermometer/{session}/123456AB", new { name = "test" });
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task UploadTest()
        {
            var url = "api/thermometer/upload?ver=1.0&sn=123456AB&ssid=wifi&key=34235&batt=3.6&rssi=5334&power=3&temp=11.5&charge=0";
            var actual = await client.GetStringAsync(url);

            Assert.Equal("11.50,360", actual);
        }

        [Fact]
        public async Task PutTest()
        {
            var post = await client.PostAsJsonAsync<object>($"/api/thermometer/{session}/123321", new { name = "test" });
            var thermometer = JsonConvert.DeserializeObject<Thermometer>(await post.Content.ReadAsStringAsync());
            var actual = await client.PutAsJsonAsync<object>($"/api/thermometer/{session}/{thermometer.Id}", new { name = "test1" });
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        [Fact]
        public async Task DeleteTest()
        {
            var post = await client.PostAsJsonAsync<object>($"/api/thermometer/{session}/del", new { name = "test" });
            var thermometer = JsonConvert.DeserializeObject<Thermometer>(await post.Content.ReadAsStringAsync());

            var actual = await client.DeleteAsync($"/api/thermometer/{session}/{thermometer.Sn}");
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }
    }
}
