using System.Data;
using MySql.Data.MySqlClient;
using Vakapay.Models.Repositories;

namespace Vakapay.Repositories.Mysql
{
    public class VakapayRepositoryMysqlPersistenceFactory : IVakapayRepositoryFactory
    {
        public RepositoryConfiguration repositoryConfiguration { get;}

        public VakapayRepositoryMysqlPersistenceFactory(RepositoryConfiguration _repositoryConfiguration)
        {
            repositoryConfiguration = _repositoryConfiguration;
        }
        public IApiKeyRepository getApiKeyRepository(IDbConnection connection)
        {
            return new ApiKeyRepository(connection);
        }

        public IDbConnection GetDbConnection()
        {
            return new MySqlConnection(repositoryConfiguration.ConnectionString);
        }

        public IWalletRepository GetWalletRepository(IDbConnection dbConnection)
        {
            throw new System.NotImplementedException();
        }

        public IUserRepository GetUserRepository(IDbConnection dbConnection)
        {
            throw new System.NotImplementedException();
        }
    }
}