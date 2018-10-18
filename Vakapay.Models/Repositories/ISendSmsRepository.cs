using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface ISendSmsRepository : IRepositoryBase<SmsQueue>, IMultiThreadUpdateEntityRepository<SmsQueue>
    {
//        SmsQueue FindPendingSms();
//        Task<ReturnObject> LockForProcess(SmsQueue sms);
//        Task<ReturnObject> SafeUpdate(SmsQueue sms);
//        Task<ReturnObject> ReleaseLock(SmsQueue sms);
    }
}