using NUnit.Framework;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    [TestFixture]
    public class SendVakacoinTest
    {
        [SetUp]
        public void Setup()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            // New DB connection
            var accountRepo = persistenceFactory.GetVakacoinAccountRepository(persistenceFactory.GetDbConnection());

        }
    }
}