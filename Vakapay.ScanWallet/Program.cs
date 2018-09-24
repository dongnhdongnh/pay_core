using System;
using System.Collections.Generic;
using System.Threading;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;
using Vakapay.WalletBusiness;

namespace Vakapay.ScanWallet
{
	class Program
	{
		const String RPCEndpoint = "http://localhost:9900";
		const String ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
		static void Main(string[] args)
		{
			RunScan();
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
				var ethAddressRepos = repoFactory.GetEthereumAddressRepository(connection);
				var btcAddressRepos = repoFactory.GetBitcoinAddressRepository(connection);
				var vakaAddressRepos = repoFactory.GetVakacoinAccountRepository(connection);

				var ethereumBusiness = new EthereumBusiness.EthereumBusiness(repoFactory);
				var bitcoinBusiness = new BitcoinBusiness.BitcoinBusiness(repoFactory);
				var vakaBusiness = new VakacoinBusiness.VakacoinBusiness(repoFactory);

				//	get all address = null with same networkName of walletId
				while (true)
				{
					Console.WriteLine("Scan wallet :START");
					List<Wallet> _walletNoAddress = _walletRepo.FindNullAddress();
					Console.WriteLine("Scan wallet :START with " + _walletNoAddress.Count);
					if (_walletNoAddress == null || _walletNoAddress.Count <= 0)
					{

					}
					else
					{
						string pass = CommonHelper.RandomString(15);
						foreach (Wallet wallet in _walletNoAddress)
						{
							switch (wallet.NetworkName)
							{
								case NetworkName.ETH:

									ethereumBusiness.CreateAddressAsyn<EthereumAddress>(ethAddressRepos, new EthereumRpc(RPCEndpoint), wallet.Id, pass);
									break;
								case NetworkName.BTC:

									bitcoinBusiness.CreateAddressAsyn<BitcoinAddress>(btcAddressRepos, new BitcoinRpc(RPCEndpoint), wallet.Id, pass);
									break;
								case NetworkName.VAKA:

									vakaBusiness.CreateAddressAsyn<VakacoinAccount>(vakaAddressRepos, new VakacoinRPC(RPCEndpoint), wallet.Id, pass);
									break;
								default:
									break;
							}
						}
					}
					Console.WriteLine("Scan wallet :END");
					Thread.Sleep(1000);

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
