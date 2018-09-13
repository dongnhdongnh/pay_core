using Vakapay.Commons.Helpers;

namespace Vakapay.Models.Entities.BTC
{
    using Newtonsoft.Json;

    public partial class BTCTransactionModel
    {
        [JsonProperty("amount")] public long Amount { get; set; }

        [JsonProperty("confirmations")] public long Confirmations { get; set; }

        [JsonProperty("blockhash")] public string Blockhash { get; set; }

        [JsonProperty("blockindex")] public long Blockindex { get; set; }

        [JsonProperty("blocktime")] public long Blocktime { get; set; }

        [JsonProperty("txid")] public string Txid { get; set; }

        [JsonProperty("walletconflicts")] public object[] Walletconflicts { get; set; }

        [JsonProperty("time")] public long Time { get; set; }

        [JsonProperty("timereceived")] public long Timereceived { get; set; }

        [JsonProperty("bip125-replaceable")] public string Bip125Replaceable { get; set; }

        [JsonProperty("details")] public BTCTransactionDetailModel[] BtcTransactionDetailsModel { get; set; }

        [JsonProperty("hex")] public string Hex { get; set; }
    }

    public partial class BTCTransactionDetailModel
    {
        [JsonProperty("account")] public string Account { get; set; }

        [JsonProperty("address")] public string Address { get; set; }

        [JsonProperty("category")] public string Category { get; set; }

        [JsonProperty("amount")] public long Amount { get; set; }

        [JsonProperty("label")] public string Label { get; set; }

        [JsonProperty("vout")] public long Vout { get; set; }
    }

    public partial class BTCTransactionModel
    {
        public static BTCTransactionModel FromJson(string json) =>
            JsonConvert.DeserializeObject<BTCTransactionModel>(json, JsonHelper.ConvertSettings);
    }

    public static class Serialize
    {
        public static string ToJson(this BTCTransactionModel self) => JsonConvert.SerializeObject(self, JsonHelper.ConvertSettings);
    }
}