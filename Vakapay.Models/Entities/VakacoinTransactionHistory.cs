using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("VakacoinTransactionHistory")]
    public class VakacoinTransactionHistory
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public Decimal Amount { get; set; }
        public DateTime TransactionTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public int status { get; set; }
    }
}