using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.Server
{
    /// <summary>
    /// 温度测量设备
    /// </summary>
    public class TempDevice
    {
        /// <summary>
        /// 数据库ID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 设备连接Wifi SSID名称
        /// </summary>
        public string SSID { get; set; }

        /// <summary>
        /// Wifi信号强度
        /// </summary>
        public int WiFiStrength { get; set; }

        /// <summary>
        /// MAC地址
        /// </summary>
        public string MAC { get; set; }

        /// <summary>
        /// 设备测量温度，摄氏度。
        /// </summary>
        public double Temp { get; set; }

        /// <summary>
        /// 设备是否已连接外接电源
        /// </summary>
        public bool Power { get; set; }

        /// <summary>
        /// 是否正在充电中
        /// </summary>
        public bool Charge { get; set; }

        /// <summary>
        /// 电池电压
        /// </summary>
        public double Battery { get; set; }
}
}
