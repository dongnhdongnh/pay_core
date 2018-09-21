using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{

	public interface IEthereumWithdrawTransactionRepository : IRepositoryBlockchainTransaction<EthereumWithdrawTransaction>
	{
		
	
	}
}
