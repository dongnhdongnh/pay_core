using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;
using Vakapay.Commons.Helpers;

namespace Vakapay.VakacoinBusiness
{
    public class VakacoinRpc
    {
        private string EndPointUrl { get; set; }
        private string VakacoinVersion { get; } = "v1";
        
        public VakacoinRpc(string endPointUrl)
        {
            if (endPointUrl[endPointUrl.Length - 1] != '/')
            {
                endPointUrl += "/";
            }

            endPointUrl += VakacoinVersion;
            
            EndPointUrl = endPointUrl; // return end://point.url/v1
        }

        private string GetAccountPostUrl()
        {
            return EndPointUrl + "/chain/get_account";
        }

        private string GetInfoPostUrl()
        {
            return EndPointUrl + "/chain/get_info";
        }

        public ReturnObject CreateAddress(string password)
        {
            return null;
        }

        public bool CheckAccountExist(string accountName)
        {
            var values = new Dictionary<string, string>
            {
                { "account_name", "vaka" },
            };

            var result = HttpClientHelper.PostRequest(GetAccountPostUrl(), values);
            var jResult= JObject.Parse(result);
            var exist = jResult["error"] == null;
            return exist;
        }
    }
}