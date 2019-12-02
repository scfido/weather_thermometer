using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeatherStation.Server
{
    public class SqLiteBaseRepository
    {
        IConfiguration config;
        string connectString;
        string dbFile;

        public SqLiteBaseRepository(IConfiguration config)
        {
            this.config = config;
            connectString = config.GetConnectionString("SqLite");
            var match = Regex.Match(connectString, @"Data Source=(.+\.sqlite)");
            if (match.Success)
                dbFile = Path.GetRelativePath(Environment.CurrentDirectory, match.Groups[1].Value.Trim());
        }

        public string DbFile => dbFile;


        public SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection(connectString);
        }
    }
}
