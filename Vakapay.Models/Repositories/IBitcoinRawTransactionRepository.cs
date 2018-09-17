using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IBitcoinRawTransactionRepository : IRepositoryBlockchainTransaction<BitcoinWithdrawTransaction>
    {
        List<BitcoinWithdrawTransaction> FindWhere(BitcoinWithdrawTransaction objectTransaction);
    }
}
