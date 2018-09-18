using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("bitcoindeposittransaction")]
    public partial class BitcoinDepositTransaction : BlockchainTransaction
    {
        public string BlockHash { get; set; }
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