using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WeatherStation.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThermometerController : ControllerBase
    {
        readonly IDbRepository db;
        readonly ILogger<ThermometerController> logger;

        public ThermometerController(IDbRepository db, ILogger<ThermometerController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        // 获取指定人员的温度计信息
        // GET api/thermometer/openid
        [HttpGet("{openId}")]
        public ActionResult<IList<Thermometer>> Get(string openId)
        {
            return Ok(db.GetThermometers(openId));
        }

        // 添加指定人员的温度计
        [HttpPost("{openid}/{mac}")]
        public ActionResult<Thermometer> Post(string openId, string mac, [FromBody]Thermometer device)
        {
            return db.AddThermometer(openId, mac, device.Name);
        }

        // 更新温度数据和设备状态
        // Get http://hostname/api/thermometer/update?ver=1.0&sn=123456AB&ssid=wifi&key=34235&batt=3.6&rssi=5334&power=3&temp=11.5&charge=0;
        // GET api/values/5
        [HttpGet("update")]
        public ActionResult<Thermometer> Update([FromBody]Thermometer device)
        {
            return NoContent();
        }


        // 修改指定人员的温度计名称
        // PUT api/thermometer/openid/mac
        [HttpPut("{openid}/{id}")]
        public ActionResult<Thermometer> Put(string openId, int id, [FromBody]Thermometer device)
        {
           return db.UpdateThermometer(openId, id, device.Name);
        }


        // 删除指定人员的温度计
        // DELETE api/thermometer/openid/mac
        [HttpDelete("{openid}/{id}")]
        public ActionResult Delete(string openId, int id)
        {
            db.RemoveThermometer(openId, id);
            return NoContent();
        }
    }
}
