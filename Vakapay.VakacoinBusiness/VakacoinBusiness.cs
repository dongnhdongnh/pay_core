using System;
using System.Data;
using System.Threading.Tasks;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Helpers;
using Vakapay.Cryptography;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;
using Vakapay.Repositories.Mysql;

//using ReturnObject=Vakapay.Models.Domains.ReturnObject;

namespace Vakapay.VakacoinBusiness
{
    using BlockchainBusiness;
    using System.Collections.Generic;

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
            rpc.AccountRepository =
                (VakacoinAccountRepository)VakapayRepositoryFactory.GetVakacoinAccountRepository(DbConnection);
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

                var resultRpc = VakacoinRPCObj.CreateRandomAccount(keyPair.PublicKey);

                if (resultRpc.Status == Status.StatusError)
                    return resultRpc;

                var repo = VakapayRepositoryFactory.GetVakacoinAccountRepository(DbConnection);

                //TODO Encrypt Password Before save
                var returnDb = repo.Insert(new VakacoinAccount
                {
                    Status = Status.StatusActive,
                    AccountName = resultRpc.Data,
                    OwnerPrivateKey = keyPair.PrivateKey,
                    OwnerPublicKey = keyPair.PublicKey,
                    ActivePrivateKey = keyPair.PrivateKey,
                    ActivePublicKey = keyPair.PublicKey,
                    CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    Id = CommonHelper.GenerateUuid(),
                    UpdatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    WalletId = walletId,
                });

                return returnDb.Status == Status.StatusSuccess ? resultRpc : returnDb;
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
        public ReturnObject AddAccount(string walletId, string accountName, string ownerPrivateKey,
            string ownerPublicKey, string activePrivateKey = "", string activePublicKey = "")
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

        public ReturnObject CreateDepositTransaction(string trxId, int blockNumber, string networkName, decimal amount,
            string fromAddress, string toAddress, decimal fee, string status)
        {
            try
            {
                if (DbConnection.State != ConnectionState.Open)
                    DbConnection.Open();
                var transaction = new VakacoinDepositTransaction
                {
                    Id = CommonHelper.GenerateUuid(),
                    TrxId = trxId,
                    BlockNumber = blockNumber,
                    Amount = amount,
                    FromAddress = fromAddress,
                    ToAddress = toAddress,
                    Fee = fee,
                    Status = status,
                    CreatedAt = CommonHelper.GetUnixTimestamp(),
                    UpdatedAt = CommonHelper.GetUnixTimestamp(),
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
                var vakacoinwithdrawRepo =
                    VakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(DbConnection);
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

        /// <summary>
        /// Created Address with optimistic lock
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="repoQuery"></param>
        /// <param name="rpcClass"></param>
        /// <param name="walletId"></param>
        /// <param name="other"></param>
        /// <typeparam name="TBlockchainAddress"></typeparam>
        /// <returns></returns>
        public override async Task<ReturnObject> CreateAddressAsync<TBlockchainAddress>(IWalletBusiness wallet,
            IAddressRepository<TBlockchainAddress> repoQuery,
            IBlockchainRPC rpcClass, string walletId, string other = "")
        {
            //return base.CreateAddressAsync(wallet, repoQuery, rpcClass, walletId, other);

            try
            {
                var walletRepository = VakapayRepositoryFactory.GetWalletRepository(DbConnection);

                var walletCheck = walletRepository.FindById(walletId);

                if (walletCheck == null)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Wallet Not Found"
                    };

                VakacoinRPCObj = rpcClass as VakacoinRPC;

                var results = CreateNewAccount(walletId); // Create account and add account name to VakacoinAccount table

                if (results.Status == Status.StatusError)
                    return results;

                var accountName = results.Data;

                //update address into wallet db
                //wallet.WalletBusiness(VakapayRepositoryFactory);
                var updateWallet =
                    wallet.UpdateAddressForWallet(walletId, accountName);

                if (updateWallet.Status == Status.StatusError)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Update address fail to WalletDB"
                    };
                }

                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Message = "Create vakacoin account success!"
                };
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

        public override List<BlockchainTransaction> GetWithdrawHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            var withdrawRepo = VakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(DbConnection);
            return GetHistory<VakacoinWithdrawTransaction>(withdrawRepo, offset, limit, orderBy);
        }

        public override List<BlockchainTransaction> GetDepositHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            var depositRepo = VakapayRepositoryFactory.GetVakacoinDepositTransactionRepository(DbConnection);
            return GetHistory<VakacoinDepositTransaction>(depositRepo, offset, limit, orderBy);
        }

        public override List<BlockchainTransaction> GetAllHistory(out int numberData,string walletAdress,int offset = -1, int limit = -1, string[] orderBy = null)
        {
            var depositRepo = VakapayRepositoryFactory.GetVakacoinDepositTransactionRepository(DbConnection);
            var withdrawRepo = VakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(DbConnection);
            return GetAllHistory<VakacoinWithdrawTransaction,VakacoinDepositTransaction>(out numberData,walletAdress, withdrawRepo, depositRepo, offset, limit, orderBy);
        }
    }
}