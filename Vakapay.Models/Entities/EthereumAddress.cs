using System;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities

{
    public class EthereumAddress : BlockchainAddress
    {
        public string Password { get; set; }
    }
}