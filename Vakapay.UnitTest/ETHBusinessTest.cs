using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
	[TestFixture]
	class ETHBusinessTest
	{
		EthereumRpc _rpcClass;
		EthereumRpc RPCClass
		{
			get
			{
				if (_rpcClass == null)
					_rpcClass = new EthereumRpc(RPCEndpoint);
				return _rpcClass;
			}
		}
		VakapayRepositoryMysqlPersistenceFactory _PersistenceFactory;
		VakapayRepositoryMysqlPersistenceFactory PersistenceFactory
		{
			get
			{
				if (_PersistenceFactory == null)
				{
					var repositoryConfig = new RepositoryConfiguration
					{
						ConnectionString = ETHBusinessTest.ConnectionString
					};
					Console.WriteLine("MAKE NEW");
					_PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
				}
				return _PersistenceFactory;

			}
			set
			{
				this._PersistenceFactory = value;
			}
		}
		const String RPCEndpoint = "http://localhost:9900";
		const String ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
		Vakapay.EthereumBusiness.EthereumBusiness _ethBus;
		[Test]
		public async System.Threading.Tasks.Task CreateNewAddressAsync()
		{
			Console.WriteLine("WTF");
			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = ETHBusinessTest.ConnectionString
			};
			Console.WriteLine("MAKE NEW");
			PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			var connection = PersistenceFactory.GetDbConnection();
			_ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory);
			var _walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
			var ethAddressRepos = PersistenceFactory.GetEthereumAddressRepository(connection);
			string walletID = CommonHelper.RandomString(15);
			string pass = CommonHelper.RandomString(15);
			var outPut = await _ethBus.CreateAddressAsyn<EthereumAddress>(_walletBusiness, ethAddressRepos, RPCClass, walletID, pass);
			Console.WriteLine(JsonHelper.SerializeObject(outPut));
			Assert.IsNotNull(outPut);
		}
		[Test]
		public void DeleteCache()
		{
			Assert.IsNotNull((String.Format(CacheHelper.CacheKey.KEY_SCANBLOCK_LASTSCANBLOCK, NetworkName.ETH)));
		}

		[Test]
		public void FakeWalletID()
		{
			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = ETHBusinessTest.ConnectionString
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			var WalletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
			var user = new User
			{
				Id = CommonHelper.GenerateUuid(),
			};
			var blockChainNetwork = new BlockchainNetwork
			{
				Name = NetworkName.ETH,
				Status = Status.StatusActive,
				Sysbol = "ETH",
				Id = CommonHelper.GenerateUuid()
			};
			var result = WalletBusiness.CreateNewWallet(user, NetworkName.ETH);
			Console.WriteLine(JsonHelper.SerializeObject(result));
			Assert.IsNotNull(result);
		}

		[TestCase(10)]
		public void FakePeningTransaction(int numOfTrans)
		{
			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = ETHBusinessTest.ConnectionString
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory);
			var _trans = new EthereumWithdrawTransaction()
			{
				FromAddress = "0x12890d2cce102216644c59dae5baed380d84830c",
				ToAddress = "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517",
				Amount = 10
			};
			ReturnObject outPut = null;
			for (int i = 0; i < numOfTrans; i++)
				outPut = _ethBus.FakePendingTransaction(_trans);
			Console.WriteLine(JsonHelper.SerializeObject(outPut));
			Assert.IsNotNull(outPut);
		}
		//[Test]
		//public void CreateNewTransactionMultirun()
		//{

		//}

		//[Test]
		//public void CreateNewTransaction()
		//{

		//	var repositoryConfig = new RepositoryConfiguration
		//	{
		//		ConnectionString = ETHBusinessTest.ConnectionString
		//	};

		//	var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
		//	var _ethBus = new Vakapay.EthereumBusiness.EthereumBusinessOld(PersistenceFactory, true, RPCEndpoint);
		//	var _trans = new EthereumWithdrawTransaction()
		//	{
		//		FromAddress = "0x12890d2cce102216644c59dae5baed380d84830c",
		//		ToAddress = "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517",
		//		Amount = 10
		//	};
		//	var outPut = _ethBus.RunSendTransaction();
		//	//var outPut = 1;
		//	Console.WriteLine(JsonHelper.SerializeObject(outPut));
		//	Assert.IsNotNull(outPut);
		//}

		//[Test]
		//public void TestScan()
		//{

		//	var repositoryConfig = new RepositoryConfiguration
		//	{
		//		ConnectionString = ETHBusinessTest.ConnectionString
		//	};

		//	var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
		//	var WalletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
		//	_ethBus = new Vakapay.EthereumBusiness.EthereumBusinessOld(PersistenceFactory);
		//	//	int block = _ethBus.ScanBlock(WalletBusiness);

		//	Assert.IsTrue(block > 0);
		//}



	}
}
