using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Server
{
    public interface IDbRepository
    {
        Task<IList<Thermometer>> GetThermometers(string openId);

        Task<Thermometer> GetThermometer(string openId, int id);

        Task<Thermometer> GetThermometer(string sn);

        Task<Thermometer> GetThermometer(string sn, string openId);


        Task<Thermometer> UpdateThermometer(string openId, int id, string name);

        Task<Thermometer> AddThermometer(string openId, string sn, string name);

        Task RemoveThermometer(string openId, string sn);

        Task SaveThermometerStatus(Thermometer device);

        Task<IList<TemparetureHistoryData>> GetTemparetureHistory(string openId, int id, DateTime start, DateTime end);

        Task<User> GetUserBySession(string session);

        Task<User> AddUser(User user);

        Task<User> UpdateUser(User user);

        Task RemoveUser(string openId);
    }
}
