using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WeatherStation.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThermometerController : ControllerBase
    {
        readonly IDbRepository db;
        readonly ILogger<ThermometerController> logger;
        readonly IConfiguration config;

        public ThermometerController(IDbRepository db, ILogger<ThermometerController> logger, IConfiguration config)
        {
            this.db = db;
            this.logger = logger;
            this.config = config;
        }

        // 获取指定人员的温度计信息
        // GET api/thermometer/openid
        [HttpGet("{session}")]
        public async Task<IList<Thermometer>> Get(string session)
        {
            string openId = await GetOpenId(session);
            if (openId == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }

            return await db.GetThermometers(openId);
        }

        // 添加指定人员的温度计
        [HttpPost("{session}/{sn}")]
        public async Task<Thermometer> Post(string session, string sn, [FromBody]Thermometer device)
        {
            string openId = await GetOpenId(session);
            if (openId == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }

            if (await db.GetThermometer(sn) == null)
                return await db.AddThermometer(openId, sn, device.Name);
            else
                return null;
        }

        // 更新温度数据和设备状态
        // Get api/thermometer/upload?ver=1.0&sn=123456AB&ssid=wifi&key=34235&batt=3.6&rssi=5334&power=3&temp=11.5&charge=0;
        [HttpGet("upload")]
        public async Task<string> Upload(string ver, string sn, string ssid, string key, double batt, int rssi, int power, double temp, int charge)
        {
            var device = await db.GetThermometer(sn);
            if (device == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return string.Empty;
            }

            device.Firmware = ver;
            device.SSID = ssid;
            device.Key = key;
            device.Power = power;
            device.Charge = charge;
            device.Battery = batt;
            device.Temperature = temp;
            device.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            device.WiFiStrength = rssi;
            device.LastUpdate = DateTime.Now;

            await db.SaveThermometerStatus(device);
            //返回值“屏幕上显示的数字,下次唤醒设备间隔的秒数”
            return $"{temp.ToString("00.00")},{config.GetValue<string>("Thermometer:SleepDuration")}";
        }

        // 修改指定人员的温度计名称
        // PUT api/thermometer/openid/id
        [HttpPut("{session}/{id}")]
        public async Task<Thermometer> Put(string session, int id, [FromBody]Thermometer device)
        {
            string openId = await GetOpenId(session);
            if (openId == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }

            return await db.UpdateThermometer(openId, id, device.Name);
        }


        // 删除指定人员的温度计
        // DELETE api/thermometer/openid/sn
        [HttpDelete("{session}/{sn}")]
        public async Task Delete(string session, string sn)
        {
            string openId = await GetOpenId(session);
            if (openId == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await db.RemoveThermometer(openId, sn);
        }

        private async Task<string> GetOpenId(string session)
        {
            var user = await db.GetUserBySession(session);
            if (user == null)
                return null;
            else
                return user.OpenId;
        }
    }
}
