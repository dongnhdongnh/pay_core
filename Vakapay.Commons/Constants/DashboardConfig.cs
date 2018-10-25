using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public class DashboardConfig
    {
        public const string CURRENT = "current";
        public const string HOUR = "hour";
        public const string DAY = "day";
        public const string WEEK = "week";
        public const string MONTH = "month";
        public const string YEAR = "year";
        public const string ALL = "all";
        
        public const string VAKACOIN = "vakacoin";
        public const string BITCOIN = "bitcoin";
        public const string ETHEREUM = "ethereum";

        public const int INTERVAL = 5;

        // current always in the last
        public static readonly List<string> TIME_LIST = new List<string>
        {
            DAY, WEEK, MONTH, YEAR, ALL, CURRENT
        };
        
        public static readonly List<string> NETWORK_LIST = new List<string>
        {
            VAKACOIN, BITCOIN, ETHEREUM
        };
    }
}