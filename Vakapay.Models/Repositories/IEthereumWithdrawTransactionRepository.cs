using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
	public interface IEthereumWithdrawTransactionRepository : IRepositoryBase<EthereumWithdrawTransaction>
	{
		string Query_Search(string SearchName, string SearchData);
        string Query_Update(object updatevalues, object selectorvalue);
    }
}
