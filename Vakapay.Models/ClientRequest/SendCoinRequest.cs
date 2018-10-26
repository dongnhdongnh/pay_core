using Newtonsoft.Json;

namespace Vakapay.Models.ClientRequest
{
    public class SendCoinRequest
    {
        [JsonProperty("type")] [JsonRequired] public string Type { get; set; }

        [JsonProperty("to")] [JsonRequired] public string To { get; set; }

        [JsonProperty("amount")]
        [JsonRequired]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        [JsonRequired]
        public string Currency { get; set; }

        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("fee")] public string Fee { get; set; }

        [JsonProperty("idem")] public string Idem { get; set; }
    }
}