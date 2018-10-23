namespace Vakapay.Configuration
{
    public static class CoinmarketConfiguration
    {
        public const string COINMARKET_PRICE_CACHEKEY = "price_{0}_{1}"; // price_{networkName}_{day/week/month/year/all}

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
    }
}