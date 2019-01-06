using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Server
{
    /// <summary>
    /// 温度测量设备的历史数据
    /// </summary>
    public class TemparetureHistoryData
    {
        /// <summary>
        /// 记录时间。
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Wifi信号强度
        /// </summary>
        public int WiFiStrength { get; set; }

        /// <summary>
        /// 设备测量温度，摄氏度。
        /// </summary>
        public double Temp { get; set; }

        /// <summary>
        /// 电池电压
        /// </summary>
        public double Battery { get; set; }

    }
}
