using System;
namespace Vakapay.Models.Entities
{
    public class EthereumAddress
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string WalletId { get; set; }
        public int Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
    }
}
