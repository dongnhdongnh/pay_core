using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    class ETHRunTest
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start auto scan");
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(persistenceFactory);
            //_ethBus.AutoScanBlock();
        }
    }
}