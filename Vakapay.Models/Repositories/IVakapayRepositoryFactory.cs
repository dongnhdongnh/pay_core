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
		IEthereumDepositTransactionRepository GetEthereumDepositeTransactionRepository(IDbConnection dbConnection);
		IBitcoinAddressRepository GetBitcoinAddressRepository(IDbConnection dbConnection);
		IBitcoinRawTransactionRepository GetBitcoinRawTransactionRepository(IDbConnection dbConnection);
		IVakacoinAccountRepository GetVakacoinAccountRepository(IDbConnection dbConnection);
		IVakacoinDepositTransactionRepository GetVakacoinDepositTransactionRepository(IDbConnection dbConnection);
		IVakacoinWithdrawTransactionRepository GetVakacoinWithdrawTransactionRepository(IDbConnection dbConnection);
		IBitcoinDepositTransactionRepository GetBitcoinDepositTransactionRepository(IDbConnection dbConnection);
		IEmailRepository GetEmailRepository(IDbConnection dbConnection);
	}
}