using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Server
{
    public interface IDbRepository
    {
        IList<Thermometer> GetThermometers(string openId);

        Thermometer GetThermometer(string openId, int id);

        Thermometer UpdateThermometer(string openId, int id, string name);

        Thermometer AddThermometer(string openId, string mac, string name);

        void RemoveThermometer(string openId, int id);

        void SaveThermometerStatus(Thermometer device);

        IList<TemparetureHistoryData> GetTemparetureHistory(string openId, int id, DateTime start, DateTime end);
    }
}
