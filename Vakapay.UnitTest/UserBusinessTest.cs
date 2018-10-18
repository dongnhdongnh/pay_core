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
                        ConnectionString = AppSettingHelper.GetDBConnection()
                    };
                    Console.WriteLine("New Connect");
                    _PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                }

                return _PersistenceFactory;
            }
            set { this._PersistenceFactory = value; }
        }

        UserBusiness.UserBusiness userBus;

        [Test]
        public void UserInfo()
        {
            Console.WriteLine("start");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
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
                ConnectionString = AppSettingHelper.GetDBConnection()
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
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            Console.WriteLine("New Address");
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var userBus = new UserBusiness.UserBusiness(PersistenceFactory);
            var walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
            var userRepo = PersistenceFactory.GetUserRepository(PersistenceFactory.GetOldConnection());

            var resultCreated = userBus.Login(
                new User {Email = "ngochuan2212@gmail.com", PhoneNumber = "+84988478266", FullName = "Ngo Ngoc Huan"});
            var resultTest = walletBusiness.MakeAllWalletForNewUser(userRepo.FindBySql("select * from User where Email='ngochuan2212@gmail.com'")[0]);
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);

            resultCreated = userBus.Login(
                new User
                {
                    Email = "tieuthanhliem@gmail.com", PhoneNumber = "+84965995710", FullName = "Tieu Thanh Liem"
                });
            resultTest = walletBusiness.MakeAllWalletForNewUser(userRepo.FindBySql("select * from User where Email='tieuthanhliem@gmail.com'")[0]);
            Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
            Assert.IsNotNull(resultCreated);
        }
    }
}