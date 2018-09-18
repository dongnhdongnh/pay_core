using System;
namespace Vakapay.Models.Entities
{
    public class VakacoinAccount
    {
        public string Id { get; set; }
        public string AccountName { get; set; }
        public string OwnerPrivateKey { get; set; }
        public string OwnerPublicKey { get; set; }
        public string ActivePrivateKey { get; set; }
        public string ActivePublicKey { get; set; }
        public string WalletId { get; set; }
        public string Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
    }
}
