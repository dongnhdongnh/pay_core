using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IInternalTransactionRepository:IRepositoryBlockchainTransaction<InternalWithdrawTransaction>
    {
    }
}
