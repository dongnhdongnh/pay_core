using System.Collections.Generic;
using Vakapay.Models.Domains;

namespace Vakapay.ApiAccess.Model
{
    public class ListWithdraws
    {
        public List<BlockchainTransaction> ListTransactions { get; set; }
        public int Total { get; set; }
    }
}