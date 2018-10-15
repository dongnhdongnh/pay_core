using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("VakacoinAccount")]
    public class VakacoinAccount : BlockchainAddress
    {
        public string AccountName { get; set; }
        public string OwnerPrivateKey { get; set; }
        public string OwnerPublicKey { get; set; }
        public string ActivePrivateKey { get; set; }
        public string ActivePublicKey { get; set; }
        
        public override string GetAddress()
        {
            return AccountName;
        }

        public override string GetSecret()
        {
            return ActivePrivateKey;
        }
    }
}
