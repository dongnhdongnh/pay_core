using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Vakapay.BlockchainBusiness;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.EthereumBusiness
{
    public class EthereumBusiness : AbsBlockchainBusiness
    {
        public EthereumBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) : base(
            vakapayRepositoryFactory, isNewConnection)
        {
        }

        public ReturnObject FakePendingTransaction(EthereumWithdrawTransaction blockchainTransaction)
        {
            try
            {
                using (var ethereumwithdrawRepo =
                        VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection))
                {
                    blockchainTransaction.Status = Status.STATUS_PENDING;
                    return ethereumwithdrawRepo.Insert(blockchainTransaction);
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

        public ReturnObject FakeDepositTransaction(EthereumDepositTransaction blockchainTransaction)
        {
            try
            {
                using (var repo = VakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(DbConnection))
                {
                    blockchainTransaction.Status = Status.STATUS_COMPLETED;
                    return repo.Insert(blockchainTransaction);
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
            using (var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection))
            {
                Console.WriteLine("Get ETH HISTORY");
                return GetHistory<EthereumWithdrawTransaction>(ethereumwithdrawRepo, offset, limit, orderBy);
            }
        }

        public override List<BlockchainTransaction> GetDepositHistory(int offset = -1, int limit = -1,
            string[] orderBy = null)
        {
            using (var ethereumDepositRepo = VakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(DbConnection))
            {
                return GetHistory<EthereumDepositTransaction>(ethereumDepositRepo, offset, limit, orderBy);
            }
        }


        public override List<BlockchainTransaction> GetAllHistory(out int numberData, string userID, string currency,
            int offset = -1,
            int limit = -1, string[] orderBy = null, string search = null,long day=-1)
        {
            using (var depositRepo = VakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(DbConnection))
            {
                using (var withdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection))
                {
                    using (var inter = VakapayRepositoryFactory.GetInternalTransactionRepository(DbConnection))
                    {
                        return GetAllHistory<EthereumWithdrawTransaction, EthereumDepositTransaction>(out numberData, userID,
                            currency, withdrawRepo, depositRepo, inter.GetTableName(), offset, limit, orderBy, search);

                    }
                }
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
                var result = rpcClass.GetBlockNumber();
                if (result.Status == Status.STATUS_ERROR)
                {
                    throw new Exception("Cant GetBlockNumber");
                }

                if (!int.TryParse(result.Data, out blockNumber))
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
                    result = rpcClass.GetBlockByNumber(i);
                    if (result.Status == Status.STATUS_ERROR)
                    {
                        return result;
                    }

                    if (result.Data == null)
                        continue;
                    TBlockResponse block = JsonHelper.DeserializeObject<TBlockResponse>(result.Data);
                    if (block.TransactionsResponse.Length > 0)
                    {
                        blocks.Add(block);
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
                    foreach (TBlockResponse block in blocks)
                    {
                        if (withdrawPendingTransactions.Count <= 0)
                        {
                            //SCAN DONE:
                            break;
                        }

                        for (int i = withdrawPendingTransactions.Count - 1; i >= 0; i--)
                        {
                            BlockchainTransaction currentPending = withdrawPendingTransactions[i];
                            EthereumTransactionResponse trans =
                                block.TransactionsResponse.SingleOrDefault(x => x.Hash.Equals(currentPending.Hash));
                            int _blockNumber = -1;
                            int fee = 0;

                            if (trans != null)
                            {
                                trans.BlockNumber.HexToInt(out _blockNumber);
                                if (trans.Fee != null)
                                    trans.Fee.HexToInt(out fee);
                                Console.WriteLine("HELLO " + currentPending.Hash);
                                currentPending.BlockNumber = _blockNumber;
                                currentPending.Fee = fee;
                                currentPending.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                                //	_currentPending.Status = Status.StatusCompleted;
                                //	_currentPending.InProcess = 0;
                                Console.WriteLine("CaLL UPDATE");

                                withdrawRepoQuery.Update((TWithDraw)currentPending);
                                withdrawPendingTransactions.RemoveAt(i);
                            }
                        }
                    }
                }

                Console.WriteLine("Scan withdrawPendingTransactions Done");
                //check wallet balance and update 
                foreach (TBlockResponse block in blocks)
                {
                    foreach (EthereumTransactionResponse trans in block.TransactionsResponse)
                    {
                        string toAddress = trans.To;
                        var _sendWallet = wallet.FindWalletByAddressAndNetworkName(toAddress, networkName);
                        if (_sendWallet == null)
                        //if(false)
                        {
                            //logger.Info(to + " is not exist in Wallet!!!");
                            continue;
                        }
                        else
                        {
                            //Console.WriteLine("value" + _trans.value);
                            if (trans.Value.HexToBigInteger(out var transactionValue))
                            {
                                // var userID = "";

                                // if (_sendWallet != null)
                                wallet.UpdateBalanceDeposit(toAddress, EthereumRpc.WeiToEther(transactionValue),
                                  networkName);
                                var _deposite = new EthereumDepositTransaction();
                                _deposite.Id = CommonHelper.GenerateUuid();
                                _deposite.FromAddress = trans.From;
                                _deposite.ToAddress = trans.To;
                                _deposite.UserId = _sendWallet.UserId;
                                _deposite.Hash = trans.Hash;
                                _deposite.BlockNumber = 0;
                                int bNum = 0;
                                if (trans.BlockNumber.HexToInt(out bNum))
                                    _deposite.BlockNumber = bNum;
                                _deposite.CreatedAt = (int)CommonHelper.GetUnixTimestamp();
                                _deposite.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();

                                depositRepoQuery.Insert(_deposite as TDeposit);

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
    }
}