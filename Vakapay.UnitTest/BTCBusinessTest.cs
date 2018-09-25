using System;
using NUnit.Framework;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;
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
            set
            {
                this._PersistenceFactory = value;
            }
        }
        const String RPCEndpoint = "http://localhost:18443";
        const String RpcUser = "bitcoinrpc";
        const String RpcPassword = "7bLjxV1CKhNJmdxTUMxTpF4vEemWCp49kMX9CwvZabYi";
        const String ConnectionString = "server=localhost;userid=root;password=huan@123;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
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
            btcBus = new  Vakapay.BitcoinBusiness.BitcoinBusiness(PersistenceFactory);
            var walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
            var bitcoinRepo = PersistenceFactory.GetBitcoinAddressRepository(connection);
            string walletID = CommonHelper.RandomString(15);
            var resultCreated = await btcBus.CreateAddressAsyn<BitcoinAddress>(walletBusiness, bitcoinRepo, RPCClass, walletID);
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);
        }
    }
}