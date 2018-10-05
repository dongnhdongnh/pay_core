using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vakapay.ApiServer.Models
{
    public class CurrencyBalance
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
    }
    
    public class GetBalanceResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public List<CurrencyBalance> Balance { get; set; }
    }
}