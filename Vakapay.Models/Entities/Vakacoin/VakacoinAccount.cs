using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("VakacoinAccount")]
    public class VakacoinAccount : BlockchainAddress
    {
        public string AccountName { get; set; }

        [JsonIgnore]
        public string OwnerPrivateKey { get; set; }

        [JsonIgnore]
        public string OwnerPublicKey { get; set; }

        [JsonIgnore]
        public string ActivePrivateKey { get; set; }

        [JsonIgnore]
        public string ActivePublicKey { get; set; }

        public override bool ShouldSerializeAddress()
        {
            return false;
        }

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
