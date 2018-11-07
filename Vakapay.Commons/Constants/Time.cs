using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public class Time
    {
        public static readonly Dictionary<string, long> SECOND_COUNT_IN_PERIOD = new Dictionary<string, long>
        {
            {DashboardConfig.CURRENT, 24 * 60 * 60},
            {DashboardConfig.HOUR, 60 * 60},
            {DashboardConfig.DAY, 24 * 60 * 60},
            {DashboardConfig.WEEK, 7 * 24 * 60 * 60},
            {DashboardConfig.MONTH, 30 * 24 * 60 * 60},
            {DashboardConfig.YEAR, 365 * 24 * 60 * 60},
            {DashboardConfig.ALL, 5 * 365 * 24 * 60 * 60}
        };
    }
}