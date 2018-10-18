using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class EthereumTransaction : BlockchainTransaction
    {
    }
    
    [Table("EthereumDepositTransaction")]
    public class EthereumDepositTransaction : EthereumTransaction
    {
    }
    
    [Table("EthereumWithdrawTransaction")]
    public class EthereumWithdrawTransaction : EthereumTransaction
    {
        public string Idem { get; set; }
    }
}