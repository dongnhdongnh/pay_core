using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.VakacoinBusiness.Test
{
	class ETHRunTest
	{
		const String ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
		static void Main(string[] args)
		{
			Console.WriteLine("start auto scan");
			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = ETHRunTest.ConnectionString
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			var _ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory);
			_ethBus.AutoScanBlock();
		}
	}
}
