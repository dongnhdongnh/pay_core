using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("VakacoinTransactionHistory")]
    public class VakacoinTransactionHistory
    {
        public string Id { get; set; }
        public string TrxId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public Decimal Amount { get; set; }
        public int BlockNumber { get; set; }
        public string TransactionTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Status { get; set; }
    }
}