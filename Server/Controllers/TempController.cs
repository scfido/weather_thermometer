using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Temp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        readonly IDeviceRepository db;
        public DeviceController(IDeviceRepository db)
        {
            this.db = db;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IList<TempDevice>> Get()
        {
            return Ok(db.GetDevices());

            //var devices = new TempDevice[] {
            //    new TempDevice
            //    {
            //        MAC="AD-EF-23-45-23",
            //        SSID="kxc",
            //        Temp =10.1,
            //        Power = true,
            //        Charge = true,
            //        Battery = 1.5,
            //        WiFiStrength=-40,
            //    }
            //};

            //return devices;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return $"id={id}";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
