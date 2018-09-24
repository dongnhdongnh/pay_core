using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vakapay.BlockchainBusiness;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.EthereumBusiness
{
	public class EthereumBusiness : AbsBlockchainBusiness
	{
		public EthereumBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) : base(vakapayRepositoryFactory, isNewConnection)
		{
		}
		public ReturnObject FakePendingTransaction(EthereumWithdrawTransaction blockchainTransaction)
		{
			try
			{
				var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
				blockchainTransaction.Id = CommonHelper.GenerateUuid();
				blockchainTransaction.Status = Status.StatusPending;
				blockchainTransaction.CreatedAt = (int)CommonHelper.GetUnixTimestamp();
				blockchainTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
				return ethereumwithdrawRepo.Insert(blockchainTransaction);
			}
			catch (Exception e)
			{

				return new ReturnObject
				{
					Status = Status.StatusError,
					Message = e.Message
				};
			}
		}

		public override List<BlockchainTransaction> GetWithdrawHistory()
		{
			var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
			return GetHistory<EthereumWithdrawTransaction>(ethereumwithdrawRepo);
		}

		public override List<BlockchainTransaction> GetDepositHistory()
		{
			var ethereumDepositRepo = VakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(DbConnection);
			return GetHistory<EthereumDepositTransaction>(ethereumDepositRepo);
		}



		//public override List<BlockchainTransaction> GetWithdrawHistory()
		//{
		//	var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
		//	return GetHistory<EthereumWithdrawTransaction>(ethereumwithdrawRepo);
		//}
	}
}