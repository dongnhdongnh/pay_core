using System;
using System.Collections.Generic;
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

        static async Task RunAsync()
        {
            var currentTime = CommonHelper.GetUnixTimestamp();

            foreach (var networkName in DashboardConfig.NETWORK_LIST)
            {
                foreach (var timeCondition in DashboardConfig.TIME_LIST)
                {
                    try
                    {
                        if (timeCondition.Equals(DashboardConfig.CURRENT))
                        {
                            GetCurrentPrice(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, networkName,
                                DashboardConfig.DAY), String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, networkName,
                                timeCondition));
                        }
                        else
                        {
                            var priceList = await GetAsyncByTimeStamp(networkName,
                                currentTime - Time.SECOND_COUNT_IN_PERIOD[timeCondition], currentTime);
                            CacheHelper.SetCacheString(
                                String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, networkName, timeCondition), priceList);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error when scanCoinmarket save to redis " + e);
                        throw;
                    }
                }
            }
        }

        //arrPrice ~ [[1540134840000,6545.53],...,[1540135140000,6545.53]]
        private static void GetCurrentPrice(string cacheKeyDay, string cacheKeyCurrent)
        {
            if (CacheHelper.HaveKey(cacheKeyDay))
            {
                var arrPrice = CacheHelper.GetCacheString(cacheKeyDay);
                Console.WriteLine("arrPrice = "+ arrPrice);
                var split = arrPrice.Split(",");
                var lastElement = split[split.Length - 1];
                var price = lastElement.Substring(0, lastElement.Length - 2);
                Console.WriteLine("price = "+ price);
                CacheHelper.SetCacheString(cacheKeyCurrent, price);
            }
        }

        public static void Main(string[] args)
        {
            client.BaseAddress = new Uri(AppSettingHelper.GetCoinMarketUrl());
            client.Timeout = TimeSpan.FromSeconds(30);
            while (true)
            {
                Console.WriteLine("Get data from Coinmarket!!!");
                RunAsync().GetAwaiter().GetResult();
                Thread.Sleep(AppSettingHelper.GetCoinMarketInterval());
            }
        }
    }
}