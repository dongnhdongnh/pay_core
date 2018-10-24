using System;
using System.Collections.Generic;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Constants;
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
				blockchainTransaction.Status = Status.STATUS_PENDING;
				blockchainTransaction.CreatedAt = (int)CommonHelper.GetUnixTimestamp();
				blockchainTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
				return ethereumwithdrawRepo.Insert(blockchainTransaction);
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
        public ReturnObject FakeDepositTransaction(EthereumDepositTransaction blockchainTransaction)
        {
            try
            {
                var repo = VakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(DbConnection);
              //  blockchainTransaction.Id = CommonHelper.GenerateUuid();
                blockchainTransaction.Status = Status.STATUS_COMPLETED;
                blockchainTransaction.CreatedAt = (int)CommonHelper.GetUnixTimestamp();
                blockchainTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                return repo.Insert(blockchainTransaction);
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

        public override List<BlockchainTransaction> GetWithdrawHistory(int offset = -1, int limit = -1, string[] orderBy = null)
		{
			var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
            Console.WriteLine("Get ETH HISTORY");
			return GetHistory<EthereumWithdrawTransaction>(ethereumwithdrawRepo, offset, limit, orderBy);
		}

		public override List<BlockchainTransaction> GetDepositHistory(int offset = -1, int limit = -1, string[] orderBy = null)
		{
			var ethereumDepositRepo = VakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(DbConnection);
			return GetHistory<EthereumDepositTransaction>(ethereumDepositRepo, offset, limit, orderBy);
		}

       
        public override List<BlockchainTransaction> GetAllHistory(out int numberData,string userID,string currency,int offset = -1, int limit = -1, string[] orderBy = null,string search=null)
        {
            var depositRepo = VakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(DbConnection);
          
            var withdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
            var inter = VakapayRepositoryFactory.GetInternalTransactionRepository(DbConnection);
            return GetAllHistory<EthereumWithdrawTransaction, EthereumDepositTransaction>(out numberData, userID,currency, withdrawRepo, depositRepo,inter.GetTableName(), offset, limit, orderBy, search);
        }
        //public override List<BlockchainTransaction> GetWithdrawHistory()
        //{
        //	var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
        //	return GetHistory<EthereumWithdrawTransaction>(ethereumwithdrawRepo);
        //}
    }
}