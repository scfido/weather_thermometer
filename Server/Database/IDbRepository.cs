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

        int AddThermometer(string openId, string mac);

        void RemoveThermometer(string openId, int id);

        void SaveThermometerStatus(Thermometer device);

        IList<TemparetureHistoryData> GetTemparetureHistory(string openId, int id, DateTime start, DateTime end);
    }
}
