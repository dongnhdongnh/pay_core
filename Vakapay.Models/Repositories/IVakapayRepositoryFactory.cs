using System.Data;

namespace Vakapay.Models.Repositories
{
    public interface IVakapayRepositoryFactory
    {
        IApiKeyRepository getApiKeyRepository(IDbConnection dbConnection);
        IDbConnection GetDbConnection();
        IWalletRepository GetWalletRepository(IDbConnection dbConnection);
        IUserRepository GetUserRepository(IDbConnection dbConnection);
    }
}