using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("vakacoindeposittransaction")]
    public class VakacoinDepositTransaction : BlockchainTransaction
    {
        public string TrxId { get; set; }
    }
}