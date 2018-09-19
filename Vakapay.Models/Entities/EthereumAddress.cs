using System;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class EthereumAddress : BlockchainAddress, IBlockchainAddress
    {

        public string Password { get; set; }
        
        public string GetAddress()
        {
            return Address;
        }

        public string GetSecret()
        {
            return Password;
        }
    }
}
