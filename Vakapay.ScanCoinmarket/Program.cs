using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;

namespace Vakapay.ScanCoinmarket
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
                string vakacoinByDay = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 24 * 60 * 60, currentTime);
                string vakacoinByWeek = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string vakacoinByMonth = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string vakacoinByYear = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string vakacoinAll = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);
                
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.VAKACOIN,
                        RedisCacheKey.DAY), vakacoinByDay);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.VAKACOIN,
                        RedisCacheKey.WEEK), vakacoinByWeek);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.VAKACOIN,
                        RedisCacheKey.MONTH), vakacoinByMonth);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.VAKACOIN,
                        RedisCacheKey.YEAR), vakacoinByYear);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.VAKACOIN,
                        RedisCacheKey.ALL), vakacoinAll);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.VAKACOIN,
                        RedisCacheKey.CURRENT), GetCurrentPrice(vakacoinByDay));
                
                // get bitcoin
                string bitcoinByDay = await GetAsyncByTimeStamp(RedisCacheKey.BITCOIN,
                    currentTime - 24 * 60 * 60, currentTime);
                string bitcoinByWeek = await GetAsyncByTimeStamp(RedisCacheKey.BITCOIN,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string bitcoinByMonth = await GetAsyncByTimeStamp(RedisCacheKey.BITCOIN,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string bitcoinByYear = await GetAsyncByTimeStamp(RedisCacheKey.BITCOIN,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string bitcoinAll = await GetAsyncByTimeStamp(RedisCacheKey.BITCOIN,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);
                
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.BITCOIN,
                        RedisCacheKey.DAY), bitcoinByDay);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.BITCOIN,
                        RedisCacheKey.WEEK), bitcoinByWeek);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.BITCOIN,
                        RedisCacheKey.MONTH), bitcoinByMonth);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.BITCOIN,
                        RedisCacheKey.YEAR), bitcoinByYear);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.BITCOIN,
                        RedisCacheKey.ALL), bitcoinAll);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.BITCOIN,
                        RedisCacheKey.CURRENT), GetCurrentPrice(bitcoinByDay));

                //get ethereum
                string ethereumByDay = await GetAsyncByTimeStamp(RedisCacheKey.ETHEREUM,
                    currentTime - 24 * 60 * 60, currentTime);
                string ethereumByWeek = await GetAsyncByTimeStamp(RedisCacheKey.ETHEREUM,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string ethereumByMonth = await GetAsyncByTimeStamp(RedisCacheKey.ETHEREUM,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string ethereumByYear = await GetAsyncByTimeStamp(RedisCacheKey.ETHEREUM,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string ethereumAll = await GetAsyncByTimeStamp(RedisCacheKey.ETHEREUM,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);

                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.ETHEREUM,
                        RedisCacheKey.DAY), ethereumByDay);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.ETHEREUM,
                        RedisCacheKey.WEEK), ethereumByWeek);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.ETHEREUM,
                        RedisCacheKey.MONTH), ethereumByMonth);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.ETHEREUM,
                        RedisCacheKey.YEAR), ethereumByYear);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.ETHEREUM,
                        RedisCacheKey.ALL), ethereumAll);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.ETHEREUM,
                        RedisCacheKey.CURRENT), GetCurrentPrice(ethereumByDay));

                //get eos
                string eosByDay = await GetAsyncByTimeStamp(RedisCacheKey.EOS, currentTime - 24 * 60 * 60,
                    currentTime);
                string eosByWeek = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string eosByMonth = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string eosByYear = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string eosAll = await GetAsyncByTimeStamp(RedisCacheKey.EOS,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);

                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.EOS,
                        RedisCacheKey.DAY), eosByDay);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.EOS,
                        RedisCacheKey.WEEK), eosByWeek);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.EOS,
                        RedisCacheKey.MONTH), eosByMonth);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.EOS,
                        RedisCacheKey.YEAR), eosByYear);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.EOS,
                        RedisCacheKey.ALL), eosAll);
                CacheHelper.SetCacheString(
                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, RedisCacheKey.EOS,
                        RedisCacheKey.CURRENT), GetCurrentPrice(eosByDay));
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

        public static void Main(string[] args)
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