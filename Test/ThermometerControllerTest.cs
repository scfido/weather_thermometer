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

namespace WeatherStation.Server.Testing
{
    public class ThermometerControllerTest : IClassFixture<TestWebHost>
    {
        private readonly HttpClient client;

        public ThermometerControllerTest(TestWebHost host)
        {
            client = host.Create()
            .CreateClient();
        }

        [Fact]
        public async Task GetTest()
        {
            var actual = await client.GetStringAsync("api/thermometer/openid");
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task PostTest()
        {
            var actual = await client.PostAsJsonAsync<object>("/api/thermometer/openid/mac", new { name = "test" });
            //Assert.Equal("oxihd5c4EBDVEUNCLRJhvkS6l1Xg", actual);
        }
        [Fact]
        public async Task UpdateTest()
        {
            var actual = await client.GetStringAsync("/api/thermometer/update");
        }

        [Fact]
        public async Task PutTest()
        {
            var actual = await client.PutAsJsonAsync<object>("/api/thermometer/openid/1", new { name = "test1" });
        }
        [Fact]
        public async Task DeleteTest()
        {
            var actual = await client.DeleteAsync("/api/thermometer/openid/1");
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }
    }
}
