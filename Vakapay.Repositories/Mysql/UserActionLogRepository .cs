using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class UserActionLogRepository : MySqlBaseRepository<UserActionLog>, IUserActionLogRepository
    {
        public UserActionLogRepository(string connectionString) : base(connectionString)
        {
        }

        public UserActionLogRepository(IDbConnection dbConnection) : base(dbConnection)
        {
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


        public List<UserActionLog> GetListLog(string sql, int skip = 0, int take = 10)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Query<UserActionLog>(sql).Skip(skip).Take(take).ToList();

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> GetListLog fail: " + e.Message);
                return null;
            }
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