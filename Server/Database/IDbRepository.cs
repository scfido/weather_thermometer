using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Server
{
    public interface IDbRepository
    {
        IList<Thermometer> GetThermometers();

        Thermometer GetThermometer(int id);

        void InsertThermometer(Thermometer device);

        void SaveThermometerStatus(Thermometer device);

        void GetTemparetureHistory(string mac, DateTime start, DateTime end);
    }
}
