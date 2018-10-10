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

		[TestCase("8377a95b-79b4-4dfb-8e1e-b4833443c306")]
		[TestCase("8377a95b-79b4-4dfb-8e1e-b4833443c307")]
		public void CreateAllWalletForUser(string userID)
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
			user.Id = userID;

			//blockChain.Name = NetworkName.ETH;
			//var resultTest = _walletBusiness.CreateNewWallet(user, blockChain.Name);
			//Assert.AreEqual(Status.StatusSuccess, resultTest.Status);

			//blockChain.Name = NetworkName.BTC;
			//_walletBusiness.CreateNewWallet(user, blockChain.Name);
			//Assert.AreEqual(Status.StatusSuccess, resultTest.Status);

			//blockChain.Name = NetworkName.VAKA;
			//_walletBusiness.CreateNewWallet(user, blockChain.Name);
			var resultTest = _walletBusiness.MakeAllWalletForNewUser(user);
			Console.WriteLine(JsonHelper.SerializeObject(resultTest));
			Assert.AreEqual(Status.StatusSuccess, resultTest.Status);

		}

		[TestCase("46b4594c-a45a-400d-86ce-9a7869d61180")]
		public async System.Threading.Tasks.Task CreateNewAddressAsync(string walletId)
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

			string pass = "password";
			//	var resultTest = _ethBus.CreateNewAddAddress(wallet);
			var outPut = await _ethBus.CreateAddressAsync<EthereumAddress>(_walletBusiness, ethAddressRepos, RPCClass, walletId, pass);
			Assert.IsNotNull(outPut);
		}
		
		[Test]
		public void InitFakeAddressAndCoin()
		{
//			var rpc = new EthereumRpc("http://localhost:8545");
//			var a = rpc.CreateNewAddress("password");
			
			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = VakapayConfig.ConnectionString
			};

			//Create user active for test
			var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			var connection = persistenceFactory.GetDbConnection();
			var userRepo = persistenceFactory.GetUserRepository(connection);
			var walletRepo = persistenceFactory.GetWalletRepository(connection);


			//fake userId
			for (int i = 1; i < 10; i++)
			{
				var ins = userRepo.Insert(
					new User()
					{
						Id = i.ToString(),
						Status =  "Active"
					}
				);
				Assert.AreEqual(Status.StatusSuccess, ins.Status);
			}

			//create wallet without address
			for (int i = 1; i < 10; i++)
			{
				CreateAllWalletForUser(i.ToString());
			}

			//insert address into WalletDb 
			// and insert to ethereumAddress
			for (int i = 1; i < 10; i++)
			{
				var prepareWallet = walletRepo.FindByUserAndNetwork(i.ToString(), NetworkName.ETH);
				CreateNewAddressAsync(prepareWallet.Id);
			}

			//send coin from rootAddress to new address
			for (int i = 1; i < 10; i++)
			{
				var prepareWallet = walletRepo.FindByUserAndNetwork(i.ToString(), NetworkName.ETH);
				InsertPendingTxsToWithdraw("46b4594c-a45a-400d-86ce-9a7869d61180", prepareWallet.Address);
			}
		}
		
		[Test]
		public void CreateRadomPendingTxsToWithdraw()
		{
			var repositoryConfig = new RepositoryConfiguration()
			{
				ConnectionString = VakapayConfig.ConnectionString
			};

			var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_walletBusiness =
				new Vakapay.WalletBusiness.WalletBusiness(persistence);


			var connection = persistence.GetDbConnection();
			var userRepo = persistence.GetUserRepository(connection);
			var walletRepo = persistence.GetWalletRepository(connection);

			ReturnObject resultTest = null;
			for (int i = 0; i < 1000; i++)
			{
				var rndFrom = new Random().Next(1, 10);
				var rndTo = new Random().Next(1, 10);
				var fromWallet = walletRepo.FindByUserAndNetwork(rndFrom.ToString(), NetworkName.ETH);

				while (rndFrom == rndTo)
				{
					rndTo = new Random().Next(1, 10);
				}
				var toWalletAddr =  walletRepo.FindByUserAndNetwork(rndTo.ToString(), NetworkName.ETH);
				resultTest = _walletBusiness.Withdraw(fromWallet, toWalletAddr.Address, 1);
			}
			
			Console.WriteLine(JsonHelper.SerializeObject(resultTest));
			Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
		}

		
		//rootAddress
		[TestCase("46b4594c-a45a-400d-86ce-9a7869d61180", "0x13f022d72158410433cbd66f5dd8bf6d2d129924")]
		public void InsertPendingTxsToWithdraw(string walletId, string toAddr)
		{
			var repositoryConfig = new RepositoryConfiguration()
			{
				ConnectionString = VakapayConfig.ConnectionString
			};

			var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_walletBusiness =
				new Vakapay.WalletBusiness.WalletBusiness(persistence);

			var wallet = new Wallet();
			wallet.Id = walletId;

			ReturnObject resultTest = null;
			
			resultTest = _walletBusiness.Withdraw(wallet, toAddr, 1000000000000000000000m);
			
			
			Console.WriteLine(JsonHelper.SerializeObject(resultTest));
			Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
		}

		[TestCase("8abc6056-9c81-4b6e-bb22-81f0ab0e0a28")]
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