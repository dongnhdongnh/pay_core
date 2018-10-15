using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities.BTC
{   
    [Table("BitcoinAddress")]
    public class BitcoinAddress : BlockchainAddress
    {
    }
}