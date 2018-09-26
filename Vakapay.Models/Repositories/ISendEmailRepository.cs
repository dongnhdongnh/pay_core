using System.Collections.Generic;
using System.Threading.Tasks;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface ISendEmailRepository : IRepositoryBase<EmailQueue>
    {
        EmailQueue FindPendingEmail();
        Task<ReturnObject> LockForProcess(EmailQueue email);
        Task<ReturnObject> SafeUpdate(EmailQueue email);
        Task<ReturnObject> ReleaseLock(EmailQueue email);
    }
}