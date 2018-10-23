using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vakapay.ApiServer.Models
{
    public class TransactionDetail
    {
        [JsonProperty(PropertyName = "Recipient_WalletAddress")]
        public string RecipientWalletAddress { get; set; }

        [JsonProperty(PropertyName = "Recipient_EmailAddress")]
        public string RecipientEmailAddress { get; set; }

        [JsonProperty(PropertyName = "VNDAmount")]
        public decimal VndAmount { get; set; }

        [JsonProperty(PropertyName = "VKCAmount")]
        public decimal VkcAmount { get; set; }

        [JsonProperty(PropertyName = "VKCnote")]
        public string VkcNote { get; set; }

        [JsonProperty(PropertyName = "withdrawn_from")]
        public string WithdrawnFrom { get; set; }

        [JsonProperty(PropertyName = "sendByAd")]
        public bool SendByAd { get; set; }
    }

    public class TransactionCheckObject
    {
        [JsonProperty(PropertyName = "vakapayfee")]
        public decimal VakapayFee;

        [JsonProperty(PropertyName = "minerfee")]
        public decimal MinerFee;

        [JsonProperty(PropertyName = "total")]
        public decimal Total;
    }

    public class SendTransaction
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "sortName")]
        public string SortName { get; set; }

        [JsonProperty(PropertyName = "networkName")]
        public string NetworkName { get; set; }

        [JsonProperty(PropertyName = "address")]
        public List<string> Address { get; set; }

        [JsonProperty(PropertyName = "detail")]
        public TransactionDetail Detail { get; set; }

        [JsonProperty(PropertyName = "checkObject")]
        public TransactionCheckObject CheckObject { get; set; }

        [JsonProperty(PropertyName = "SMScode")]
        public string SmsCode { get; set; }
    }
}