using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IEmailRepository : IRepositoryBlockchainTransaction<EmailQueue>
    {
        List<BitcoinDepositTransaction> FindWhere(EmailQueue emailQueue);
    }
}