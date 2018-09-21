using System;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.WalletBusiness;

namespace Vakapay.ScanWallet
{
	class Program
	{
		const String ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
		}

		static void RunScan()
		{
			try
			{
				var repositoryConfig = new RepositoryConfiguration()
				{
					ConnectionString = ConnectionString
				};

				var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
				var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
				var connection = repoFactory.GetDbConnection();
				var _walletBusiness = new WalletBusiness.WalletBusiness(persistence);
				var _walletRepo = repoFactory.GetWalletRepository(connection);
				_walletRepo.FindNullAddress();
				//	get all address = null with same networkName of walletId
				while (true)
				{

				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				throw;
			}
		}
	}
}
