using NUnit.Framework;
using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    [TestFixture]
    class WalletBussinessTest
    {
        const String RPCEndpoint = "http://localhost:8545";
        const String ConnectionString = "server=localhost;userid=root;password=123;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
        Vakapay.WalletBusiness.WalletBusiness _walletBusiness;
        Vakapay.EthereumBusiness.EthereumBusiness _ethBus;

        [Test]
        public void CreateNewWallet()
        {
            var repositoryConfig = new RepositoryConfiguration()
            {
                ConnectionString = WalletBussinessTest.ConnectionString
            };
            
            var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _walletBusiness =
                new Vakapay.WalletBusiness.WalletBusiness(persistence);
            var user = new User();
            var blockChain = new BlockchainNetwork();
            user.Id = "8377a95b-79b4-4dfb-8e1e-b4833443c306";
            
            blockChain.Name = "Ethereum";
            var resultTest = _walletBusiness.CreateNewWallet(user, blockChain);
            Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
            
            blockChain.Name = "BTC";
            _walletBusiness.CreateNewWallet(user, blockChain);
            Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
            
            blockChain.Name = "VAKA";
            _walletBusiness.CreateNewWallet(user, blockChain);
            Assert.AreEqual(Status.StatusSuccess, resultTest.Status);

        }
        
        [Test]
        public void CreateNewAddress()
        {

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = WalletBussinessTest.ConnectionString
            };

            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory, true, RPCEndpoint);
            var wallet = new Wallet();
            wallet.Id = "c8da3877-7e35-4872-ace8-db2fadfea2c1";
            var resultTest = _ethBus.CreateNewAddAddress(wallet);

            Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
        }

        [Test]
        public void InsertPendingTxsToWithdraw()
        {
            var repositoryConfig = new RepositoryConfiguration()
            {
                ConnectionString = WalletBussinessTest.ConnectionString
            };
            
            var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _walletBusiness =
                new Vakapay.WalletBusiness.WalletBusiness(persistence);
            
            var wallet = new Wallet();
            wallet.Id = "220f8c33-053d-472f-b85a-ca94464b5176";
            var toAddr = "0xe2605fe203781dd11a6c44c07c2535eabc0aed23";
            
            var resultTest = _walletBusiness.Withdraw(wallet, toAddr, 5);
            Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
        }
        
        [Test]
        public void SendTransaction_GetHash()
        {

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = WalletBussinessTest.ConnectionString
            };

            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory, true, RPCEndpoint);

            var resultTest = _ethBus.RunSendTransaction();
            Assert.AreEqual(Status.StatusSuccess, resultTest.Status);
        }
        
        [Test]
        public void TestScan()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = WalletBussinessTest.ConnectionString
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);
            _ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(persistenceFactory, true, RPCEndpoint);
            int block = _ethBus.ScanBlock(walletBusiness);

            Assert.IsTrue(block > 0);
        }
    }
}