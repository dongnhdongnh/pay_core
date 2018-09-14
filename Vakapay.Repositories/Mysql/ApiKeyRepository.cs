using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class ApiKeyRepository : MysqlBaseConnection, IApiKeyRepository
    {
        public ApiKeyRepository(string connectionString) : base(connectionString)
        {
        }

        public ApiKeyRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(ApiKey objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(ApiKey objectInsert)
        {
            throw new System.NotImplementedException();
        }

        public ApiKey FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<ApiKey> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }
    }
}