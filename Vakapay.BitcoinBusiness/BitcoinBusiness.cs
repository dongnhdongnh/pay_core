using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using NLog;
using Vakapay.BlockchainBusiness;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.BitcoinBusiness
{
	public class BitcoinBusiness : AbsBlockchainBusiness
	{
		public BitcoinBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) :
			base(vakapayRepositoryFactory, isNewConnection)
		{
			// <summary>
		}

		public ReturnObject FakePendingTransaction(BitcoinWithdrawTransaction blockchainTransaction)
		{
			try
			{
				var bitcoinWithDrawRepo =
					VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);
				blockchainTransaction.Id = CommonHelper.GenerateUuid();
				blockchainTransaction.Status = Status.StatusPending;
				blockchainTransaction.CreatedAt = (int)CommonHelper.GetUnixTimestamp();
				blockchainTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
				return bitcoinWithDrawRepo.Insert(blockchainTransaction);
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
			var withdrawRepo = VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);
			return GetHistory<BitcoinWithdrawTransaction>(withdrawRepo, offset, limit, orderBy);
		}

		public override List<BlockchainTransaction> GetDepositHistory(int offset = -1, int limit = -1, string[] orderBy = null)
		{
			var depositRepo = VakapayRepositoryFactory.GetBitcoinDepositTransactionRepository(DbConnection);
			return GetHistory<BitcoinDepositTransaction>(depositRepo, offset, limit, orderBy);
		}
	}
}