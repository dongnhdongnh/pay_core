using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Vakapay.Commons.Helpers;
using Vakapay.Configuration;

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
                // get bitcoin
                string bitcoinByDay = await GetAsyncByTimeStamp(CoinmarketConfiguration.BITCOIN,
                    currentTime - 24 * 60 * 60, currentTime);
                string bitcoinByWeek = await GetAsyncByTimeStamp(CoinmarketConfiguration.BITCOIN,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string bitcoinByMonth = await GetAsyncByTimeStamp(CoinmarketConfiguration.BITCOIN,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string bitcoinByYear = await GetAsyncByTimeStamp(CoinmarketConfiguration.BITCOIN,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string bitcoinAll = await GetAsyncByTimeStamp(CoinmarketConfiguration.BITCOIN,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);

                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.BITCOIN,
                        CoinmarketConfiguration.DAY), bitcoinByDay);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.BITCOIN,
                        CoinmarketConfiguration.WEEK), bitcoinByWeek);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.BITCOIN,
                        CoinmarketConfiguration.MONTH), bitcoinByMonth);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.BITCOIN,
                        CoinmarketConfiguration.YEAR), bitcoinByYear);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.BITCOIN,
                        CoinmarketConfiguration.ALL), bitcoinAll);

                //get ethereum
                string ethereumByDay = await GetAsyncByTimeStamp(CoinmarketConfiguration.ETHEREUM,
                    currentTime - 24 * 60 * 60, currentTime);
                string ethereumByWeek = await GetAsyncByTimeStamp(CoinmarketConfiguration.ETHEREUM,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string ethereumByMonth = await GetAsyncByTimeStamp(CoinmarketConfiguration.ETHEREUM,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string ethereumByYear = await GetAsyncByTimeStamp(CoinmarketConfiguration.ETHEREUM,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string ethereumAll = await GetAsyncByTimeStamp(CoinmarketConfiguration.ETHEREUM,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);

                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.ETHEREUM,
                        CoinmarketConfiguration.DAY), ethereumByDay);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.ETHEREUM,
                        CoinmarketConfiguration.WEEK), ethereumByWeek);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.ETHEREUM,
                        CoinmarketConfiguration.MONTH), ethereumByMonth);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.ETHEREUM,
                        CoinmarketConfiguration.YEAR), ethereumByYear);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.ETHEREUM,
                        CoinmarketConfiguration.ALL), ethereumAll);

                //get eos
                string eosByDay = await GetAsyncByTimeStamp(CoinmarketConfiguration.EOS, currentTime - 24 * 60 * 60,
                    currentTime);
                string eosByWeek = await GetAsyncByTimeStamp(CoinmarketConfiguration.EOS,
                    currentTime - 7 * 24 * 60 * 60, currentTime);
                string eosByMonth = await GetAsyncByTimeStamp(CoinmarketConfiguration.EOS,
                    currentTime - 30 * 24 * 60 * 60, currentTime);
                string eosByYear = await GetAsyncByTimeStamp(CoinmarketConfiguration.EOS,
                    currentTime - 365 * 24 * 60 * 60, currentTime);
                string eosAll = await GetAsyncByTimeStamp(CoinmarketConfiguration.EOS,
                    currentTime - 5 * 365 * 24 * 60 * 60, currentTime);

                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.EOS,
                        CoinmarketConfiguration.DAY), eosByDay);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.EOS,
                        CoinmarketConfiguration.WEEK), eosByWeek);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.EOS,
                        CoinmarketConfiguration.MONTH), eosByMonth);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.EOS,
                        CoinmarketConfiguration.YEAR), eosByYear);
                CacheHelper.SetCacheString(
                    String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.EOS,
                        CoinmarketConfiguration.ALL), eosAll);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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