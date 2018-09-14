using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("bitcoindeposittransaction")]
    public partial class BitcoinDepositTransaction : IBlockchainTransaction
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public string BlockNumber { get; set; }
        public string BlockHash { get; set; }
        public string NetworkName { get; set; }
        public decimal Amount { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Fee { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }

    public partial class BitcoinDepositTransaction
    {
        public static BitcoinDepositTransaction FromJson(string json) =>
            JsonConvert.DeserializeObject<BitcoinDepositTransaction>(json, JsonHelper.ConvertSettings);
    }

    public static class Serialize
    {
        public static string ToJson(this BitcoinDepositTransaction self) =>
            JsonConvert.SerializeObject(self, JsonHelper.ConvertSettings);
    }
}