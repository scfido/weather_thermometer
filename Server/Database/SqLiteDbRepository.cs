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

        public IList<Thermometer> GetThermometers(string openId)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = cnn.Query<Thermometer>(
                    @"SELECT Id, SSID, Name, WiFiStrength, MAC, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware, OpenId
                    FROM Thermometer
                    WHERE OpenId = @openId
                    ", new { openId }).ToList();

                return result;
            }
        }

        public Thermometer GetThermometer(string openId, int id)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = cnn.Query<Thermometer>(
                    @"SELECT Id, SSID, Name, WiFiStrength, MAC, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware, OpenId
                    FROM Thermometer
                    WHERE
                        OpenId = @openId AND Id = @id"
                    , new { openId, id }).FirstOrDefault();
                return result;
            }
        }

        public Thermometer UpdateThermometer(string openId, int id, string name)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = cnn.Query<Thermometer>(
                    @"
                    UPDATE Thermometer
                    SET
                        Name = @name
                    WHERE
                         OpenID = @openId AND
                         Id = @id;

                    SELECT Id, Name, SSID, WiFiStrength, MAC, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware, OpenId
                    FROM Thermometer
                    WHERE
                        OpenId = @openId AND Id = @id"
                    , new { openId, id, name }).FirstOrDefault();
                return result;
            }
        }

        public Thermometer AddThermometer(string openId, string mac, string name)
        {
            Thermometer device = new Thermometer(openId, mac, name);
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                device.Id = cnn.Query<int>(
                    @"INSERT INTO Thermometer 
                        (MAC, OpenId, Name) 
                    VALUES 
                        (  @MAC, @OpenId ,@name);
                    select last_insert_rowid()", device)
                    .First();
            }

            return device;
        }

        public void RemoveThermometer(string openId, int id)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var trans = cnn.BeginTransaction();
                int rows = cnn.Execute(@"DELETE FROM Thermometer WHERE OpenID=@openId AND ID=@id", new { openId, id });
                if(rows > 0)
                    cnn.Execute(@"DELETE FROM TemperatureHistory WHERE ThermometerID=@id", new { openId, id });
                trans.Commit();
            }
        }

        public void SaveThermometerStatus(Thermometer device)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();

                bool exist = cnn.Query<int>(
                    @"SELECT count(*) FROM Thermometer WHERE OpenID=@openId AND MAC=@MAC",
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
                         OpenID=@openId AND
                        MAC=@MAC;"
                    , device);

                //添加温度历史
                cnn.Execute(
                    @"INSERT INTO TemperatureHistory 
                        (ThermometerID, MAC, Temp, Battery, IPAddress) 
                    VALUES 
                        (@ID, @MAC, @Temp, @Battery, @IPAddress);"
                    , device);

                trans.Commit();
            }
        }

        /// <summary>
        /// 获取温度计历史数据
        /// </summary>
        /// <param name="id">温度计ID</param>
        /// <param name="start">历史数据开始日期</param>
        /// <param name="end">历史数据结束日期</param>
        public IList<TemparetureHistoryData> GetTemparetureHistory(string openId, int id, DateTime start, DateTime end)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = cnn.Query<TemparetureHistoryData>(
                    @"SELECT Time, WiFiStrength, Temp, Battery
                    FROM TemperatureHistory
                    WHERE
                        OpenId = @openId AND
                        Id = @id AND
                        Time >= @Start AND
                        Time <= end
                    "
                    , new { openId, id }).ToList();
                return result;
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
                db.Execute(@"
                    CREATE TABLE Thermometer (
	                    ID integer PRIMARY KEY AUTOINCREMENT,
	                    SSID varchar ( 100 ),
	                    WiFiStrength INTEGER,
	                    MAC varchar ( 100 ) NOT NULL,
	                    Name varchar ( 100 ) NOT NULL,
	                    Temp REAL,
	                    Power INTEGER,
	                    Charge INTEGER,
	                    Battery REAL,
	                    IPAddress varchar ( 20 ),
	                    Firmware varchar ( 100 ),
	                    CreatedDate datetime DEFAULT (
	                    datetime( 'now', 'localtime' )),
	                    LastUpdate datetime,
	                    OpenId varchar ( 100 ) NOT NULL 
                    );

                    CREATE TABLE TemperatureHistory (
	                    ID integer PRIMARY KEY AUTOINCREMENT,
                        ThermometerID integer,
	                    Temp REAL,
	                    Battery REAL,
	                    IPAddress varchar ( 20 ),
	                    CreatedDate datetime DEFAULT (
	                    datetime( 'now', 'localtime' )) 
                    );
                ");
            }
        }
    }
}
