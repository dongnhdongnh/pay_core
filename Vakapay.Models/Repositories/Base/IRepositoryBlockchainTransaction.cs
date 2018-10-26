using System.Collections.Generic;

namespace Vakapay.Models.Repositories.Base
{
    public interface IRepositoryBlockchainTransaction<TBlockchainTransaction> :
        IRepositoryTransaction<TBlockchainTransaction>,
        IRepositoryBase<TBlockchainTransaction>
    {
        List<TBlockchainTransaction> FindTransactionsNotCompleteOnNet();
    }
}