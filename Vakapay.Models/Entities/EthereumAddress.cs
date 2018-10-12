﻿using System.ComponentModel.DataAnnotations.Schema;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    [Table("EthereumAddress")]
    public class EthereumAddress : BlockchainAddress
    {
        public override string GetAddress()
        {
            return Address;
        }

        public override string GetSecret()
        {
            return Password;
        }
    }
}
