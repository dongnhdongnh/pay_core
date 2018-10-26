using System;
using NUnit.Framework;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    [TestFixture]
    public class BTCBusinessTest
    {
        BitcoinRpc _btcRpc;

        BitcoinRpc RpcClass
        {
            get
            {
                if (_btcRpc == null)
                    _btcRpc = new BitcoinRpc(AppSettingHelper.GetBitcoinNode(),
                        AppSettingHelper.GetBitcoinRpcAuthentication());
                return _btcRpc;
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
                    Console.WriteLine("New Connect");
                    _persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                }

                return _persistenceFactory;
            }
            set { _persistenceFactory = value; }
        }

        Vakapay.BitcoinBusiness.BitcoinBusiness _btcBus;

        [Test]
        public async System.Threading.Tasks.Task CreateNewAddressAsync()
        {
            Console.WriteLine("start");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };
            Console.WriteLine("New Address");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var connection = PersistenceFactory.GetDbConnection();
            _btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(PersistenceFactory);
            var bitcoinRepo = PersistenceFactory.GetBitcoinAddressRepository(connection);
            string walletId = CommonHelper.RandomString(15);
            var resultCreated =
                await _btcBus.CreateAddressAsync<BitcoinAddress>(bitcoinRepo, RpcClass, walletId);
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);
        }

        [TestCase(25000)]
        public void FakePeningTransaction(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(persistenceFactory);
            var trans = new BitcoinWithdrawTransaction
            {
                ToAddress = "2Muk22rW4opjTd18KA48bzHUqiG19ZUJDLb",
                Amount = (decimal)0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = _btcBus.FakePendingTransaction(trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }

        [TestCase(25000)]
        public void FakePeningTransaction1(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(persistenceFactory);
            var trans = new BitcoinWithdrawTransaction
            {
                ToAddress = "2NBRLqwA5NGfXtMkmU82aveKxVHLVPNewpG",
                Amount = (decimal)0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = _btcBus.FakePendingTransaction(trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }

        [TestCase(25000)]
        public void FakePeningTransaction2(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(persistenceFactory);
            var trans = new BitcoinWithdrawTransaction
            {
                ToAddress = "2MtXuJo6U69RP2otQuAiw2bKQmZEgiAHVJE",
                Amount = (decimal)0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = _btcBus.FakePendingTransaction(trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }

        [TestCase(25000)]
        public void FakePeningTransaction3(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(persistenceFactory);
            var trans = new BitcoinWithdrawTransaction
            {
                ToAddress = "2NEQBQ2JU1gezBaJg5Lwq69Pr8c65y7TEbv",
                Amount = (decimal)0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = _btcBus.FakePendingTransaction(trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }
    }
}