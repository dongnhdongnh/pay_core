using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class ApiKeyRepository : MysqlBaseConnection<ApiKeyRepository>, IApiKeyRepository
    {
        public ApiKey FindApiKeyById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<ApiKey> FindApiKeyByUser(string UserId)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateNewApiKey(ApiKey apiKey)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteApiKey(string Id)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateApiKey(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ApiKeyRepository(string connectionString) : base(connectionString)
        {
        }

        public ApiKeyRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }
    }
}