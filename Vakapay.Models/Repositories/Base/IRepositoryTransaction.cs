using System.Collections.Generic;
using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface  IRepositoryTransaction
    {
        BlockchainTransaction FindTransactionPending();
		List<BlockchainTransaction> FindTransactionsPending();
		List<BlockchainTransaction> FindTransactionsInProcess();
		List<BlockchainTransaction> FindTransactionsNotCompleteOnNet();
		BlockchainTransaction FindTransactionError();
        BlockchainTransaction FindTransactionByStatus(string status);
		List<BlockchainTransaction> FindTransactionsByStatus(string status);
		Task<ReturnObject> LockForProcess(BlockchainTransaction transaction);
        Task<ReturnObject> ReleaseLock(BlockchainTransaction transaction);
        Task<ReturnObject> SafeUpdate(BlockchainTransaction transaction);
    }
}