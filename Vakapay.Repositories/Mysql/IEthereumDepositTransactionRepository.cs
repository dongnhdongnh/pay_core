using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;

namespace Vakapay.Repositories.Mysql
{
	public interface IEthereumDepositTransactionRepository
	{
		ReturnObject Delete(string Id);
		EthereumDepositTransaction FindById(string Id);
		List<EthereumDepositTransaction> FindBySql(string sqlString);
		ReturnObject Insert(EthereumDepositTransaction objectInsert);
		ReturnObject Update(EthereumDepositTransaction objectUpdate);
	}
}