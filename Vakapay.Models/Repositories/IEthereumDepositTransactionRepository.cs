using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
	public interface IEthereumDepositTransactionRepository : IRepositoryBlockchainTransaction<EthereumDepositTransaction>
	{
	}
}
