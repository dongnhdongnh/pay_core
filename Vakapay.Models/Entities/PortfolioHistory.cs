using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("PortfolioHistory")]
    public class PortfolioHistory
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal VakacoinAmount { get; set; }
        public decimal VakacoinValue { get; set; }
        public decimal BitcoinAmount { get; set; }
        public decimal BitcoinValue { get; set; }
        public decimal EthereumAmount { get; set; }
        public decimal EthereumValue { get; set; }
        public long Timestamp { get; set; }
    }
}