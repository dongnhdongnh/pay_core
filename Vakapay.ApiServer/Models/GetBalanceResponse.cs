using System.Collections.Generic;
using Newtonsoft.Json;
using Vakapay.Commons.Constants;

namespace Vakapay.ApiServer.Models
{
    public class CurrencyBalance
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency;

        [JsonProperty(PropertyName = "amount")]
        public string Amount => CryptoCurrency.GetAmount(Currency, AmountDecimal);

        [JsonIgnore] public decimal AmountDecimal { private get; set; }
    }

    public class GetBalanceResponse
    {
        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public List<CurrencyBalance> Balance { get; set; }
    }
}