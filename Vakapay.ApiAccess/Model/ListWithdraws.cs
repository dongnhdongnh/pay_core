using System.Collections.Generic;
using Vakapay.Models.Domains;

namespace Vakapay.ApiAccess.Model
{
    public class ListWithdraws
    {
        public List<BlockchainTransaction> listWithdraws { get; set; }
        public int total { get; set; }
    }
}