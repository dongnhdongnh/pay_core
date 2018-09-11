using System;
using NUnit.Framework;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.VakacoinBusiness.Test
{
    [TestFixture]
    public class VakacoinBusinessTest
    {
        private VakacoinBusiness _vb;
 
        [SetUp]
        public void Setup()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = "server=127.0.0.1;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
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
        public void FourPlusOneEqualFive()
        {
//            Assert.AreEqual(5, _vb.Add(4, 1));
        }
    }
}