using System;
using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{   
    [Table("bitcoinaddress")]
    public class BitcoinAddress : BlockchainAddress, IBlockchainAddress
    {
       
        public string GetAddress()
        {
            return Address;
        }

        public string GetSecret()
        {
            throw new NotImplementedException();
        }
    }
}