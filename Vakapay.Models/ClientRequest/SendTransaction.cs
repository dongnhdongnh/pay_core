using Newtonsoft.Json;

namespace Vakapay.Models.ClientRequest
{
    public class TransactionDetail
    {
        [JsonProperty(PropertyName = "recipientWalletAddress")]
        public string RecipientWalletAddress { get; set; }

        [JsonProperty(PropertyName = "recipientEmailAddress")]
        public string RecipientEmailAddress { get; set; }

        [JsonProperty(PropertyName = "PricePerCoin")]
        public decimal PricePerCoin { get; set; }

        [JsonProperty(PropertyName = "VNDAmount")]
        public decimal VndAmount { get; set; }

        [JsonProperty(PropertyName = "VKCAmount")]
        public decimal VkcAmount { get; set; }

        [JsonProperty(PropertyName = "VKCNote")]
        public string VkcNote { get; set; }

        [JsonProperty(PropertyName = "sendByAd")]
        [JsonRequired]
        public bool SendByAd { get; set; }
    }

    public class SendTransaction
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "sortName")]
        public string SortName { get; set; }

        [JsonProperty(PropertyName = "networkName")]
        [JsonRequired]
        public string NetworkName { get; set; }

        [JsonProperty(PropertyName = "detail")]
        [JsonRequired]
        public TransactionDetail Detail { get; set; }

        [JsonProperty(PropertyName = "SMSCode")]
        public string SmsCode { get; set; }

    }
}