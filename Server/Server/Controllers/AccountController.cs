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
        readonly HttpClient httpClient;

        public AccountController(
            IDbRepository db,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            this.db = db;
            this.logger = logger;
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        [HttpGet("login/{code}")]
        public async Task<dynamic> Login(string code)
        {
            //GET https://api.weixin.qq.com/sns/jscode2session?appid=APPID&secret=SECRET&js_code=JSCODE&grant_type=authorization_code
            //Response 200 OK: {"session_key":"Pw2PbhyOuHiuz7W4QfimKw==","openid":"oxihd5c4EBDVEUNCLRJhvkS6l1Xg"}
            httpClient.BaseAddress = new Uri("https://api.weixin.qq.com");

            var appId = configuration.GetValue<string>("WeChat:AppId");
            var secret = configuration.GetValue<string>("WeChat:AppSecret");
            try
            {
                var ret = await httpClient.GetStringAsync($"/sns/jscode2session?appid={appId}&secret={secret}&js_code={code}&grant_type=authorization_code");
                var weChatSession = JsonConvert.DeserializeObject<WeChatSession>(ret);
                if (weChatSession.ErrorCode == 0)
                {
                    var newUser = false;
                    var user = await db.GetUserBySession(weChatSession.OpenId);
                    if (user == null)
                    {
                        //新用户
                        newUser = true;
                        user = new User()
                        {
                            OpenId = weChatSession.OpenId,
                        };
                    }

                    user.WeChatSessioKey = weChatSession.SessionKey;
                    user.LastLogin = DateTime.Now;
                    user.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    user.Session = Guid.NewGuid().ToString();

                    if (newUser)
                        await db.AddUser(user);
                    else
                        await db.UpdateUser(user);

                    //成功返回session
                    return new { user.Session };
                }
                else
                {
                    logger.LogWarning("登陆失败，错误代码:" + weChatSession.ErrorCode);
                    //失败返回null。
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "登陆出现异常。");
                return null;
            }

        }
    }
}