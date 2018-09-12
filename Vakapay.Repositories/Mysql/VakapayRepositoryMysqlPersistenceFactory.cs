using System;
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
			Console.WriteLine("CONNECTION NULL= " + Connection == null);
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
	}
}