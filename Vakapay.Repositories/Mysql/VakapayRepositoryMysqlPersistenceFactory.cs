using System.Data;
using MySql.Data.MySqlClient;
using Vakapay.Models.Repositories;

namespace Vakapay.Repositories.Mysql
{
	public class VakapayRepositoryMysqlPersistenceFactory : IVakapayRepositoryFactory
	{
		public RepositoryConfiguration repositoryConfiguration { get; }

		public IDbConnection Connection { get; set; }

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
			Connection = new MySqlConnection(repositoryConfiguration.ConnectionString);
			return Connection;
		}

		public IDbConnection GetOldConnection()
		{
			return Connection;
		}

		public IWalletRepository GetWalletRepository(IDbConnection dbConnection)
		{
			return new WalletRepository(dbConnection);
		}

		public IUserRepository GetUserRepository(IDbConnection dbConnection)
		{
			return new UserRepository(dbConnection);
		}

		public IEthereumAddressRepository GetEthereumAddressRepository(IDbConnection dbConnection)
		{
			return new EthereumAddressRepository(dbConnection);
		}

		public IBitcoinAddressRepository GetBitcoinAddressRepository(IDbConnection dbConnection)
		{
			return new BitcoinAddressRepository(dbConnection);
		}

		public IBitcoinRawTransactionRepository GetBitcoinRawTransactionRepository(IDbConnection dbConnection)
		{
			return new BitcoinRawTransactionRepository(dbConnection);
		}

		public IVakacoinAccountRepository GetVakacoinAccountRepository(IDbConnection dbConnection)
		{
			return new VakacoinAccountRepository(dbConnection);
		}

		public IEthereumWithdrawTransactionRepository GetEthereumWithdrawTransactionRepository(IDbConnection dbConnection)
		{
			return new EthereumWithdrawnTransactionRepository(dbConnection);
		}
		public IEthereumDepositTransactionRepository GetEthereumDepositeTransactionRepository(IDbConnection dbConnection)
		{
			return new EthereumDepositTransactionRepository(dbConnection);
		}


		public IVakacoinWithdrawTransactionRepository GetVakacoinWithdrawTransactionRepository(IDbConnection dbConnection)
		{
			return new VakacoinWithdrawTransactionRepository(dbConnection);
		}
		
		public IVakacoinDepositTransactionRepository GetVakacoinDepositTransactionRepository(IDbConnection dbConnection)
		{
			return new VakacoinDepositTransactionRepository(dbConnection);
		}
		public IBitcoinDepositTransactionRepository GetBitcoinDepositTransactionRepository(IDbConnection dbConnection)
		{
			return new BitcoinDepositTransactionRepository(dbConnection);
		}

		public ISendEmailRepository GetSendEmailRepository(IDbConnection dbConnection)
		{
			return new SendEmailRepository(dbConnection);
		}

		
	}
}