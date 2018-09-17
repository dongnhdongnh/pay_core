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
        public VakacoinBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
            : base(vakapayRepositoryFactory, isNewConnection)
        {
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

        public ReturnObject CreateTransactionHistory(string from, string to, decimal amount, string transactionTime, string status)
        {
            try
            {
                if(DbConnection.State != ConnectionState.Open)
                    DbConnection.Open();
                var transaction = new VakacoinTransactionHistory
                {
                    Id = CommonHelper.GenerateUuid(),
                    From = from,
                    To = to,
                    Amount = amount,
                    TransactionTime = transactionTime,
                    CreatedTime =  DateTime.UtcNow,
                    Status = status
                };
                var vakacoinRepo = VakapayRepositoryFactory.GetVakacoinTransactionHistoryRepository(DbConnection);

                var result = vakacoinRepo.Insert(transaction);
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
    }
}