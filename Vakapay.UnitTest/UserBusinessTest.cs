using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    [TestFixture]
    public class UserBusinessTest
    {
        VakapayRepositoryMysqlPersistenceFactory _PersistenceFactory;

        VakapayRepositoryMysqlPersistenceFactory PersistenceFactory
        {
            get
            {
                if (_PersistenceFactory == null)
                {
                    var repositoryConfig = new RepositoryConfiguration
                    {
                        ConnectionString = UserBusinessTest.ConnectionString
                    };
                    Console.WriteLine("New Connect");
                    _PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                }

                return _PersistenceFactory;
            }
            set { this._PersistenceFactory = value; }
        }

        const String ConnectionString =
            "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";

        UserBusiness.UserBusiness userBus;

        [Test]
        public void UserInfo()
        {
            Console.WriteLine("start");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = UserBusinessTest.ConnectionString
            };

            Console.WriteLine("New Address");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var userBus = new UserBusiness.UserBusiness(PersistenceFactory);

            var search =
                new Dictionary<string, string>
                {
                    {"Email", ""}
                };
            var resultCreated = userBus.getUserInfo(search);
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
        }

        [Test]
        public void UserLog()
        {
            Console.WriteLine("start");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = UserBusinessTest.ConnectionString
            };

            Console.WriteLine("New Address");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var userBus = new UserBusiness.UserBusiness(PersistenceFactory);

            var log = new UserActionLog
            {
                Description = "aaaa",
                Ip = "192.168.1.157",
                ActionName = "loggin",
                UserId = "aaaaaaaaaa",
                CreatedAt = (int) CommonHelper.GetUnixTimestamp()
            };
            var resultCreated = userBus.AddActionLog(log.Description, log.UserId, log.ActionName, log.Ip);
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);
        }

        [Test]
        public void AfterLogin()
        {
            Console.WriteLine("start");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = UserBusinessTest.ConnectionString
            };

            Console.WriteLine("New Address");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var userBus = new UserBusiness.UserBusiness(PersistenceFactory);
            var walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
            var resultCreated = userBus.Login(
                new User {Email = "ngochuan2212@gmail.com", PhoneNumber = "+84988478266", FullName = "Ngo Ngoc Huan"});
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);

            resultCreated = userBus.Login(
                new User
                {
                    Email = "tieuthanhliem@gmail.com", PhoneNumber = "+84965995710", FullName = "Tieu Thanh Liem"
                });
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);
        }
    }
}