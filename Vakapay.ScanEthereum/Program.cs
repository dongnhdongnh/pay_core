using System;
using System.Threading;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ScanEthereum
{
	internal static class Program
	{
		private const string ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";

		private static void Main(string[] args)
		{
			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = ConnectionString
			};
			RunScan(repositoryConfig);
		}


		private static void RunScan(RepositoryConfiguration repositoryConfig)
		{

			var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

			var ethereumBusiness = new EthereumBusiness.EthereumBusiness(repoFactory);
			var WalletBusiness = new WalletBusiness.WalletBusiness(repoFactory);
			var connection = repoFactory.GetOldConnection() ?? repoFactory.GetDbConnection();
			try
			{
				while (true)
				{

					Console.WriteLine("==========Start Scan Ethereum==========");

					var rpc = new EthereumRpc("http://localhost:9900");

					var ethereumRepo = repoFactory.GetEthereumWithdrawTransactionRepository(connection);
					var ethereumDepoRepo = repoFactory.GetEthereumDepositeTransactionRepository(connection);
					var resultSend = ethereumBusiness.ScanBlockAsync<EthereumWithdrawTransaction, EthereumDepositTransaction, ETHEntities.ETHBlockInfor, ETHEntities.ETHTransaction>(NetworkName.ETH, WalletBusiness, ethereumRepo, ethereumDepoRepo, rpc);
					Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));


					Console.WriteLine("==========Scan Ethereum End==========");
					Console.WriteLine("==========Wait for next scan==========");
					Thread.Sleep(5000);
				}
			}
			catch (Exception e)
			{
				connection.Close();
				Console.WriteLine(e.ToString());
			}

		}
	}


}