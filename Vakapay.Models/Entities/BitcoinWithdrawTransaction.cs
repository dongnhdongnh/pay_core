using System;
using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("bitcoinwithdrawtransaction")]
    public class BitcoinWithdrawTransaction : IBlockchainTransaction
    {
        public string BlockHash { get; set; }
    }
}
