using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class UserActionLogRepository : MySqlBaseRepository<UserActionLog>, IUserActionLogRepository
    {
        private string TableNameWallet { get; }
        private string TableNameBitcoinAddress { get; }

        public UserActionLogRepository(string connectionString) : base(connectionString)
        {
            TableNameWallet = SimpleCRUD.GetTableName(typeof(Wallet));
            TableNameBitcoinAddress = SimpleCRUD.GetTableName(typeof(BitcoinAddress));
        }

        public UserActionLogRepository(IDbConnection dbConnection) : base(dbConnection)
        {
            TableNameWallet = SimpleCRUD.GetTableName(typeof(Wallet));
            TableNameBitcoinAddress = SimpleCRUD.GetTableName(typeof(BitcoinAddress));
        }

        public string QuerySearch(Dictionary<string, string> models)
        {
            var sQuery = "SELECT * FROM " + TableName + " WHERE 1 = 1";
            foreach (var model in models)
            {
                sQuery += string.Format(" AND {0}='{1}'", model.Key, model.Value);
            }

            return sQuery;
        }

        public UserActionLog FindWhere(string sql)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.QuerySingle<UserActionLog>(sql);

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindWhere fail: " + e.Message);
                return null;
            }
        }

    }
}