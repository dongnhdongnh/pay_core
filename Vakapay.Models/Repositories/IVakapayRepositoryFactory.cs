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
		IUserActionLogRepository GetUserActionLogRepository(IDbConnection dbConnection);
		IWebSessionRepository GetWebSessionRepository(IDbConnection dbConnection);
		IApiKeyRepository GetApiKeyRepository(IDbConnection dbConnection);
		IConfirmedDevicesRepository GetConfirmedDevicesRepository(IDbConnection dbConnection);
		IEthereumAddressRepository GetEthereumAddressRepository(IDbConnection dbConnection);
		IEthereumWithdrawTransactionRepository GetEthereumWithdrawTransactionRepository(IDbConnection dbConnection);
		IEthereumDepositTransactionRepository GetEthereumDepositeTransactionRepository(IDbConnection dbConnection);
		IBitcoinAddressRepository GetBitcoinAddressRepository(IDbConnection dbConnection);
		IBitcoinWithdrawTransactionRepository GetBitcoinWithdrawTransactionRepository(IDbConnection dbConnection);
		IVakacoinAccountRepository GetVakacoinAccountRepository(IDbConnection dbConnection);
		IVakacoinDepositTransactionRepository GetVakacoinDepositTransactionRepository(IDbConnection dbConnection);
		IVakacoinWithdrawTransactionRepository GetVakacoinWithdrawTransactionRepository(IDbConnection dbConnection);
		IBitcoinDepositTransactionRepository GetBitcoinDepositTransactionRepository(IDbConnection dbConnection);
		ISendEmailRepository GetSendEmailRepository(IDbConnection dbConnection);
		ISendSmsRepository GetSendSmsRepository(IDbConnection dbConnection);
	}
}