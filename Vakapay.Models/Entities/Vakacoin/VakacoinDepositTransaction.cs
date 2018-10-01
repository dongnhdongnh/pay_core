using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("VakacoinDepositTransaction")]
    public class VakacoinDepositTransaction : VakacoinTransaction
    {
        public string TrxId { get; set; }
    }
}