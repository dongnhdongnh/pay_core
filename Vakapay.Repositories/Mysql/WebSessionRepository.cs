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
    public class WebSessionRepository : MySqlBaseRepository<WebSession>, IWebSessionRepository
    {
        public WebSessionRepository(string connectionString) : base(connectionString)
        {
        }

        public WebSessionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public string QuerySearch(Dictionary<string, string> models)
        {
            {
                var sQuery = "SELECT * FROM " + TableName + " WHERE 1 = 1";
                foreach (var model in models)
                {
                    sQuery += string.Format(" AND {0}='{1}'", model.Key, model.Value);
                }

                return sQuery;
            }
        }

        public WebSession FindWhere(string sql)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.QuerySingle<WebSession>(sql);

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("WebSessionRepository =>> FindWhere fail: " + e.Message);
                return null;
            }
        }

        public List<WebSession> GetListWebSession(string sql, int skip, int take)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Query<WebSession>(sql).Skip(skip).Take(take).ToList();

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("WebSessionRepository =>> GetListWebSession fail: " + e.Message);
                return null;
            }
        }

        public int GetCount()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }

                var sQuery = "SELECT count(*) FROM " + TableName + ";";
                var count = Convert.ToInt32(Connection.Query<int>(sQuery));
                return count;
            }
            catch (Exception e)
            {
                Logger.Error("WebSessionRepository =>> GetCount fail: " + e.Message);
                return 0;
            }
        }
    }
}