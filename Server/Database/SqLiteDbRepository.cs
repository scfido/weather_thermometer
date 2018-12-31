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
                    CreateDatabase().Wait();
                    logger.LogInformation($"数据库创建成功。");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"数据库创建失败。");
                }
            }
        }

        public async Task<IList<Thermometer>> GetThermometers(string openId)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = await cnn.QueryAsync<Thermometer>(
                    @"SELECT Id, SSID, Name, WiFiStrength, Sn, Key, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware, OpenId
                    FROM Thermometer
                    WHERE OpenId = @openId
                    ", new { openId });

                return result.ToList();
            }
        }

        public async Task<Thermometer> GetThermometer(string openId, int id)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = await cnn.QueryAsync<Thermometer>(
                    @"SELECT Id, SSID, Name, WiFiStrength, Sn, Key, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware, OpenId
                    FROM Thermometer
                    WHERE
                        OpenId = @openId AND Id = @id"
                    , new { openId, id });

                return result.FirstOrDefault();
            }
        }

        public async Task<Thermometer> GetThermometer(string sn)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = await cnn.QueryAsync<Thermometer>(
                    @"SELECT Id, SSID, Name, WiFiStrength, Sn, Key, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware, OpenId
                    FROM Thermometer
                    WHERE
                        Sn = @sn"
                    , new { sn });

                return result.FirstOrDefault();
            }
        }


        public async Task<Thermometer> UpdateThermometer(string openId, int id, string name)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = await cnn.QueryAsync<Thermometer>(
                    @"
                    UPDATE Thermometer
                    SET
                        Name = @name
                    WHERE
                         OpenID = @openId AND
                         Id = @id;

                    SELECT Id, Name, SSID, WiFiStrength, Sn, Key, Temp, Power, Charge, Battery, LastUpdate, IPAddress, Firmware, OpenId
                    FROM Thermometer
                    WHERE
                        OpenId = @openId AND Id = @id"
                    , new { openId, id, name });
                return result.FirstOrDefault();
            }
        }

        public async Task<Thermometer> AddThermometer(string openId, string sn, string name)
        {
            Thermometer device = new Thermometer(openId, sn, name);
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                device.Id = (await cnn.QueryAsync<int>(
                    @"INSERT INTO Thermometer 
                        (Sn, OpenId, Name) 
                    VALUES 
                        (  @Sn, @OpenId ,@name);
                    select last_insert_rowid()", device))
                    .First();
            }

            return device;
        }

        public async Task RemoveThermometer(string openId, int id)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var trans = cnn.BeginTransaction();
                int rows = await cnn.ExecuteAsync(@"DELETE FROM Thermometer WHERE OpenID=@openId AND ID=@id", new { openId, id });
                if (rows > 0)
                    await cnn.ExecuteAsync(@"DELETE FROM TemperatureHistory WHERE ThermometerID=@id", new { openId, id });
                trans.Commit();
            }
        }

        public async Task SaveThermometerStatus(Thermometer device)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();

                bool exist = (await cnn.QueryAsync<int>(
                    @"SELECT count(*) FROM Thermometer WHERE OpenID=@openId AND Sn=@Sn",
                    device
                ))
                .Single() > 0;

                if (!exist)
                {
                    logger.LogWarning($"来自{device.IPAddress}的设备{device.Sn}不存在。");
                    return;
                }

                var trans = cnn.BeginTransaction();

                //更新设备状态
                await cnn.ExecuteAsync(
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
                        Sn=@Sn;"
                    , device);

                //添加温度历史
                await cnn.ExecuteAsync(
                    @"INSERT INTO TemperatureHistory 
                        (ThermometerID, Sn, Key, Temp, Battery, IPAddress) 
                    VALUES 
                        (@ID, @Sn, @Key, @Temp, @Battery, @IPAddress);"
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
        public async Task<IList<TemparetureHistoryData>> GetTemparetureHistory(string openId, int id, DateTime start, DateTime end)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = await cnn.QueryAsync<TemparetureHistoryData>(
                    @"SELECT Time, WiFiStrength, Temp, Battery
                    FROM TemperatureHistory
                    WHERE
                        OpenId = @openId AND
                        Id = @id AND
                        Time >= @Start AND
                        Time <= end
                    "
                    , new { openId, id });
                return result.ToList();
            }
        }

        public async Task<User> GetUser(string openId)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = await cnn.QueryAsync<User>(
                    @"SELECT *
                    FROM User
                    WHERE OpenId = @openId
                    ", new { openId });

                return result.FirstOrDefault();
            }
        }

        public async Task<User> AddUser(User user)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                user.Id = (await cnn.QueryAsync<int>(
                    @"INSERT INTO User 
                        ( OpenId, Session, WeChatSessioKey, IPAddress) 
                    VALUES 
                        ( @OpenId, @Session, @WeChatSessioKey, @IPAddress);
                    select last_insert_rowid()", user)).First();
            }

            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                var result = await cnn.QueryAsync<User>(
                    @"
                    UPDATE User
                    SET
                        Session = @Session,
                        WeChatSessioKey = @WeChatSessioKey,
                        IPAddress = @IPAddress,
                        LastLogin = @LastLogin
                    WHERE
                         OpenID = @openId; 

                    SELECT *
                    FROM User
                    WHERE
                        OpenId = @openId"
                    , user);

                return result.FirstOrDefault();
            }
        }

        public async Task RemoveUser(string openId)
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                await cnn.ExecuteAsync(@"DELETE FROM User WHERE OpenID=@openId", new { openId });
            }
        }

        private async Task CreateDatabase()
        {
            //创建数据库文件夹
            var dbDir = Path.GetDirectoryName(DbFile);
            if (!Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);

            using (var db = SimpleDbConnection())
            {
                db.Open();
                await db.ExecuteAsync(@"
                    CREATE TABLE Thermometer (
	                    ID integer PRIMARY KEY AUTOINCREMENT,
	                    SSID varchar ( 100 ),
	                    WiFiStrength INTEGER,
	                    Sn varchar ( 100 ) NOT NULL,
	                    Key varchar ( 100 ) NOT NULL,
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
 
                    CREATE TABLE User (
	                    ID integer PRIMARY KEY AUTOINCREMENT,
                        OpenId varchar ( 50 ) NOT NULL,
	                    Session varchar ( 50 ),
	                    WeChatSessioKey varchar ( 50 ) NOT NULL,
	                    IPAddress varchar ( 20 ),
	                    LastLogin datetime DEFAULT (
	                    datetime( 'now', 'localtime' )) 
                    );
               ");
            }
        }
    }
}
