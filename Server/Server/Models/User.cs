using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Server
{
    public class User
    {
        public int Id { get; set; }

        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 微信获取的session_key
        /// </summary>
        public string WeChatSessioKey { get; set; }
        
        /// <summary>
        /// 本系统分配给用户的回话标识。
        /// </summary>
        public string Session { get; set; }

        /// <summary>
        /// 上次访问IP
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 上次登陆时间
        /// </summary>
        public DateTime LastLogin { get; set; }
    }
}
