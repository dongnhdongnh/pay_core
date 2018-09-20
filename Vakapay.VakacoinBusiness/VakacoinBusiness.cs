using System;
using System.Data;
using System.Threading.Tasks;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Helpers;
using Vakapay.Cryptography;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Models.Entities;
using Vakapay.Repositories.Mysql;

namespace Vakapay.VakacoinBusiness
{
    using BlockchainBusiness;

    public class VakacoinBusiness : AbsBlockchainBusiness, IBlockchainBusiness
    {
        private IVakacoinDepositTransactionRepository VakacoinDepositRepo { get; set; }
        private VakacoinRPC VakacoinRPCObj { get; set; }

        public VakacoinBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
            : base(vakapayRepositoryFactory, isNewConnection)
        {
            VakacoinDepositRepo = VakapayRepositoryFactory.GetVakacoinDepositTransactionRepository(DbConnection);
        }
        
        public void SetAccountRepositoryForRpc(VakacoinRPC rpc)
        {
            rpc.AccountRepository = (VakacoinAccountRepository) VakapayRepositoryFactory.GetVakacoinAccountRepository(DbConnection);
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
                    CreatedAt = (int) CommonHelper.GetUnixTimestamp(),
                    Id = CommonHelper.GenerateUuid(),
                    UpdatedAt = (int) CommonHelper.GetUnixTimestamp(),
                    WalletId = walletId,
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
        
        /// <summary>
        /// Add existed account to database
        /// save account name to database
        /// </summary>
        /// <returns></returns>
        public ReturnObject AddAccount(string walletId, string accountName, string ownerPrivateKey, string ownerPublicKey, string activePrivateKey = "", string activePublicKey = "")
        {
            try
            {
                var repo = VakapayRepositoryFactory.GetVakacoinAccountRepository(DbConnection);

                if (string.IsNullOrEmpty(activePrivateKey))
                {
                    activePrivateKey = ownerPrivateKey;
                    activePublicKey = ownerPublicKey;
                }

                //TODO Encrypt Password Before save
                var returnObject = repo.Insert(new VakacoinAccount
                {
                    Status = Status.StatusActive,
                    AccountName = accountName,
                    OwnerPrivateKey = ownerPrivateKey,
                    OwnerPublicKey = ownerPublicKey,
                    ActivePrivateKey = activePrivateKey,
                    ActivePublicKey = activePublicKey,
                    CreatedAt = (int) CommonHelper.GetUnixTimestamp(),
                    Id = CommonHelper.GenerateUuid(),
                    UpdatedAt = (int) CommonHelper.GetUnixTimestamp(),
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

        public ReturnObject CreateDepositTransaction(string trxId, int blockNumber, string networkName, decimal amount,
            string fromAddress, string toAddress, decimal fee, string status)
        {
            try
            {
                if (DbConnection.State != ConnectionState.Open)
                    DbConnection.Open();
                int currentTimestamp = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var transaction = new VakacoinDepositTransaction
                {
                    Id = CommonHelper.GenerateUuid(),
                    TrxId = trxId,
                    BlockNumber = blockNumber,
                    NetworkName = networkName,
                    Amount = amount,
                    FromAddress = fromAddress,
                    ToAddress = toAddress,
                    Fee = fee,
                    Status = status,
                    CreatedAt = currentTimestamp,
                    UpdatedAt = currentTimestamp,
                    InProcess = 0,
                    Version = 0
                };
                var result = VakacoinDepositRepo.Insert(transaction);
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
        
        public ReturnObject FakePendingTransaction(VakacoinWithdrawTransaction blockchainTransaction)
        {
            try
            {
                var vakacoinwithdrawRepo = VakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(DbConnection);
                blockchainTransaction.Id = CommonHelper.GenerateUuid();
                blockchainTransaction.Status = Status.StatusPending;
                blockchainTransaction.CreatedAt = (int)CommonHelper.GetUnixTimestamp();
                blockchainTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                return vakacoinwithdrawRepo.Insert(blockchainTransaction);
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

        public ReturnObject SendTransaction(string From, string To, decimal amount)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SendTransaction(string From, string To, decimal amount, string Password)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SendMultiTransaction(string From, string[] To, decimal amount)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SignData(string data, string privateKey)
        {
            throw new NotImplementedException();
        }

        public ReturnObject CreateNewAddress(string WalletId, string password = "")
        {
            throw new NotImplementedException();
        }

        public Task<ReturnObject> SendTransactionAsysn(string From, string To, decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}