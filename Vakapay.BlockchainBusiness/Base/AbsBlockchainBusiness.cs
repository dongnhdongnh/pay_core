using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;


namespace Vakapay.BlockchainBusiness.Base
{
    public abstract class AbsBlockchainBusiness
    {
        public IVakapayRepositoryFactory VakapayRepositoryFactory { get; }
        public IDbConnection DbConnection { get; }


        public AbsBlockchainBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            VakapayRepositoryFactory = vakapayRepositoryFactory;
            DbConnection = isNewConnection
                ? VakapayRepositoryFactory.GetDbConnection()
                : VakapayRepositoryFactory.GetOldConnection();
        }

        /// <summary>
        /// Send Transaction with optimistic lock
        /// Send with multiple thread
        /// </summary>
        /// <param name="repoQuery"></param>
        /// <param name="rpcClass"></param>
        /// <param name="privateKey"></param>
        /// <typeparam name="TBlockchainTransaction"></typeparam>
        /// <returns></returns>
        public virtual async Task<ReturnObject> SendTransactionAsync<TBlockchainTransaction>(
            IRepositoryBlockchainTransaction<TBlockchainTransaction> repoQuery, IBlockchainRpc rpcClass,
            string privateKey = "") where TBlockchainTransaction : BlockchainTransaction
        {
            /*
             * 1. Query Transaction Withdraw pending
             * 2. Update Processing = 1, version = version + 1
             * 3. Commit Transaction
             * 4. Call RPC send transaction
             * 5. Update Transaction Status
             */
            // find transaction pending
//            var pendingTransaction = repoQuery.FindTransactionPending();
            var pendingTransaction = repoQuery.FindRowPending();

            if (pendingTransaction?.Id == null)
            {
                //if (!CacheHelper.HaveKey("cache"))
                //{
                //	long numberOfTicks = (DateTime.Now.Ticks - startTime);

                //	TimeSpan ts = TimeSpan.FromTicks(numberOfTicks);
                //	double minutesFromTs = ts.TotalMinutes;
                //	CacheHelper.SetCacheString("cache", minutesFromTs.ToString());

                //}
                //Console.WriteLine("END TIME " + CacheHelper.GetCacheString("cache"));
                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Message = "Not found Transaction"
                };
            }

            if (DbConnection.State != ConnectionState.Open)
            {
                //Console.WriteLine(DbConnection.State);
                DbConnection.Open();
            }


            //begin first transaction
            var transactionScope = DbConnection.BeginTransaction();
            try
            {
                //lock transaction for process
                var resultLock = await repoQuery.LockForProcess(pendingTransaction);
                if (resultLock.Status == Status.STATUS_ERROR)
                {
                    transactionScope.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Message = "Cannot Lock For Process"
                    };
                }

