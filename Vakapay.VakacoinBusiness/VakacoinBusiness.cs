using System;
using System.Data;
using System.Threading.Tasks;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Constants;
using Vakapay.Cryptography;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Models.Entities.VAKA;
using Vakapay.Models.Repositories.Base;
using Vakapay.Repositories.Mysql;

//using ReturnObject=Vakapay.Models.Domains.ReturnObject;

namespace Vakapay.VakacoinBusiness
{
    using BlockchainBusiness;
    using System.Collections.Generic;

    public class VakacoinBusiness : AbsBlockchainBusiness, IBlockchainBusiness
    {
       // private IVakacoinDepositTransactionRepository VakacoinDepositRepo { get; set; }
        private VakacoinRpc VakacoinRpcObj { get; set; }

        public VakacoinBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
            : base(vakapayRepositoryFactory, isNewConnection)
        {
          //  VakacoinDepositRepo = VakapayRepositoryFactory.GetVakacoinDepositTransactionRepository(DbConnection);
        }

        public void SetAccountRepositoryForRpc(VakacoinRpc rpc)
        {
            using (var rep = (VakacoinAccountRepository)VakapayRepositoryFactory.GetVakacoinAccountRepository(DbConnection))
            {
                rpc.AccountRepository = rep;
            }

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

                var resultRpc = VakacoinRpcObj.CreateRandomAccount(keyPair.PublicKey);

                if (resultRpc.Status == Status.STATUS_ERROR)
                    return resultRpc;

                using (var repo = VakapayRepositoryFactory.GetVakacoinAccountRepository(DbConnection))
                {

                    //TODO Encrypt Password Before save
                    var returnDb = repo.Insert(new VakacoinAccount
                    {
                        Status = Status.STATUS_ACTIVE,
                        Address = resultRpc.Data,
                        OwnerPrivateKey = keyPair.PrivateKey,
                        OwnerPublicKey = keyPair.PublicKey,
                        ActivePrivateKey = keyPair.PrivateKey,
                        ActivePublicKey = keyPair.PublicKey,
                        WalletId = walletId,
                    });


                    return returnDb.Status == Status.STATUS_SUCCESS ? resultRpc : returnDb;
                }
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
                using (var repo = VakapayRepositoryFactory.GetVakacoinAccountRepository(DbConnection))
                {

                    if (string.IsNullOrEmpty(activePrivateKey))
                    {
                        activePrivateKey = ownerPrivateKey;
                        activePublicKey = ownerPublicKey;
                    }

                    //TODO Encrypt Password Before save
                    var returnObject = repo.Insert(new VakacoinAccount
                    {
                        Status = Status.STATUS_ACTIVE,
                        Address = accountName,
                        OwnerPrivateKey = ownerPrivateKey,
                        OwnerPublicKey = ownerPublicKey,
                        ActivePrivateKey = activePrivateKey,
                        ActivePublicKey = activePublicKey,
                        WalletId = walletId
                    });

                    return returnObject;
                }
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

        public ReturnObject CreateDepositTransaction(string trxId, int blockNumber, string networkName, decimal amount,
            string fromAddress, string toAddress, decimal fee, string status)
        {
            try
            {
                using (var _rep = VakapayRepositoryFactory.GetVakacoinDepositTransactionRepository(DbConnection))
                {
                    if (DbConnection.State != ConnectionState.Open)
                        DbConnection.Open();
                    var transaction = new VakacoinDepositTransaction
                    {
                        TrxId = trxId,
                        BlockNumber = blockNumber,
                        Amount = amount,
                        FromAddress = fromAddress,
                        ToAddress = toAddress,
                        Fee = fee,
                        Status = status,
                        IsProcessing = 0,
                        Version = 0
                    };
                    var result = _rep.Insert(transaction);
                    return result;
                }
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

        public ReturnObject FakePendingTransaction(VakacoinWithdrawTransaction blockchainTransaction)
        {
            try
            {
                using (var vakacoinwithdrawRepo =VakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(DbConnection))
                {
                    blockchainTransaction.Status = Status.STATUS_PENDING;
                    return vakacoinwithdrawRepo.Insert(blockchainTransaction);
                }
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

        public ReturnObject SendTransaction(string from, string to, decimal amount)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SendTransaction(string @from, string to, decimal amount, string password)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SendMultiTransaction(string from, string[] to, decimal amount)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SignData(string data, string privateKey)
        {
            throw new NotImplementedException();
        }

        public ReturnObject CreateNewAddress(string walletId, string password = "")
        {
            throw new NotImplementedException();
        }

        public Task<ReturnObject> SendTransactionAsysn(string from, string to, decimal amount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Created Address with optimistic lock
        /// </summary>
        /// <param name="repoQuery"></param>
        /// <param name="rpcClass"></param>
        /// <param name="walletId"></param>
        /// <param name="other"></param>
        /// <typeparam name="TBlockchainAddress"></typeparam>
        /// <returns></returns>
        public override async Task<ReturnObject> CreateAddressAsync<TBlockchainAddress>(
            IAddressRepository<TBlockchainAddress> repoQuery,
            IBlockchainRpc rpcClass, string walletId, string other = "")
        {
            //return base.CreateAddressAsync(wallet, repoQuery, rpcClass, walletId, other);

            try
            {
                using (var walletRepository = VakapayRepositoryFactory.GetWalletRepository(DbConnection))
                {

                    var walletCheck = walletRepository.FindById(walletId);

                    if (walletCheck == null)
                        return new ReturnObject
                        {
                            Status = Status.STATUS_ERROR,
                            Message = "Wallet Not Found"
                        };

                    VakacoinRpcObj = rpcClass as VakacoinRpc;

                    var results =
                        CreateNewAccount(walletId); // Create account and add account name to VakacoinAccount table

                    if (results.Status == Status.STATUS_ERROR)
                        return results;

                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Message = "Create vakacoin account success!"
                    };
                }
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
            using (var withdrawRepo = VakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(DbConnection))
            {
                return GetHistory<VakacoinWithdrawTransaction>(withdrawRepo, offset, limit, orderBy);
            }
        }

        public override List<BlockchainTransaction> GetDepositHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            using (var depositRepo = VakapayRepositoryFactory.GetVakacoinDepositTransactionRepository(DbConnection))
            {
                return GetHistory<VakacoinDepositTransaction>(depositRepo, offset, limit, orderBy);
            }
        }

        public override List<BlockchainTransaction> GetAllHistory(out int numberData, string userId, string currency,
            int offset = -1, int limit = -1, string[] orderBy = null, string search = null,long day=-1)
        {
            using (var depositRepo = VakapayRepositoryFactory.GetVakacoinDepositTransactionRepository(DbConnection))
            {
                using (var withdrawRepo = VakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(DbConnection))
                {
                    using (var inter = VakapayRepositoryFactory.GetInternalTransactionRepository(DbConnection))
                    {
                        return GetAllHistory<VakacoinWithdrawTransaction, VakacoinDepositTransaction>(out numberData, userId,
                            currency, withdrawRepo, depositRepo, inter.GetTableName(), offset, limit, orderBy, search);
                    }
                }
            }
        }
    }
}