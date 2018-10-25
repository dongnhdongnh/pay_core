using System;

namespace Vakapay.Commons.Constants
{
    public class RedisCacheKey
    {
        // Dashboard's config
        public const string COINMARKET_PRICE_CACHEKEY = "price_{0}_{1}";
        public const string VAKACOIN = "vakacoin";
        public const string BITCOIN = "bitcoin";
        public const string ETHEREUM = "ethereum";
        public const string EOS = "eos";
        
        public const string DAY = "day";
        public const string WEEK = "week";
        public const string MONTH = "month";
        public const string YEAR = "year";
        public const string ALL = "all";
        public const string CURRENT = "current";

        public const int INTERVAL = 5;
        
        //CacheHelper
        public const String KEY_SCANBLOCK_LASTSCANBLOCK = "KEY_{0}_LASTSCANBLOCK";
    }
}