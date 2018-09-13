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

		public IBitcoinRawTransactionRepository GeBitcoinRawTransactionRepository(IDbConnection dbConnection)
		{
			return new BitcoinRawTransactionRepository(dbConnection);
		}

		public IEthereumWithdrawnTransactionRepository GetEthereumWithdrawnTransactionRepository(IDbConnection dbConnection)
		{
			return new EthereumWithdrawnTransactionRepository(dbConnection);
		}

		public IVakacoinTransactionHistoryRepository GetVakacoinTransactionHistoryRepository(IDbConnection dbConnection)
		{
			return new VakacoinTransactionHistoryRepository(dbConnection);
		}
		public IBitcoinDepositTransactioRepository GetBitcoinDepositTransactioRepository(IDbConnection dbConnection)
		{
			return new BitcoinDepositTransactioRepository(dbConnection);
		}
	}
}