using System;
namespace Vakapay.Models.Entities
{
    public class Wallet
    {
       
        public string Id { get; set; }
        public decimal Balance { get; set; }
        public string UserId { get; set; }
        public string NetworkName { get; set; }
        public string Address { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public int Version { get; set; }
    }
}
