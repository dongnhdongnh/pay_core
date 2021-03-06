﻿using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities.ETH
{
    [Table("EthereumAddress")]
    public class EthereumAddress : BlockchainAddress
    {
        [JsonIgnore] public string Password { get; set; }

        public override string GetSecret()
        {
            return Password;
        }
    }
}