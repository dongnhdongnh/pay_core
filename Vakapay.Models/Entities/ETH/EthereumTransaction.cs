using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities.ETH
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
    }
}