using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Vakapay.Repositories.Mysql.Base
{
    public class MysqlBaseConnection : IDisposable
    {
        public MySqlConnection Connection { get; }

        public MysqlBaseConnection(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public MysqlBaseConnection(IDbConnection dbConnection)
        {
            Connection = dbConnection as MySqlConnection;
        }
        
        public void Dispose()
        {
            if (Connection.State == ConnectionState.Open)
                Connection.Close();
            this.Connection?.Dispose();
        }
    }
}