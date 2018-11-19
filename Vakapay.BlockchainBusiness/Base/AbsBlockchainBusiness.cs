using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;
using Vakapay.PortfolioHistoryBusiness;

namespace Vakapay.BlockchainBusiness.Base
{
    public abstract class AbsBlockchainBusiness
    {
        public IVakapayRepositoryFactory VakapayRepositoryFactory { get; }
        public IDbConnection DbConnection { get; }
        PortfolioHistoryBusiness.PortfolioHistoryBusiness portfolioHistoryBusiness;

        public AbsBlockchainBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            VakapayRepositoryFactory = vakapayRepositoryFactory;
            DbConnection = isNewConnection
                ? VakapayRepositoryFactory.GetDbConnection()
                : VakapayRepositoryFactory.GetOldConnection();
            portfolioHistoryBusiness = new PortfolioHistoryBusiness.PortfolioHistoryBusiness(VakapayRepositoryFactory, false);
            //   (PortfolioHistoryBusiness.PortfolioHistoryBusiness) ;
            // portfolioHistoryBusiness = new PortfolioHistoryBusiness.PortfolioHistoryBusiness();
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
                if (sendTransaction.Status == Status.STATUS_COMPLETED)
                {
                    var email = GetEmailByTransaction(pendingTransaction);
                    if (email != null)
                    {
                        //                        await CreateDataEmail("Notify send " + pendingTransaction.NetworkName(),
                        //                            email, pendingTransaction.Amount,
                        //                            Constants.TEMPLATE_EMAIL_SENT, pendingTransaction.NetworkName(),Constants.TYPE_SEND);
                        await SendMailBusiness.SendMailBusiness.CreateDataEmail("Notify send " + pendingTransaction.NetworkName(),
                            email, pendingTransaction.Amount, pendingTransaction.Id,
                            EmailTemplate.Sent, pendingTransaction.NetworkName(), VakapayRepositoryFactory, false);
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
            catch (Exception e)
            {
                //release lock
                transactionDbSend.Rollback();
                var resultRelease = repoQuery.ReleaseLock(pendingTransaction);
                Console.WriteLine(JsonHelper.SerializeObject(resultRelease));
                throw e;
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

        public virtual async Task<ReturnObject> ScanBlockAsync<TWithDraw, TDeposit, TBlockResponse, TTransaction>(
            string networkName,
            IWalletBusiness wallet,
            IRepositoryBlockchainTransaction<TWithDraw> withdrawRepoQuery,
            IRepositoryBlockchainTransaction<TDeposit> depositRepoQuery,
            IBlockchainRpc rpcClass)
            where TWithDraw : BlockchainTransaction
            where TDeposit : BlockchainTransaction
            where TBlockResponse : EthereumBlockResponse
            where TTransaction : EthereumTransactionResponse
        {
            try
            {
                int lastBlock = -1;
                int blockNumber = -1;
                //Get lastBlock from last time
                int.TryParse(
                    CacheHelper.GetCacheString(String.Format(RedisCacheKey.KEY_SCANBLOCK_LASTSCANBLOCK,
                        networkName)), out lastBlock);
                if (lastBlock < 0)
                    lastBlock = 0;

                //get blockNumber:
                var _result = rpcClass.GetBlockNumber();
                if (_result.Status == Status.STATUS_ERROR)
                {
                    throw new Exception("Cant GetBlockNumber");
                }

                if (!int.TryParse(_result.Data.ToString(), out blockNumber))
                {
                    throw new Exception("Cant parse block number");
                }

                //Get list of new block that have transactions
                if (lastBlock >= blockNumber)
                    lastBlock = blockNumber;
                Console.WriteLine("SCAN FROM " + lastBlock + "___" + blockNumber);
                List<TBlockResponse> blocks = new List<TBlockResponse>();
                for (int i = lastBlock; i <= blockNumber; i++)
                {
                    if (i < 0) continue;
                    _result = rpcClass.GetBlockByNumber(i);
                    if (_result.Status == Status.STATUS_ERROR)
                    {
                        return _result;
                    }

                    if (_result.Data == null)
                        continue;
                    TBlockResponse _block = JsonHelper.DeserializeObject<TBlockResponse>(_result.Data.ToString());
                    if (_block.TransactionsResponse.Length > 0)
                    {
                        blocks.Add(_block);
                    }
                }

                CacheHelper.SetCacheString(String.Format(RedisCacheKey.KEY_SCANBLOCK_LASTSCANBLOCK, networkName),
                    blockNumber.ToString());
                if (blocks.Count <= 0)
                {
                    throw new Exception("no blocks have transaction");
                }
                //Get done,List<> blocks now contains all block that have transaction
                //check Transaction and update:
                //Search transactions which need to scan:

                var withdrawPendingTransactions = withdrawRepoQuery.FindTransactionsNotCompleteOnNet();
                //Scan all block and check Withdraw transaction:
                Console.WriteLine("Scan withdrawPendingTransactions");
                if (withdrawPendingTransactions.Count > 0)
                {
                    foreach (TBlockResponse _block in blocks)
                    {
                        if (withdrawPendingTransactions.Count <= 0)
                        {
                            //SCAN DONE:
                            break;
                        }

                        for (int i = withdrawPendingTransactions.Count - 1; i >= 0; i--)
                        {
                            BlockchainTransaction _currentPending = withdrawPendingTransactions[i];
                            EthereumTransactionResponse _trans =
                                _block.TransactionsResponse.SingleOrDefault(x => x.Hash.Equals(_currentPending.Hash));
                            int _blockNumber = -1;
                            int _fee = 0;

                            if (_trans != null)
                            {
                                _trans.BlockNumber.HexToInt(out _blockNumber);
                                if (_trans.Fee != null)
                                    _trans.Fee.HexToInt(out _fee);
                                Console.WriteLine("HELLO " + _currentPending.Hash);
                                _currentPending.BlockNumber = _blockNumber;
                                _currentPending.Fee = _fee;
                                _currentPending.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                                //	_currentPending.Status = Status.StatusCompleted;
                                //	_currentPending.InProcess = 0;
                                Console.WriteLine("CaLL UPDATE");

                                portfolioHistoryBusiness.InsertWithPrice(_currentPending.UserId);
                                withdrawRepoQuery.Update((TWithDraw)_currentPending);
                                withdrawPendingTransactions.RemoveAt(i);
                            }
                        }
                    }
                }

                Console.WriteLine("Scan withdrawPendingTransactions Done");
                //check wallet balance and update 
                foreach (TBlockResponse _block in blocks)
                {
                    foreach (EthereumTransactionResponse _trans in _block.TransactionsResponse)
                    {
                        string _toAddress = _trans.To;
                        string _fromAddress = _trans.From;
                        if (!wallet.CheckExistedAddress(_toAddress, networkName))
                        {
                            //logger.Info(to + " is not exist in Wallet!!!");
                            continue;
                        }
                        else
                        {
                            //Console.WriteLine("value" + _trans.value);
                            int _transaValue = 0;
                            if (_trans.Value.HexToInt(out _transaValue))
                            {
                                var userID = "";
                                //  portfolioHistoryBusiness.InsertWithPrice(_trans.i);
                                wallet.UpdateBalanceDeposit(_toAddress, (Decimal)_transaValue, networkName);
                            }
                        }
                    }
                }


                return new ReturnObject
                {
                    Status = Status.STATUS_COMPLETED,
                    Message = "Scan done"
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