using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vakapay.Commons.Helpers
{
	public class CacheHelper
	{
		static CacheHelper()
		{
			CacheHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
			{
				return ConnectionMultiplexer.Connect("127.0.0.1:6379");
			});
		}

		private static Lazy<ConnectionMultiplexer> lazyConnection;

		private static ConnectionMultiplexer Connection
		{
			get
			{
				return lazyConnection.Value;
			}
		}

		private static IDatabase CacheDatabase
		{
			get
			{
				return Connection.GetDatabase();
			}
		}

		public static void SetCacheString(String key, String value)
		{
			CacheDatabase.StringSet(key, value);

		}
		public static String GetCacheString(String key)
		{

			return CacheDatabase.StringGet(key);
		}

		public static bool DeleteCacheString(String key)
		{

			return CacheDatabase.KeyDelete(key);
		}

		public class CacheKey
		{
			public const String KEY_SCANBLOCK_LASTSCANBLOCK = "KEY_{0}_LASTSCANBLOCK";
		}
	}
}
