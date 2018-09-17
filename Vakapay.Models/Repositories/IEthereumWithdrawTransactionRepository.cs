using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{

	public interface IEthereumWithdrawTransactionRepository : IRepositoryBlockchainTransaction<EthereumWithdrawTransaction>
	{
		string Query_Search(Dictionary<string, string> whereValue);
		string Query_Update(object updateValue, object whereValue);
		ReturnObject ExcuteSQL(string sqlString);
		
	}
}
