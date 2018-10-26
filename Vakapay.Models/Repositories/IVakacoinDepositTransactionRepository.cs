using Vakapay.Models.Entities.VAKA;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface
        IVakacoinDepositTransactionRepository : IRepositoryBlockchainTransaction<VakacoinDepositTransaction>
    {
    }
}