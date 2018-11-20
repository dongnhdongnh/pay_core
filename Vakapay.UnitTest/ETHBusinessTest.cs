using NUnit.Framework;
using System;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    [TestFixture]
    class ETHBusinessTest
    {
        EthereumRpc _rpcClass;

        EthereumRpc RpcClass
        {
            get
            {
                if (_rpcClass == null)
                    _rpcClass = new EthereumRpc(RPCEndpoint);
                return _rpcClass;
            }
        }

        VakapayRepositoryMysqlPersistenceFactory _persistenceFactory;

        VakapayRepositoryMysqlPersistenceFactory PersistenceFactory
        {
            get
            {
                if (_persistenceFactory == null)
                {
                    var repositoryConfig = new RepositoryConfiguration
                    {
                        ConnectionString = AppSettingHelper.GetDbConnection()
                    };
                    Console.WriteLine("MAKE NEW");
                    _persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                }

                return _persistenceFactory;
            }
            set { _persistenceFactory = value; }
        }

        const String RPCEndpoint = "http://localhost:9900";
        Vakapay.EthereumBusiness.EthereumBusiness _ethBus;

        [Test]
        public async System.Threading.Tasks.Task CreateNewAddressAsync()
        {
            Console.WriteLine("WTF");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };
            Console.WriteLine("MAKE NEW");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var connection = PersistenceFactory.GetDbConnection();
            _ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory);
          using(  var ethAddressRepos = PersistenceFactory.GetEthereumAddressRepository(connection);)
            {
                string walletId = CommonHelper.RandomString(15);
                string pass = CommonHelper.RandomString(15);
                var outPut = await _ethBus.CreateAddressAsync<EthereumAddress>(ethAddressRepos, RpcClass, walletId, pass);
                Console.WriteLine(JsonHelper.SerializeObject(outPut));
                Assert.IsNotNull(outPut);
            }
        }

        [Test]
        public void DeleteCache()
        {
            Assert.IsNotNull((String.Format(RedisCacheKey.KEY_SCANBLOCK_LASTSCANBLOCK, CryptoCurrency.ETH)));
        }

        [Test]
        public void FakeWalletID()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);
            var user = new User
            {
                Id = CommonHelper.GenerateUuid(),
            };
            var result = walletBusiness.CreateNewWallet(user, CryptoCurrency.ETH);
            Console.WriteLine(JsonHelper.SerializeObject(result));
            Assert.IsNotNull(result);
        }

        [TestCase(10)]
        public void FakePendingTransaction(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(persistenceFactory);
            var trans = new EthereumWithdrawTransaction()
            {
                UserId = "8377a95b-79b4-4dfb-8e1e-b4833443c306",
                FromAddress = "0x12890d2cce102216644c59dae5baed380d84830c",
                ToAddress = "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517",
                Amount = 10
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = _ethBus.FakePendingTransaction(trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }

        [TestCase(10)]
        public void FakeDeposite(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(persistenceFactory);
            var trans = new EthereumDepositTransaction()
            {
                UserId = "8377a95b-79b4-4dfb-8e1e-b4833443c306",
                ToAddress = "0x12890d2cce102216644c59dae5baed380d84830c",
                FromAddress = "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517",
                Amount = 10
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = _ethBus.FakeDepositTransaction(trans);
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