                //commit transaction
                transactionScope.Commit();
            }
            catch (Exception e)
            {
                transactionScope.Rollback();
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.ToString()
                };
            }

            //Update Version to Model
            pendingTransaction.Version += 1;

            //start send and update

            var transactionDbSend = DbConnection.BeginTransaction();
            try
            {
                //Call RPC Transaction
                //TODO EDIT RPC Class
                var sendTransaction = await rpcClass.SendTransactionAsync(pendingTransaction);
                pendingTransaction.Status = sendTransaction.Status;
                pendingTransaction.IsProcessing = 0;
//                pendingTransaction.UpdatedAt = (int) CommonHelper.GetUnixTimestamp(); // set in SafeUpdate
                pendingTransaction.Hash = sendTransaction.Data;

                //create database email when send success
                if (sendTransaction.Status == Status.STATUS_SUCCESS)
                {
                    var email = GetEmailByTransaction(pendingTransaction);
                    if (email != null)
                    {
                        //                        await CreateDataEmail("Notify send " + pendingTransaction.NetworkName(),
                        //                            email, pendingTransaction.Amount,
                        //                            Constants.TEMPLATE_EMAIL_SENT, pendingTransaction.NetworkName(),Constants.TYPE_SEND);
                        await CreateDataEmail("Notify send " + pendingTransaction.NetworkName(),
                            email, pendingTransaction.Amount, pendingTransaction.Id,
                            EmailTemplate.Sent, pendingTransaction.NetworkName());
                    }
                }

                var result = await repoQuery.SafeUpdate(pendingTransaction);
                if (result.Status == Status.STATUS_ERROR)
                {
                    transactionDbSend.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Cannot Save Transaction Status"
                    };
                }

                transactionDbSend.Commit();
                return new ReturnObject
                {
                    Status = sendTransaction.Status,
                    Message = sendTransaction.Message,
                    Data = sendTransaction.Data
                };
            }
            catch (Exception)
            {
                //release lock
                transactionDbSend.Rollback();
                var resultRelease = repoQuery.ReleaseLock(pendingTransaction);
                Console.WriteLine(JsonHelper.SerializeObject(resultRelease));
                throw;
            }
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
        public virtual async Task<ReturnObject> CreateAddressAsync<TBlockchainAddress>(
            IAddressRepository<TBlockchainAddress> repoQuery, IBlockchainRpc rpcClass, string walletId,
            string other = "") where TBlockchainAddress : BlockchainAddress
        {
            try
            {
                var walletRepository = VakapayRepositoryFactory.GetWalletRepository(DbConnection);

                var walletCheck = walletRepository.FindById(walletId);

                if (walletCheck == null)
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Wallet Not Found"
                    };

                var resultsRPC = rpcClass.CreateNewAddress(other);
                if (resultsRPC.Status == Status.STATUS_ERROR)
                    return resultsRPC;

                var address = resultsRPC.Data;

                //	TBlockchainAddress _newAddress = new TBlockchainAddress();

                var resultDB = await repoQuery.InsertAddress(address, walletId, other);

                //
                //if (result.Status == Status.StatusError)
                //{
                //	return new ReturnObject
                //	{
                //		Status = Status.StatusSuccess,
                //		Message = "Cannot Insert Address"
                //	};
                //}

                //update address into wallet db
                //wallet.WalletBusiness(VakapayRepositoryFactory);

                return new ReturnObject
                {
                    Status = resultDB.Status,
                    Message = resultDB.Message
                };
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
        /// get history from both withdrawn and deposit table
        /// </summary>
        /// <param name="tableInternalWithdrawName"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="orderBy"></param>
        /// <param name="numberData"></param>
        /// <param name="currency"></param>
        /// <param name="withdrawRepository"></param>
        /// <param name="search"></param>
        /// <param name="userID"></param>
        /// <param name="depositRepository"></param>
        /// <returns></returns>
        public virtual List<BlockchainTransaction> GetAllHistory<T1, T2>(out int numberData, string userID,
            string currency,
            IRepositoryBlockchainTransaction<T1> withdrawRepository,
            IRepositoryBlockchainTransaction<T2> depositRepository, string tableInternalWithdrawName, int offset = -1,
            int limit = -1,
            string[] orderBy = null, string search = null)
        {
            try
            {
                Console.WriteLine("FIND HISTORY FROM ABS");
                return withdrawRepository.FindTransactionHistoryAll(out numberData, userID, currency,
                    withdrawRepository.GetTableName(), depositRepository.GetTableName(), tableInternalWithdrawName,
                    offset, limit, orderBy, search);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        /// <summary>
        /// get history from withdrawn or deposit table
        /// </summary>
        /// <typeparam name="TBlockchainTransaction"></typeparam>
        /// <param name="repoQuery">withdrawn or deposit</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public virtual List<BlockchainTransaction> GetHistory<TBlockchainTransaction>(
            IRepositoryBlockchainTransaction<TBlockchainTransaction> repoQuery, int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            try
            {
                Console.WriteLine("FIND HISTORY FROM ABS");
                return repoQuery.FindTransactionHistory(offset, limit, orderBy);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public virtual List<BlockchainTransaction> GetWithdrawHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            Console.WriteLine("Not override");
            return null;
        }

        public virtual List<BlockchainTransaction> GetDepositHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            Console.WriteLine("Not override");
            return null;
        }

        public virtual List<BlockchainTransaction> GetAllHistory(out int numberData, string userID, string currency,
            int offset = -1, int limit = -1,
            string[] orderBy = null, string search = null)
        {
            numberData = -1;
            Console.WriteLine("Not override");
            return null;
        }

        /// <summary>
        /// CreateDataEmail
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="email"></param>
        /// <param name="amount"></param>
        /// <param name="transactionId"></param>
        /// <param name="template"></param>
        /// <param name="networkName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task CreateDataEmail(string subject, string email, decimal amount, string transactionId,
            EmailTemplate template, string networkName)
        {
            try
            {
                var sendMailBusiness = new SendMailBusiness.SendMailBusiness(VakapayRepositoryFactory, false);

                if (email == null) return;
                var emailQueue = new EmailQueue
                {
                    ToEmail = email,
                    Template = template,
                    Subject = subject,
                    NetworkName = networkName,
                    Amount = amount,
                    TransactionId = transactionId,
                    Status = Status.STATUS_PENDING,
                };
                await sendMailBusiness.CreateEmailQueueAsync(emailQueue);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// GetEmailByAddress
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private string GetEmailByTransaction(BlockchainTransaction transaction)
        {
            try
            {
                var userRepository = VakapayRepositoryFactory.GetUserRepository(DbConnection);
                var email = userRepository.FindEmailBySendTransaction(transaction);
                return email;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}