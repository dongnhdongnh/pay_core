using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.SendEthereum
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				var startTime = DateTime.Now.Ticks;
				CacheHelper.DeleteCacheString("cache");
				var builder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("setting.json");
				IConfiguration Configuration = builder.Build();

				var connectionString = Configuration.GetConnectionString("DefaultConnection");
				var repositoryConfig = new RepositoryConfiguration
				{
					ConnectionString = connectionString
				};

				/*var connection = repoFactory.GetDbConnection();
                ///One thread
                while (true)
                {
                
                    Console.WriteLine("Start Send Ethereum....");
                    
                    var rpc = new EthereumRpc("http://localhost:8545");

                    var ethereumRepo = repoFactory.GetEthereumWithdrawTransactionRepository(connection);
                    var resultSend = ethereumBusiness.SendTransactionAsysn(ethereumRepo, rpc, "");
                    Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));
                
                    //connection.Close();
                    Console.WriteLine("Send Ethereum...");
                    Thread.Sleep(1000);
                }*/
				for (var i = 0; i < 10; i++)
				{
					var ts = new Thread(() => RunSend(repositoryConfig, startTime));
					ts.Start();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}





		}

		private static void RunSend(RepositoryConfiguration repositoryConfig, long startTime)
		{
			var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

			var ethereumBusiness = new EthereumBusiness.EthereumBusiness(repoFactory);
			var connection = repoFactory.GetDbConnection();
			try
			{
				while (true)
				{
					Console.WriteLine("Start Send Ethereum....");

					var rpc = new EthereumRpc("http://localhost:9900");

					var ethereumRepo = repoFactory.GetEthereumWithdrawTransactionRepository(connection);
					var resultSend = ethereumBusiness.SendTransactionAsync(ethereumRepo, rpc, "");
					Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));


					Console.WriteLine("Send Ethereum End...");
					Thread.Sleep(1000);
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
