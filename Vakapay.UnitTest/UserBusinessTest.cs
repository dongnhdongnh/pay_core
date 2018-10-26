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

        [Test]
        public void UserInfo()
        {
            Console.WriteLine("start");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            Console.WriteLine("New Address");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var userBus = new UserBusiness.UserBusiness(PersistenceFactory);

            var search =
                new Dictionary<string, string>
                {
                    {"Email", ""}
                };
            var resultCreated = userBus.GetUserInfo(search);
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
        }

        [Test]
        public void UserLog()
        {
            Console.WriteLine("start");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
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
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            Console.WriteLine("New Address");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var userBus = new UserBusiness.UserBusiness(PersistenceFactory);
            var walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
            var userRepo = PersistenceFactory.GetUserRepository(PersistenceFactory.GetOldConnection());

            var resultCreated = userBus.Login(
                new User {Email = "ngochuan2212@gmail.com", PhoneNumber = "+84988478266", FullName = "Ngo Ngoc Huan"});
            walletBusiness.MakeAllWalletForNewUser(userRepo.FindByEmailAddress("ngochuan2212@gmail.com"));
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);

            resultCreated = userBus.Login(
                new User
                {
                    Email = "tieuthanhliem@gmail.com", PhoneNumber = "+84965995710", FullName = "Tieu Thanh Liem"
                });
            walletBusiness.MakeAllWalletForNewUser(userRepo.FindByEmailAddress("tieuthanhliem@gmail.com"));
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);
        }
    }
}