using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("Wallet")]
    public class Wallet : MultiThreadUpdateModel
    {
        public decimal Balance { get; set; }
        public string UserId { get; set; }
        public string Currency { get; set; }
        public int AddressCount { get; set; }

        public string GetAmount()
        {
            return CryptoCurrency.GetAmount(Currency, Balance);
        }
    }
}