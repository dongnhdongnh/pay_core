﻿using StackExchange.Redis;
using System;

namespace Vakapay.Commons.Helpers
{
	public class CacheHelper
	{
		static CacheHelper()
		{
			CacheHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
			{
				return ConnectionMultiplexer.Connect($"{AppSettingHelper.GetRedisConfig()}");
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
		public static bool HaveKey(RedisKey key)
		{
			return CacheDatabase.KeyExists(key);
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
