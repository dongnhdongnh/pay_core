using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{   
    [Table("bitcoinaddress")]
    public class BitcoinAddress
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string WalletId { get; set; }
        public string Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
    }
}