using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.Server
{
    public class SqLiteBaseRepository
    {
        public SqLiteBaseRepository(IConfiguration config)
        {

        }

        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\db.sqlite"; }
        }

        public static SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DbFile);
        }
    }
}
