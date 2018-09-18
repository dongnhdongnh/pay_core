using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class VakacoinTransaction : IBlockchainTransaction
    {
        public string Memo { get; set; }
    }
}