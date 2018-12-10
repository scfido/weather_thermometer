using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.Server
{
    public class SqLiteDeviceRepository : SqLiteBaseRepository, IDeviceRepository
    {
        public SqLiteDeviceRepository(IConfiguration config)
            : base(config)
        {
            if (!File.Exists(DbFile))
                CreateDatabase();
        }

        public IList<TempDevice> GetDevices()
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = cnn.Query<TempDevice>(
                    @"SELECT Id, SSID, WiFiStrength, MAC,Temp,Power,Charge,Battery
                    FROM Device").ToList();

                return result;
            }
        }

        public TempDevice GetDevice(int id)
        {
            if (!File.Exists(DbFile))
                return null;

            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = cnn.Query<TempDevice>(
                    @"SELECT Id, SSID, WiFiStrength, MAC,Temp,Power,Charge,Battery
                    FROM Device
                    WHERE Id = @id", new { id }).FirstOrDefault();
                return result;
            }
        }

        public void SaveDevice(TempDevice device)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                device.Id = cnn.Query<int>(
                    @"INSERT INTO Device 
                    (SSID, WiFiStrength, MAC,Temp,Power,Charge,Battery ) VALUES 
                    ( @SSID, @WiFiStrength, @MAC, @Temp, @Power, @Charge, @Battery );
                    select last_insert_rowid()", device).First();
            }
        }

        private static void CreateDatabase()
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"create table Device
                      (
                        ID                  integer primary key AUTOINCREMENT,
                        SSID                varchar(100) not null,
                        WiFiStrength        INTEGER,
                        MAC                 varchar(100) not null,
                        Temp                REAL,
                        Power               INTEGER,
                        Charge              INTEGER,
                        Battery             REAL,
                        CreatedDate         datetime not null,
                        LastUpdate          datetime
                      )"
                );
            }
        }
    }
}
