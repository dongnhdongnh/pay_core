using System;
using NUnit.Framework;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    [TestFixture]
    public class BTCBusinessTest
    {
        BitcoinRpc btcRpc;

        BitcoinRpc RPCClass
        {
            get
            {
                if (btcRpc == null)
                    btcRpc = new BitcoinRpc(RPCEndpoint, RpcUser, RpcPassword);
                return btcRpc;
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
                        ConnectionString = BTCBusinessTest.ConnectionString
                    };
                    Console.WriteLine("New Connect");
                    _PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                }

                return _PersistenceFactory;
            }
            set { this._PersistenceFactory = value; }
        }

        const String RPCEndpoint = "http://localhost:18443";
        const String RpcUser = "bitcoinrpc";
        const String RpcPassword = "7bLjxV1CKhNJmdxTUMxTpF4vEemWCp49kMX9CwvZabYi";

        const String ConnectionString =
            "server=localhost;userid=root;password=huan@123;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";

        Vakapay.BitcoinBusiness.BitcoinBusiness btcBus;

        [Test]
        public async System.Threading.Tasks.Task CreateNewAddressAsync()
        {
            Console.WriteLine("start");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = BTCBusinessTest.ConnectionString
            };
            Console.WriteLine("New Address");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var connection = PersistenceFactory.GetDbConnection();
            btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(PersistenceFactory);
            var walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
            var bitcoinRepo = PersistenceFactory.GetBitcoinAddressRepository(connection);
            string walletID = CommonHelper.RandomString(15);
            var resultCreated =
                await btcBus.CreateAddressAsync<BitcoinAddress>(walletBusiness, bitcoinRepo, RPCClass, walletID);
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);
        }

        [TestCase(25000)]
        public void FakePeningTransaction(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = BTCBusinessTest.ConnectionString
            };

            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(PersistenceFactory);
            var _trans = new BitcoinWithdrawTransaction
            {
                ToAddress = "2Muk22rW4opjTd18KA48bzHUqiG19ZUJDLb",
                Amount = (decimal) 0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = btcBus.FakePendingTransaction(_trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }

        [TestCase(25000)]
        public void FakePeningTransaction1(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = BTCBusinessTest.ConnectionString
            };

            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(PersistenceFactory);
            var _trans = new BitcoinWithdrawTransaction
            {
                ToAddress = "2NBRLqwA5NGfXtMkmU82aveKxVHLVPNewpG",
                Amount = (decimal) 0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = btcBus.FakePendingTransaction(_trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }

        [TestCase(25000)]
        public void FakePeningTransaction2(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = BTCBusinessTest.ConnectionString
            };

            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(PersistenceFactory);
            var _trans = new BitcoinWithdrawTransaction
            {
                ToAddress = "2MtXuJo6U69RP2otQuAiw2bKQmZEgiAHVJE",
                Amount = (decimal) 0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = btcBus.FakePendingTransaction(_trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }

        [TestCase(25000)]
        public void FakePeningTransaction3(int numOfTrans)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = BTCBusinessTest.ConnectionString
            };

            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            btcBus = new Vakapay.BitcoinBusiness.BitcoinBusiness(PersistenceFactory);
            var _trans = new BitcoinWithdrawTransaction
            {
                ToAddress = "2NEQBQ2JU1gezBaJg5Lwq69Pr8c65y7TEbv",
                Amount = (decimal) 0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
                outPut = btcBus.FakePendingTransaction(_trans);
            Console.WriteLine(JsonHelper.SerializeObject(outPut));
            Assert.IsNotNull(outPut);
        }
    }
}