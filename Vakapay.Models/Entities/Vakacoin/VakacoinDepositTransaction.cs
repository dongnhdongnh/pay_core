using System.ComponentModel.DataAnnotations.Schema;

namespace Vakapay.Models.Entities
{
    [Table("VakacoinDepositTransaction")]
    public class VakacoinDepositTransaction : VakacoinTransaction
    {
        public string TrxId { get; set; }
    }
}