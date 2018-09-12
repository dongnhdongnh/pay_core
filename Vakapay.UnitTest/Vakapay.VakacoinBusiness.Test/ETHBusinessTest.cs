using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.VakacoinBusiness.Test
{
	[TestFixture]
	class ETHBusinessTest
	{
		const String ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
		Vakapay.EthereumBusiness.EthereumBusiness _ethBus;
		[Test]
		public void CreateNewAddress()
		{

			var repositoryConfig = new RepositoryConfiguration
			{
				ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
			};

			var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
			_ethBus = new Vakapay.EthereumBusiness.EthereumBusiness(PersistenceFactory);
			string walletID = CommonHelper.RandomString(15);
			var outPut = _ethBus.CreateNewAddAddress(walletID);
			Console.WriteLine(JsonHelper.SerializeObject(outPut));
			Assert.IsNotNull(outPut);
		}
	}
}
