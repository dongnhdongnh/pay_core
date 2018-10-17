using System.Collections.Generic;
using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface IMultiThreadUpdateEntityRepository<TEntity>
    {
        TEntity FindRowPending();
        List<TEntity> FindRowsPending();
        List<TEntity> FindRowsInProcess();
//        List<TEntity> FindRowsNotCompleteOnNet();
        TEntity FindRowError();
        TEntity FindRowByStatus(string status);
        List<TEntity> FindRowsByStatus(string status);
        Task<ReturnObject> LockForProcess(TEntity row);
        Task<ReturnObject> ReleaseLock(TEntity row);
        Task<ReturnObject> SafeUpdate(TEntity row);
    }
}