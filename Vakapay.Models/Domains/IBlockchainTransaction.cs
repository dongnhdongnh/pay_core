using System;
namespace Vakapay.Models.Domains
{
    public interface IBlockchainTransaction
    {
        string Id { get; set; }
        string Hash { get; set; }
        string BlockNumber { get; set; }
        string NetworkName { get; set; }
        decimal Amount { get; set; }
        string FromAddress { get; set; }
        string ToAddress { get; set; }
        decimal Fee { get; set; }
        string Status { get; set; }
        string CreatedAt { get; set; }
        string UpdatedAt { get; set; }
    }
}
