using NUnit.Framework;
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
                ConnectionString =
                    "server=127.0.0.1;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            // New DB connection
            var accountRepo = persistenceFactory.GetVakacoinAccountRepository(persistenceFactory.GetDbConnection());

        }
    }
}