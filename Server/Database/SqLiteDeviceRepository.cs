using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.Server
{
    public class SqLiteDeviceRepository : SqLiteBaseRepository, IDeviceRepository
    {
        private readonly ILogger logger;

        public SqLiteDeviceRepository(IConfiguration config, ILogger<SqLiteDeviceRepository> logger)
            : base(config)
        {
            this.logger = logger;

            if (!File.Exists(DbFile))
            {
                logger.LogWarning($"数据库\"{DbFile}\"不存在将创建。");
                try
                {
                    CreateDatabase();
                    logger.LogInformation($"数据库创建成功。");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"数据库创建失败。");
                }
            }
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
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                device.Id = cnn.Query<int>(
                    @"INSERT INTO Device 
                    (SSID, WiFiStrength, MAC,Temp,Power,Charge,Battery ) 
                    VALUES 
                    ( @SSID, @WiFiStrength, @MAC, @Temp, @Power, @Charge, @Battery );
                    select last_insert_rowid()", device)
                    .First();
            }
        }

        private void CreateDatabase()
        {
            //创建数据库文件夹
            var dbDir = Path.GetDirectoryName(DbFile);
            if (!Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);

            using (var db = SimpleDbConnection())
            {
                db.Open();
                db.Execute(
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
