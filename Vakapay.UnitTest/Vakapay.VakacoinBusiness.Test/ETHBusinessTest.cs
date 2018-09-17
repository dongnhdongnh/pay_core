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

namespace Vakapay.VakacoinBusiness.Test
{
	[TestFixture]
	class ETHBusinessTest
	{
		const String ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
		Vakapay.EthereumBusiness.EthereumBusiness _ethBus;
		[Test]
		public void CreateNewAddress()
		{

			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = ETHBusinessTest.ConnectionString
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory);
			string walletID = CommonHelper.RandomString(15);
			var outPut = _ethBus.CreateNewAddAddress(walletID);
			Console.WriteLine(JsonHelper.SerializeObject(outPut));
			Assert.IsNotNull(outPut);
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
			var result = WalletBusiness.CreateNewWallet(user, blockChainNetwork);
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
		[Test]
		public void CreateNewTransactionMultirun()
		{
			
		}

		[Test]
		public void CreateNewTransaction()
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
			var outPut = _ethBus.RunSendTransaction();
			Console.WriteLine(JsonHelper.SerializeObject(outPut));
			Assert.IsNotNull(outPut);
		}

		[Test]
		public void TestScan()
		{

			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = ETHBusinessTest.ConnectionString
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			var WalletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
			_ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory);
			int block = _ethBus.ScanBlock(WalletBusiness);

			Assert.IsTrue(block > 0);
		}



	}
}
