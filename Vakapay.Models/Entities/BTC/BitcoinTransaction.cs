using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class BitcoinTransaction : BlockchainTransaction
    {
        public string BlockHash { get; set; }
    }
    
    [Table("BitcoinDepositTransaction")]
    public class BitcoinDepositTransaction : BitcoinTransaction
    {
        public static BitcoinDepositTransaction FromJson(string json) =>
            JsonHelper.DeserializeObject<BitcoinDepositTransaction>(json, JsonHelper.ConvertSettings);

        public string ToJson() =>
            JsonHelper.SerializeObject(this, JsonHelper.ConvertSettings);
    }
    
    [Table("BitcoinWithdrawTransaction")]
    public class BitcoinWithdrawTransaction : BitcoinTransaction
    {
        public string Idem { get; set; }
    }
}