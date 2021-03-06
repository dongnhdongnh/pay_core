﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;

namespace Vakapay.DashboardServices
{
    public class ScanCoinmarket
    {
        private static HttpClient client = new HttpClient();
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static async Task<string> GetAsync(string path)
        {
            _logger.Info("path = " + path);
            string result = null;
            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    string resultTemp = await response.Content.ReadAsStringAsync();
                    var jsonObj = JToken.Parse(resultTemp);
                    result = JsonHelper.SerializeObject(jsonObj["price_usd"]);
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        static async Task<string> GetAsyncByTimeStamp(string networkName, long from, long to)
        {
            string path = networkName + "/" + from + "000/" + to +
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
                                DashboardConfig.DAY), String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                                networkName,
                                timeCondition));
                        }
                        else
                        {
                            var priceList = await GetAsyncByTimeStamp(networkName,
                                currentTime - Time.SECOND_COUNT_IN_PERIOD[timeCondition], currentTime);
                            if (priceList != null)
                            {
                                CacheHelper.SetCacheString(
                                    String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY, networkName, timeCondition),
                                    priceList);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error("Error when scanCoinmarket save to redis " + e);
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
                var split = arrPrice.Split(",");
                var lastElement = split[split.Length - 1];
                var price = lastElement.Substring(0, lastElement.Length - 2);
                CacheHelper.SetCacheString(cacheKeyCurrent, price);
            }
        }

        public static void RunScanCoinmarket()
        {
            client.BaseAddress = new Uri(AppSettingHelper.GetCoinMarketUrl());
            client.Timeout = TimeSpan.FromSeconds(30);
            while (true)
            {
                _logger.Info("Get data from Coinmarket!!!");
                RunAsync().GetAwaiter().GetResult();
                Thread.Sleep(AppSettingHelper.GetCoinMarketInterval());
            }
        }
    }
}