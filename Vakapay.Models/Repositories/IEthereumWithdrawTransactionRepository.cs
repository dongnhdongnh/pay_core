using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
	public interface IEthereumWithdrawTransactionRepository : IRepositoryBase<EthereumWithdrawTransaction>
	{
		string Query_Search(object whereValue);
		string Query_Update(object updateValue, object whereValue);
		ReturnObject ExcuteSQL(string sqlString);
	}
}
