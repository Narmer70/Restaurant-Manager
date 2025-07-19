using System;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace PadTai.Classes
{
    internal class DatabaseConnection
    {
        private static string sqliteConnectionString = "Data Source=Localdatabase.db; Version=3; FailIfMissing=True; BusyTimeout=3000;";

        public static string GetSQLiteConnectionString()
        {
            return sqliteConnectionString;
        }
    }
}
