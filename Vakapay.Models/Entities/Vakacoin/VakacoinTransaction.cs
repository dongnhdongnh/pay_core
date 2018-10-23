using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("VakacoinTransaction")]
    public class VakacoinTransaction : BlockchainTransaction
    {
        public string Memo { get; set; }

        public string GetStringAmount()
        {
            return CryptoCurrency.GetAmount(CryptoCurrency.VAKA, Amount);
        }
    }

    [Table("VakacoinDepositTransaction")]
    public class VakacoinDepositTransaction : VakacoinTransaction
    {
        public string TrxId { get; set; }
    }

    [Table("VakacoinWithdrawTransaction")]
    public class VakacoinWithdrawTransaction : VakacoinTransaction
    {
    }
}