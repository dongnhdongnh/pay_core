using System;
using NUnit.Framework;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    using VakacoinBusiness;
    [TestFixture]
    public class VakacoinBusinessTest
    {
        private VakacoinBusiness _vb;
        const string ConnectionString = "server=localhost;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
 
        [SetUp]
        public void Setup()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = VakacoinBusinessTest.ConnectionString// "server=127.0.0.1;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
            };

            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _vb = new VakacoinBusiness(PersistenceFactory);
        }
 
        [Test]
        public void OnePlusOneEqualTwo()
        {
//            Assert.AreEqual(2, _vb.Add(1, 1));
        }
 
        [Test]
        public void TwoPlusTwoEqualFour()
        {
//            Assert.AreEqual(4, _vb.Add(2, 2));
        }
 
        [Test]
        public void AddAccount()
        {
            var result = _vb.AddAccount(CommonHelper.GenerateUuid(), "useraaaaaaaa",
                "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr",
                "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4D");
//            Assert.AreEqual(result.Status, Status.StatusSuccess);
        }
        
        [TestCase(10)]
        public void FakePeningTransaction(int numOfTrans)
        {
            var _trans = new VakacoinWithdrawTransaction()
            {
                FromAddress = "useraaaaaaaa",
                ToAddress   = "useraaaaaaab",
                Amount = (decimal) 0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
            {
                outPut = _vb.FakePendingTransaction(_trans);
                Assert.AreEqual(outPut.Status, Status.StatusSuccess);
            }

            Console.WriteLine(JsonHelper.SerializeObject(outPut));
        }
    }
}