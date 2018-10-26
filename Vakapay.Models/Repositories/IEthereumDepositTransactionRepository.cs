using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface
        IEthereumDepositTransactionRepository : IRepositoryBlockchainTransaction<EthereumDepositTransaction>
    {
    }
}