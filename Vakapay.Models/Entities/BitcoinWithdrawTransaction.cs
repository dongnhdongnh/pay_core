using System;
using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("bitcoinwithdrawtransaction")]
    public class BitcoinWithdrawTransaction : IBlockchainTransaction
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public string BlockNumber { get; set; }
        public string BlockHash { get; set; }
        public string NetworkName { get; set; }
        public decimal Amount { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Fee { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}
