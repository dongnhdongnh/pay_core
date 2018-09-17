using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
	public interface IEthereumWithdrawTransactionRepository : IRepositoryBlockchainTransaction<EthereumWithdrawTransaction>
	{
		string Query_Search(string SearchName, string SearchData);
		string Query_Update(object updatevalues, object selectorvalue);
		ReturnObject ExcuteSQL(string sqlString);
	}
}
