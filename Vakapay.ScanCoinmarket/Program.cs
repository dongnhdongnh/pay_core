using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Vakapay.Commons.Helpers;

namespace ScanCoinmarket
{
    class Program
    {
        private static HttpClient client = new HttpClient();

        private static IConfiguration InitConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environment))
                environment = "Development";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            return builder.Build();
        }

        static async Task<string> GetAsync(string path)
        {
            Console.WriteLine("path = " + path);
            string result = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                string resultTemp = await response.Content.ReadAsStringAsync();
                var jsonObj = JToken.Parse(resultTemp);
                result = JsonHelper.SerializeObject(jsonObj["price_usd"]);
            }

            return result;
        }

        static async Task<string> GetAsyncByTimeStamp(string networkName, long from, long to)
        {
            string path = networkName + "/" + from.ToString() + "000/" + to.ToString() +
                          "000"; //from and to in second -> to miliseconds
            var result = await GetAsync(path);
            return result;
        }

        static async Task RunAsync(IConfiguration configuration)
        {
            var currentTime = UnixTimestamp.GetCurrentEpoch();
            try
            {
                //get vakacoin
                string vakacoinByDay = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 24 * 60 * 60, currentTime);
                string vakacoinByWeek = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string vakacoinByMonth = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string vakacoinByYear = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string vakacoinAll = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);
                
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.VAKACOIN,
                        DashboardConfig.DAY), vakacoinByDay);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.VAKACOIN,
                        DashboardConfig.WEEK), vakacoinByWeek);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.VAKACOIN,
                        DashboardConfig.MONTH), vakacoinByMonth);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.VAKACOIN,
                        DashboardConfig.YEAR), vakacoinByYear);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.VAKACOIN,
                        DashboardConfig.ALL), vakacoinAll);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.VAKACOIN,
                        DashboardConfig.CURRENT), GetCurrentPrice(vakacoinByDay));
                
                // get bitcoin
                string bitcoinByDay = await GetAsyncByTimeStamp(DashboardConfig.BITCOIN,
                    currentTime - 24 * 60 * 60, currentTime);
                string bitcoinByWeek = await GetAsyncByTimeStamp(DashboardConfig.BITCOIN,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string bitcoinByMonth = await GetAsyncByTimeStamp(DashboardConfig.BITCOIN,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string bitcoinByYear = await GetAsyncByTimeStamp(DashboardConfig.BITCOIN,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string bitcoinAll = await GetAsyncByTimeStamp(DashboardConfig.BITCOIN,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);
                
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.BITCOIN,
                        DashboardConfig.DAY), bitcoinByDay);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.BITCOIN,
                        DashboardConfig.WEEK), bitcoinByWeek);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.BITCOIN,
                        DashboardConfig.MONTH), bitcoinByMonth);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.BITCOIN,
                        DashboardConfig.YEAR), bitcoinByYear);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.BITCOIN,
                        DashboardConfig.ALL), bitcoinAll);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.BITCOIN,
                        DashboardConfig.CURRENT), GetCurrentPrice(bitcoinByDay));

                //get ethereum
                string ethereumByDay = await GetAsyncByTimeStamp(DashboardConfig.ETHEREUM,
                    currentTime - 24 * 60 * 60, currentTime);
                string ethereumByWeek = await GetAsyncByTimeStamp(DashboardConfig.ETHEREUM,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string ethereumByMonth = await GetAsyncByTimeStamp(DashboardConfig.ETHEREUM,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string ethereumByYear = await GetAsyncByTimeStamp(DashboardConfig.ETHEREUM,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string ethereumAll = await GetAsyncByTimeStamp(DashboardConfig.ETHEREUM,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);

                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.ETHEREUM,
                        DashboardConfig.DAY), ethereumByDay);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.ETHEREUM,
                        DashboardConfig.WEEK), ethereumByWeek);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.ETHEREUM,
                        DashboardConfig.MONTH), ethereumByMonth);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.ETHEREUM,
                        DashboardConfig.YEAR), ethereumByYear);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.ETHEREUM,
                        DashboardConfig.ALL), ethereumAll);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.ETHEREUM,
                        DashboardConfig.CURRENT), GetCurrentPrice(ethereumByDay));

                //get eos
                string eosByDay = await GetAsyncByTimeStamp(DashboardConfig.EOS, currentTime - 24 * 60 * 60,
                    currentTime);
                string eosByWeek = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string eosByMonth = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string eosByYear = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string eosAll = await GetAsyncByTimeStamp(DashboardConfig.EOS,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);

                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.EOS,
                        DashboardConfig.DAY), eosByDay);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.EOS,
                        DashboardConfig.WEEK), eosByWeek);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.EOS,
                        DashboardConfig.MONTH), eosByMonth);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.EOS,
                        DashboardConfig.YEAR), eosByYear);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.EOS,
                        DashboardConfig.ALL), eosAll);
                CacheHelper.SetCacheString(
                    String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY, DashboardConfig.EOS,
                        DashboardConfig.CURRENT), GetCurrentPrice(eosByDay));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //arrPrice ~ [[1540134840000,6545.53],...,[1540135140000,6545.53]]
        private static string GetCurrentPrice(string arrPrice)
        {
            var split = arrPrice.Split(",");
            var lastElement = split[split.Length - 1];
            var price = lastElement.Substring(0, lastElement.Length - 2);
            return price;
        }

        static void Main(string[] args)
        {
            var configuration = InitConfiguration();
            client.BaseAddress = new Uri(configuration["coinmarketUrl"]);
            client.Timeout = TimeSpan.FromSeconds(30);
            while (true)
            {
                Console.WriteLine("Get data from Coinmarket!!!");
                RunAsync(configuration).GetAwaiter().GetResult();
                Thread.Sleep(Int32.Parse(configuration["intervalTime"]));
            }
        }
    }
}