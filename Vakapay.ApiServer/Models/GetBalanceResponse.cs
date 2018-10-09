using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vakapay.ApiServer.Models
{
    public class CurrencyBalance
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency => Vakapay.Models.Domains.NetworkName.CurrencySymbols[NetworkName];
        
        [JsonProperty(PropertyName = "amount")]
        public string Amount => Vakapay.Models.Domains.NetworkName.GetAmount(NetworkName, AmountDecimal);

        [JsonIgnore]
        public decimal AmountDecimal { private get; set; }
        
        [JsonIgnore]
        public string NetworkName { private get; set; }
    }
    
    public class GetBalanceResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public List<CurrencyBalance> Balance { get; set; }
    }
}