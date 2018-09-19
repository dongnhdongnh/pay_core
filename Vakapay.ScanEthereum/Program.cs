using System;
using System.Threading;
using Vakapay.BlockchainBusiness;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ScanEthereum
{
	class Program
	{
		const String ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = ConnectionString
			};
			runScan(repositoryConfig);
		}


		static void runScan(RepositoryConfiguration repositoryConfig)
		{

			var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

			var ethereumBusiness = new EthereumBusinessNew(repoFactory);
			var WalletBusiness = new WalletBusiness.WalletBusiness(repoFactory);
			var connection = repoFactory.GetDbConnection();
			try
			{
				//while (true)
				{

					Console.WriteLine("Start Send Ethereum....");

					var rpc = new EthereumRpc("http://localhost:9900");

					var ethereumRepo = repoFactory.GetEthereumWithdrawTransactionRepository(connection);
					var resultSend = ethereumBusiness.ScanBlockAsyn<EthereumWithdrawTransaction, ETHBlockInfor>(NetworkName.ETH, WalletBusiness, ethereumRepo, rpc);
					Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));


					Console.WriteLine("Send Ethereum End...");
					Thread.Sleep(100000);
				}
			}
			catch (Exception e)
			{
				connection.Close();
				Console.WriteLine(e.ToString());
			}

		}
	}

	public class ETHBlockInfor : IBlockInfor
	{ }
}