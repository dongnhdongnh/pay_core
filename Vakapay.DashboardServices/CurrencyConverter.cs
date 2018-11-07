using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.Commons.Helpers;

namespace Vakapay.DashboardServices
{
    public class CurrencyConverter
    {
        private static HttpClient client = new HttpClient();
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static async Task<string> GetAsync(string path)
        {
            _logger.Info("path = " + path);
            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    string resultTemp = await response.Content.ReadAsStringAsync();
                    return resultTemp;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }
        }

        static async Task RunAsync()
        {
            var result = await GetAsync("convert?q=USD_VND&compact=y");
            if (result != null)
            {
                try
                {
                    var jsonObject = JObject.Parse(result);
                    // jsonObjec ~ {"USD_VND":{"val":23203.1}}
                    CacheHelper.SetCacheString("USD_VND", jsonObject["USD_VND"]["val"].ToString());
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }
        }

        public static void GetCurrencyConverter()
        {
            client.BaseAddress = new Uri(AppSettingHelper.GetCurrencyConverterUrl());
            client.Timeout = TimeSpan.FromSeconds(30);
            while (true)
            {
                _logger.Info("Get data from CurrencyConverter Api!!!");
                RunAsync().GetAwaiter().GetResult();
                Thread.Sleep(AppSettingHelper.GetCurrencyConverterInterval());
            }
        }
    }
}