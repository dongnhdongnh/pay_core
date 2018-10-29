using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities.VAKA
{
    [Table("VakacoinAccount")]
    public class VakacoinAccount : BlockchainAddress
    {
        [JsonIgnore] public string OwnerPrivateKey { get; set; }

        [JsonIgnore] public string OwnerPublicKey { get; set; }

        [JsonIgnore] public string ActivePrivateKey { get; set; }

        [JsonIgnore] public string ActivePublicKey { get; set; }

        public override string GetSecret()
        {
            return ActivePrivateKey;
        }
    }
}