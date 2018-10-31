using Newtonsoft.Json;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class UserSendTransaction : BaseModel
    {
        public string UserId { get; set; }

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

        [JsonProperty("idem")]
        public string Idem { get; set; }

        [JsonProperty(PropertyName = "sendByAd")]
        public bool SendByBlockchainAddress{ get; set; }
        //[JsonProperty(PropertyName = "note")]
        //public string Description { get; set; }
    }
}