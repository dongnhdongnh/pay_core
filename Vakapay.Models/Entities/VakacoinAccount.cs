using System;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class VakacoinAccount : BlockchainAddress, IBlockchainAddress
    {
        public string AccountName { get; set; }
        public string OwnerPrivateKey { get; set; }
        public string OwnerPublicKey { get; set; }
        public string ActivePrivateKey { get; set; }
        public string ActivePublicKey { get; set; }
        
        public string GetAddress()
        {
            return AccountName;
        }

        public string GetSecret()
        {
            return ActivePrivateKey;
        }
    }
}
