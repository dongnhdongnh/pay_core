using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("ethereumaddress")]
    public class EthereumAddress : BlockchainAddress
    {
        public string Password { get; set; }
    }
}