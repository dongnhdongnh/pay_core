using System;
using System.Collections.Generic;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

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

		public override List<BlockchainTransaction> GetWithdrawHistory(int offset = -1, int limit = -1, string[] orderBy = null)
		{
			var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
			return GetHistory<EthereumWithdrawTransaction>(ethereumwithdrawRepo, offset, limit, orderBy);
		}

		public override List<BlockchainTransaction> GetDepositHistory(int offset = -1, int limit = -1, string[] orderBy = null)
		{
			var ethereumDepositRepo = VakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(DbConnection);
			return GetHistory<EthereumDepositTransaction>(ethereumDepositRepo, offset, limit, orderBy);
		}



		//public override List<BlockchainTransaction> GetWithdrawHistory()
		//{
		//	var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
		//	return GetHistory<EthereumWithdrawTransaction>(ethereumwithdrawRepo);
		//}
	}
}