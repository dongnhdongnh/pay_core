using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("PendingVakacoinTransaction")]
    public class PendingVakacoinTransaction
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public int Version { get; set; }
    }
}