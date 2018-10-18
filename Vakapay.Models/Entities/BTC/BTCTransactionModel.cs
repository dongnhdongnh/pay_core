namespace Vakapay.Models.Entities
{
    using Newtonsoft.Json;

    public class BtcTransactionModel
    {
        [JsonProperty("amount")] public decimal Amount { get; set; }

        [JsonProperty("confirmations")] public long Confirmations { get; set; }

        [JsonProperty("blockhash")] public string BlockHash { get; set; }

        [JsonProperty("blockindex")] public long BlockIndex { get; set; }

        [JsonProperty("blocktime")] public long BlockTime { get; set; }

        [JsonProperty("txid")] public string Txid { get; set; }

        [JsonProperty("walletconflicts")] public object[] WalletConflicts { get; set; }

        [JsonProperty("time")] public long Time { get; set; }

        [JsonProperty("timereceived")] public long TimeReceived { get; set; }

        [JsonProperty("bip125-replaceable")] public string Bip125Replaceable { get; set; }

        [JsonProperty("details")] public BtcTransactionDetailModel[] BtcTransactionDetailsModel { get; set; }

        [JsonProperty("hex")] public string Hex { get; set; }

        public static BtcTransactionModel FromJson(string json) =>
            JsonHelper.DeserializeObject<BtcTransactionModel>(json, JsonHelper.ConvertSettings);

        public string ToJson() =>
            JsonHelper.SerializeObject(this, JsonHelper.ConvertSettings);
    }

    public class BtcTransactionDetailModel
    {
        [JsonProperty("account")] public string Account { get; set; }

        [JsonProperty("address")] public string Address { get; set; }

        [JsonProperty("category")] public string Category { get; set; }

        [JsonProperty("amount")] public decimal Amount { get; set; }

        [JsonProperty("label")] public string Label { get; set; }

        [JsonProperty("vout")] public long VOut { get; set; }
    }
}