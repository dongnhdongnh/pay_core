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
				ConnectionString = AppSettingHelper.GetDBConnection()
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			var _ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory);
			//_ethBus.AutoScanBlock();
		}
	}
}
