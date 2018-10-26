using System;
using Newtonsoft.Json;

namespace VakaSharp.CustomTypes
{
    [Serializable]
    public class TransferData
    {
        [JsonProperty("from")] public string From { get; set; }
        [JsonProperty("to")] public string To { get; set; }
        [JsonProperty("quantity")] public string Quantity { get; set; }
        [JsonProperty("memo")] public string Memo { get; set; }

        public string Symbol()
        {
            return Quantity.Split(" ")[1];
        }

        public decimal Amount()
        {
            return Decimal.Parse(Quantity.Split(" ")[0]);
        }
    }
}