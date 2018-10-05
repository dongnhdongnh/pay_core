using System.Data;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class ApiKeyRepository : MySqlBaseRepository<ApiKey>, IApiKeyRepository
    {
        public ApiKeyRepository(string connectionString) : base(connectionString)
        {
        }

        public ApiKeyRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }
    }
}