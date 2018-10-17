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
            return string.Format("{0:0.0000}", Amount) + CryptoCurrency.GetSymbol(CryptoCurrency.VAKA);
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