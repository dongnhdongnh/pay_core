using Vakapay.Models.Domains;

namespace Vakapay.Models.Entities
{
    public class VakacoinTransaction : BlockchainTransaction
    {
        public string Memo { get; set; }
    }
}