using System;
namespace Vakapay.Models.Domains
{
    public abstract class IBlockchainTransaction
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public int BlockNumber { get; set; }
        public string NetworkName { get; set; }
        public decimal Amount { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Fee { get; set; }
        public string Status { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
        public int InProcess { get; set; }
        public int Version { get; set; }
    }
}
