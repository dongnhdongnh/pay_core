using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("Wallet")]
    public class Wallet
    {
       
        public string Id { get; set; }
        public decimal Balance { get; set; }
        public string UserId { get; set; }
        public string Currency { get; set; }
        public bool HasAddress { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
        public int Version { get; set; }
        
        public string GetAmount()
        {
            return Domains.CryptoCurrency.GetAmount(Currency, Balance);
        }
    }
}
