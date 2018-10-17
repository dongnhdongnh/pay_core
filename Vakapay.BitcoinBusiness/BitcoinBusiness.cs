using System;
using System.Collections.Generic;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities.BTC;
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
                blockchainTransaction.Status = Status.STATUS_PENDING;
                blockchainTransaction.CreatedAt = (int) CommonHelper.GetUnixTimestamp();
                blockchainTransaction.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
                return bitcoinWithDrawRepo.Insert(blockchainTransaction);
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }


        public override List<BlockchainTransaction> GetWithdrawHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            var withdrawRepo = VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);
            return GetHistory<BitcoinWithdrawTransaction>(withdrawRepo, offset, limit, orderBy);
        }



		//public override List<BlockchainTransaction> GetWithdrawHistory(int offset = -1, int limit = -1, string[] orderBy = null)
		//{
		//	var withdrawRepo = VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);
		//	return GetHistory<BitcoinWithdrawTransaction>(withdrawRepo, offset, limit, orderBy);
		//}

		public override List<BlockchainTransaction> GetDepositHistory(int offset = -1, int limit = -1, string[] orderBy = null)
		{
			var depositRepo = VakapayRepositoryFactory.GetBitcoinDepositTransactionRepository(DbConnection);
			return GetHistory<BitcoinDepositTransaction>(depositRepo, offset, limit, orderBy);
		}

        public override List<BlockchainTransaction> GetAllHistory(out int numberData,string walletAdd,int offset = -1, int limit = -1, string[] orderBy = null)
        {
            var depositRepo = VakapayRepositoryFactory.GetBitcoinDepositTransactionRepository(DbConnection);
            var withdrawRepo = VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);
            return GetAllHistory<BitcoinWithdrawTransaction,BitcoinDepositTransaction>(out numberData,walletAdd, withdrawRepo, depositRepo, offset, limit, orderBy);
        }
    }
}