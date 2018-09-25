using NUnit.Framework;
using System;
using System.Collections.Generic;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.WalletBusiness;

namespace Vakapay.UnitTest
{
	[TestFixture]
	class WalletBussinessTest
	{

		//const String ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
		Vakapay.WalletBusiness.WalletBusiness _walletBusiness;
		Vakapay.WalletBusiness.WalletBusiness WalletBusiness
		{
			get
			{
				if (_walletBusiness == null)
				{
					var repositoryConfig = new RepositoryConfiguration()
					{
						ConnectionString = VakapayConfig.ConnectionString
					};

					var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
					_walletBusiness =
							new Vakapay.WalletBusiness.WalletBusiness(persistence);
				}

				return _walletBusiness;

			}
		}

		Vakapay.EthereumBusiness.EthereumBusiness _ethBus;
		EthereumRpc _rpcClass;
		EthereumRpc RPCClass
		{
			get
			{
				if (_rpcClass == null)
					_rpcClass = new EthereumRpc(VakapayConfig.RPCEndpoint);
				return _rpcClass;
			}
		}

		[Test]
		public void CreateNewWallet()
		{
			var repositoryConfig = new RepositoryConfiguration()
			{
				ConnectionString = VakapayConfig.ConnectionString
			};

			var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_walletBusiness =
				new Vakapay.WalletBusiness.WalletBusiness(persistence);
			var user = new User();
			var blockChain = new BlockchainNetwork();
			user.Id = "8377a95b-79b4-4dfb-8e1e-b4833443c306";

			blockChain.Name = NetworkName.ETH;
			var resultTest = _walletBusiness.CreateNewWallet(user, blockChain.Name);
			Assert.AreEqual(Status.StatusSuccess, resultTest.Status);

			blockChain.Name = NetworkName.BTC;
			_walletBusiness.CreateNewWallet(user, blockChain.Name);
			Assert.AreEqual(Status.StatusSuccess, resultTest.Status);

			blockChain.Name = NetworkName.VAKA;
			_walletBusiness.CreateNewWallet(user, blockChain.Name);
			Assert.AreEqual(Status.StatusSuccess, resultTest.Status);

		}

		[Test]
		public async System.Threading.Tasks.Task CreateNewAddressAsync()
		{

			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = VakapayConfig.ConnectionString
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory, true);
			var connection = PersistenceFactory.GetDbConnection();
			var ethAddressRepos = PersistenceFactory.GetEthereumAddressRepository(connection);
			var _walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
			var wallet = new Wallet();
			wallet.Id = "29bb8133-d8d2-4b60-9882-60f0c1bd5f68";

			string pass = CommonHelper.RandomString(15);
			//	var resultTest = _ethBus.CreateNewAddAddress(wallet);
			var outPut = await _ethBus.CreateAddressAsyn<EthereumAddress>(_walletBusiness, ethAddressRepos, RPCClass, wallet.Id, pass);
			Assert.IsNotNull(outPut);
		}

		[Test]
		public void InsertPendingTxsToWithdraw()
		{
			var repositoryConfig = new RepositoryConfiguration()
			{
				ConnectionString = VakapayConfig.ConnectionString
			};

			var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_walletBusiness =
				new Vakapay.WalletBusiness.WalletBusiness(persistence);

			var wallet = new Wallet();
			wallet.Id = "64308d79-5523-4fd7-80a1-bba398b62c9b";
			var toAddr = "0x18cbb2afa209e5735122708a39e4715139f125d2";

			var resultTest = _walletBusiness.Withdraw(wallet, toAddr, 5);
			Console.WriteLine(JsonHelper.SerializeObject(resultTest));
			Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
		}

		[TestCase("64308d79-5523-4fd7-80a1-bba398b62c9b")]
		public void GetHistory(string walletID)
		{
			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = VakapayConfig.ConnectionString
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_walletBusiness =
					new Vakapay.WalletBusiness.WalletBusiness(PersistenceFactory);

			var wallet = _walletBusiness.GetWalletByID(walletID);
			if (wallet == null)
			{
				Console.WriteLine("wallet null");
			}
			else
				_walletBusiness.GetHistory(wallet, 1, 3, new string[] { nameof(BlockchainTransaction.Amount) });
		}

		//[Test]
		//public void SendTransaction_GetHash()
		//{

		//	var repositoryConfig = new RepositoryConfiguration
		//	{
		//		ConnectionString = WalletBussinessTest.ConnectionString
		//	};

		//	var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
		//	_ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory, true);

		//	var resultTest = _ethBus.RunSendTransaction();
		//	Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
		//}

		//[Test]
		//public void TestScan()
		//{
		//	var repositoryConfig = new RepositoryConfiguration
		//	{
		//		ConnectionString = WalletBussinessTest.ConnectionString
		//	};

		//	var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
		//	var walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);
		//	_ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(persistenceFactory, true, RPCEndpoint);
		//	int block = _ethBus.ScanBlock(walletBusiness);

		//	Assert.IsTrue(block > 0);
		//}
	}
}