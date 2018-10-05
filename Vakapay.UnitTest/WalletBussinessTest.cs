using NUnit.Framework;
using System;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

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
			wallet.Id = "46b4594c-a45a-400d-86ce-9a7869d61180";

			string pass = CommonHelper.RandomString(15);
			//	var resultTest = _ethBus.CreateNewAddAddress(wallet);
			var outPut = await _ethBus.CreateAddressAsync<EthereumAddress>(_walletBusiness, ethAddressRepos, RPCClass, wallet.Id, pass);
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
			wallet.Id = "46b4594c-a45a-400d-86ce-9a7869d61180";
			var toAddr = "0x13f022d72158410433cbd66f5dd8bf6d2d129924";

			ReturnObject resultTest = null;
			for (int i = 0; i < 1000; i++)
			{
				 resultTest = _walletBusiness.Withdraw(wallet, toAddr, 5);
			}
			
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