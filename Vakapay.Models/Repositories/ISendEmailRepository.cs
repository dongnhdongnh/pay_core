﻿using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;
using System;
namespace Vakapay.Models.Repositories
{
    public interface ISendEmailRepository : IRepositoryBase<EmailQueue>, IMultiThreadUpdateEntityRepository<EmailQueue>,IDisposable
    {
//        EmailQueue FindPendingEmail();
//        Task<ReturnObject> LockForProcess(EmailQueue email);
//        Task<ReturnObject> SafeUpdate(EmailQueue email);
//        Task<ReturnObject> ReleaseLock(EmailQueue email);
    }
}