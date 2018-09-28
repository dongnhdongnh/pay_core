using System;
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
			var resultCreated = userBus.Login(walletBusiness, "ngochuan2212@gmail.com", "+84988478266", "Ngo Ngoc Huan");
			Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
			Assert.IsNotNull(resultCreated);
			
			resultCreated = userBus.Login(walletBusiness, "tieuthanhliem@gmail.com", "+84965995710", "Tieu Thanh Liem");
			Console.WriteLine(JsonHelper.SerializeObject(resultCreated));
			Assert.IsNotNull(resultCreated);
		}
	}
}