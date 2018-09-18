using System;
using System.Data;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Models.Entities;

namespace Vakapay.VakacoinBusiness
{
    using BlockchainBusiness;

    public class VakacoinBusiness : BlockchainBusiness
    {
        private IVakacoinTransactionHistoryRepository VakacoinHistoryRepo { get; set; }
        private IPendingVakacoinTransactionRepository PendingVakacoinTransRepo { get; set; }

        public VakacoinBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
            : base(vakapayRepositoryFactory, isNewConnection)
        {
            VakacoinHistoryRepo = VakapayRepositoryFactory.GetVakacoinTransactionHistoryRepository(DbConnection);
            PendingVakacoinTransRepo = VakapayRepositoryFactory.GetPendingVakacoinTransactionRepository(DbConnection);
        }

        /// <summary>
        /// call RPC Vakacoin to create a new account
        /// save account name to database
        /// </summary>
        /// <returns></returns>
        public ReturnObject CreateNewAccount()
        {
            try
            {
                return new ReturnObject { };
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

        public ReturnObject CreateTransactionHistory(string trxId, string from, string to, int blockNumber,
            decimal amount, string transactionTime, string status)
        {
            try
            {
                if (DbConnection.State != ConnectionState.Open)
                    DbConnection.Open();
                var transaction = new VakacoinTransactionHistory
                {
                    Id = CommonHelper.GenerateUuid(),
                    TrxId = trxId,
                    From = from,
                    To = to,
                    BlockNumber = blockNumber,
                    Amount = amount,
                    TransactionTime = transactionTime,
                    CreatedTime = DateTime.UtcNow,
                    Status = status
                };
                var result = VakacoinHistoryRepo.Insert(transaction);
                return result;
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

//        public ReturnObject 
    }
}