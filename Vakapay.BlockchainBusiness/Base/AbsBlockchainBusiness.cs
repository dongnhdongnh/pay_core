using System;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.BlockchainBusiness.Base
{
    public abstract class AbsBlockchainBusiness
    {
        public  IVakapayRepositoryFactory VakapayRepositoryFactory { get; }
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
        public async Task<ReturnObject> SendTransactionAsync<TBlockchainTransaction>(IRepositoryBlockchainTransaction<TBlockchainTransaction> repoQuery, IBlockchainRPC rpcClass, string privateKey = "")
        {
            /*
             * 1. Query Transaction Withdraw pending
             * 2. Update Processing = 1, version = version + 1
             * 3. Commit Transaction
             * 4. Call RPC send transaction
             * 5. Update Transaction Status
             */
            // find transaction pending
            var pendingTransaction =  repoQuery.FindTransactionPending();
            if(pendingTransaction == null || pendingTransaction.Id == null)
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Message = "Not found Transaction"
                };
            if (DbConnection.State != ConnectionState.Open)
            {
                Console.WriteLine(DbConnection.State);
                DbConnection.Open();
            }
            
               
            //begin first transaction
            var transactionFirst = DbConnection.BeginTransaction();
            try
            {
                //lock transaction for process
                var resultLock = await repoQuery.LockForProcess(pendingTransaction);
                if (resultLock.Status == Status.StatusError)
                {
                    transactionFirst.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Message = "Cannot Lock For Process"
                    };
                }
                //commit transaction
                transactionFirst.Commit();
            }
            catch (Exception e)
            {
                transactionFirst.Rollback();
                return new ReturnObject
                {
                    Status = Status.StatusError,
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
                pendingTransaction.InProcess = 0;
                pendingTransaction.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                pendingTransaction.Hash = sendTransaction.Data;
                var result = await repoQuery.SafeUpdate(pendingTransaction);
                if (result.Status == Status.StatusError)
                {
                    transactionDbSend.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Cannot Save Transaction Status"
                    };
                }
                transactionDbSend.Commit();
                return new ReturnObject
                {
                    Status = sendTransaction.Status,
                    Message = sendTransaction.Message
                };
                
            }
            catch (Exception e)
            {
                //release lock
                var resultRelease = repoQuery.ReleaseLock(pendingTransaction);
                Console.WriteLine(JsonHelper.SerializeObject(resultRelease));
                transactionDbSend.Rollback();
                throw e;
            }
            
            
        }

        

        public async Task<ReturnObject> CreateAddressAsyn()
        {
            return null;
        }

        public async Task<ReturnObject> ScanBlockAsyn()
        {
            return null;
        }

        public async Task<ReturnObject> ScanBlockByNumberAsyn(int blockNumber)
        {
            return null;
        }
    }
}