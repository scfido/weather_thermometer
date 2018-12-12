using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Server
{
    public class SqLiteDbRepository : SqLiteBaseRepository, IDbRepository
    {
        private readonly ILogger logger;

        public SqLiteDbRepository(IConfiguration config, ILogger<SqLiteDbRepository> logger)
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

        public IList<Thermometer> GetThermometers()
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = cnn.Query<Thermometer>(
                    @"SELECT Id, SSID, WiFiStrength, MAC, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware
                    FROM Thermometer").ToList();

                return result;
            }
        }

        public Thermometer GetThermometer(int id)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = cnn.Query<Thermometer>(
                    @"SELECT Id, SSID, WiFiStrength, MAC, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware
                    FROM Thermometer
                    WHERE Id = @id", new { id }).FirstOrDefault();
                return result;
            }
        }

        public void InsertThermometer(Thermometer device)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                device.Id = cnn.Query<int>(
                    @"INSERT INTO Thermometer 
                    (SSID, WiFiStrength, MAC, Temp, Power, Charge, Battery, IPAddress, Firmware) 
                    VALUES 
                    ( @SSID, @WiFiStrength, @MAC, @Temp, @Power, @Charge, @Battery, @IPAddress, @Firmware );
                    select last_insert_rowid()", device)
                    .First();
            }
        }

        public void SaveThermometerStatus(Thermometer device)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();

                bool exist = cnn.Query<int>(
                    @"SELECT count(*) FROM Thermometer WHERE MAC=@MAC",
                    device
                )
                .Single() > 0;

                if (!exist)
                {
                    logger.LogWarning($"来自{device.IPAddress}的设备{device.MAC}不存在。");
                    return;
                }

                var trans = cnn.BeginTransaction();

                //更新设备状态
                cnn.Execute(
                    @"UPDATE Thermometer 
                    SET 
                        WiFiStrength = @WiFiStrength, 
                        SSID=@SSID, 
                        Temp=@Temp,
                        Power=@Power,
                        Charge=@Charge,
                        Battery=@Battery,
                        IPAddress=@IPAddress 
                    WHERE
                        MAC=@MAC;"
                    , device);

                //添加温度历史
                cnn.Execute(
                    @"INSERT INTO TemperatureHistory 
                        (MAC, Temp, Battery) 
                    VALUES 
                        ( @MAC, @Temp, @Battery);"
                    , device);

                trans.Commit();
            }
        }

        public void GetTemparetureHistory(string mac, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
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
                db.Execute(@"
                    CREATE TABLE Thermometer (
	                    ID integer PRIMARY KEY AUTOINCREMENT,
	                    SSID varchar ( 100 ) NOT NULL,
	                    WiFiStrength INTEGER,
	                    MAC varchar ( 100 ) NOT NULL,
	                    TEMP REAL,
	                    Power INTEGER,
	                    Charge INTEGER,
	                    Battery REAL,
	                    IPAddress varchar ( 100 ) NOT NULL,
	                    Firmware varchar ( 100 ) NOT NULL,
	                    CreatedDate datetime DEFAULT (
	                    datetime( 'now', 'localtime' )),
	                    LastUpdate datetime DEFAULT (
	                    datetime( 'now', 'localtime' )) 
                    );

                    CREATE TABLE TemperatureHistory (
	                    ID integer PRIMARY KEY AUTOINCREMENT,
	                    MAC varchar ( 100 ) NOT NULL,
	                    TEMP REAL,
	                    Battery REAL,
	                    CreatedDate datetime DEFAULT (
	                    datetime( 'now', 'localtime' )) 
                    );
                ");
            }
        }
    }
}
