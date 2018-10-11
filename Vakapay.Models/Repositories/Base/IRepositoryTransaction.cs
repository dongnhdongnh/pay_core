using System.Collections.Generic;
using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
	public interface IRepositoryTransaction
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
		List<BlockchainTransaction> FindTransactionHistory(int offset, int limit, string[] orderBy);
		List<BlockchainTransaction> FindTransactionsByUserId(string userId);
		List<BlockchainTransaction> FindTransactionsFromAddress(string fromAddress);
		List<BlockchainTransaction> FindTransactionsToAddress(string toAddress);
		List<BlockchainTransaction> FindTransactionsInner(string innerAddress);
	}
}