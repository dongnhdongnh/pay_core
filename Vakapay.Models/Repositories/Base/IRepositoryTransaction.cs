using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface  IRepositoryTransaction
    {
        IBlockchainTransaction FindTransactionPending();
        IBlockchainTransaction FindTransactionError();
        IBlockchainTransaction FindTransactionByStatus(string status);
        Task<ReturnObject> LockForProcess(IBlockchainTransaction transaction);
        Task<ReturnObject> ReleaseLock(IBlockchainTransaction transaction);
        Task<ReturnObject> SafeUpdate(IBlockchainTransaction transaction);
    }
}