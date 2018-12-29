using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WeatherStation.Server;
using Microsoft.Net.Http;
using Newtonsoft.Json;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly IDbRepository db;
        readonly ILogger<AccountController> logger;
        readonly IConfiguration configuration;
        HttpClient httpClient;

        public AccountController(IDbRepository db, ILogger<AccountController> logger, IConfiguration configuration, HttpClient httpClient)
        {
            this.db = db;
            this.logger = logger;
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        [HttpGet("login/{code}")]
        public async Task<string> Login(string code)
        {
            //GET https://api.weixin.qq.com/sns/jscode2session?appid=APPID&secret=SECRET&js_code=JSCODE&grant_type=authorization_code
            //Response 200OK {"session_key":"Pw2PbhyOuHiuz7W4QfimKw==","openid":"oxihd5c4EBDVEUNCLRJhvkS6l1Xg"}
            httpClient.BaseAddress = new Uri("https://api.weixin.qq.com");

            var appId = configuration.GetValue<string>("WeChat:AppId");
            var secret = configuration.GetValue<string>("WeChat:AppSecret");
            var ret = await httpClient.GetStringAsync($"/sns/jscode2session?appid={appId}&secret={secret}&js_code={code}&grant_type=authorization_code");
            var session = JsonConvert.DeserializeObject<WeChatSession>(ret);

            return session.Openid;
        }
    }
}