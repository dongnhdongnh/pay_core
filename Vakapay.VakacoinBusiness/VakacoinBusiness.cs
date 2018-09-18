using System;
using System.Data;
using Vakapay.Commons.Helpers;
using Vakapay.Cryptography;
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
        private VakacoinRPC VakacoinRPCObj { get; set; }

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
        public ReturnObject CreateNewAccount(string walletId)
        {
            try
            {
                var keyPair = KeyManager.GenerateKeyPair();
                
                var result = VakacoinRPCObj.CreateRandomAccount(keyPair.PublicKey);
                
                if (result.Status == Status.StatusError)
                    return result;

                var repo = VakapayRepositoryFactory.GetVakacoinAccountRepository(DbConnection);

                //TODO Encrypt Password Before save
                var returnObject = repo.Insert(new VakacoinAccount
                {
                    Status = Status.StatusActive,
                    AccountName = result.Data,
                    OwnerPrivateKey = keyPair.PrivateKey,
                    OwnerPublicKey = keyPair.PublicKey,
                    ActivePrivateKey = keyPair.PrivateKey,
                    ActivePublicKey = keyPair.PublicKey,
                    CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    Id = CommonHelper.GenerateUuid(),
                    UpdatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    WalletId = walletId

                });
                
                return returnObject;
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