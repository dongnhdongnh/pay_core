using System;
using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("BitcoinWithdrawTransaction")]
    public class BitcoinWithdrawTransaction : BlockchainTransaction
    {
        public string BlockHash { get; set; }
    }
}
