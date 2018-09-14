using System.Data;

namespace Vakapay.Models.Repositories
{
	public interface IVakapayRepositoryFactory
	{
		IApiKeyRepository getApiKeyRepository(IDbConnection dbConnection);
		IDbConnection GetDbConnection();
		IDbConnection GetOldConnection();
		IWalletRepository GetWalletRepository(IDbConnection dbConnection);
		IUserRepository GetUserRepository(IDbConnection dbConnection);
		IEthereumAddressRepository GetEthereumAddressRepository(IDbConnection dbConnection);
		IEthereumWithdrawTransactionRepository GetEthereumWithdrawTransactionRepository(IDbConnection dbConnection);
		IBitcoinAddressRepository GetBitcoinAddressRepository(IDbConnection dbConnection);
		IBitcoinRawTransactionRepository GeBitcoinRawTransactionRepository(IDbConnection dbConnection);
		IVakacoinTransactionHistoryRepository GetVakacoinTransactionHistoryRepository(IDbConnection dbConnection);
		IBitcoinDepositTransactioRepository GetBitcoinDepositTransactioRepository(IDbConnection dbConnection);
	}
}