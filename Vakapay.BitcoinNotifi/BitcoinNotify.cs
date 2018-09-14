using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.BitcoinNotifi
{
    class BitcoinNotify
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString =
                        "server=127.0.0.1;userid=root;password=Chelsea1992;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
                };

                var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

                var btcBussines = new BitcoinBusiness.BitcoinBusiness(persistenceFactory);
                ReturnObject transaction = btcBussines.GetTransaction(args[0]);
                logger.Debug("BitcoinNotify =>> BTCTransactionModel: " + transaction.Data);
                BTCTransactionModel transactionModel = BTCTransactionModel.FromJson(transaction.Data);
                if (transactionModel.BtcTransactionDetailsModel != null &&
                    transactionModel.BtcTransactionDetailsModel.Length > 0)
                {
                    foreach (var transactionModelDetail in transactionModel.BtcTransactionDetailsModel)
                    {
                        if (transactionModelDetail.Category.Equals("receive"))
                        {
                            HandleNotifyDataReceiver(transactionModel, transactionModelDetail, btcBussines);
                        }
                        else
                        {
                            // if isExist(by address and transactionId) then update, else insert
                            HandleNotifyDataSend(transactionModel, transactionModelDetail, btcBussines);
                        }
                    }
                }
                else
                {
                    //show log transaction is exist
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "BitcoinNotify exception");
            }
        }

        private static BitcoinDepositTransaction GetDepositTransaction(
            IBitcoinDepositTransactioRepository btcDepositTransactionRepository, String address, String transactionId)
        {
            try
            {
                List<BitcoinDepositTransaction> listBtcDepositTransactions = btcDepositTransactionRepository.FindBySql(
                    "SELECT * FROM bitcoindeposittransaction WHERE Hash = '" +
                    transactionId + "' and ToAddress = '" + address + "'");
                logger.Debug("GetDepositTransaction from DB: " + listBtcDepositTransactions.Count);
                if (listBtcDepositTransactions.Count > 0)
                {
                    return listBtcDepositTransactions.ElementAt(0);
                }

                return null;
            }

            catch (Exception e)
            {
                logger.Error(e, "GetDepositTransaction exception");
                return null;
            }
        }


        // handle notify transaction receiver
        private static void HandleNotifyDataReceiver(BTCTransactionModel transactionModel,
            BTCTransactionDetailModel transactionModelDetail, BitcoinBusiness.BitcoinBusiness btcBussines)
        {
            try
            {
                logger.Debug("HandleNotifiDataReceiver start");

                var btcDepositTransactionRepository = btcBussines
                    .factory.GetBitcoinDepositTransactioRepository(btcBussines.dbconnect);

                logger.Debug("HandleNotifiDataReceiver start1");
                var currentBtcDepositTransaction = GetDepositTransaction(btcDepositTransactionRepository,
                    transactionModelDetail.Address, transactionModel.Txid);
                logger.Debug("HandleNotifiDataReceiver =>> currentBtcDepositTransaction: " +
                             currentBtcDepositTransaction);
                var currentTime = CommonHelper.GetUnixTimestamp().ToString();
                if (transactionModel.Confirmations == 0)
                {
                    logger.Debug("HandleNotifiDataReceiver with confirm = 0");
                    if (currentBtcDepositTransaction == null)
                    {
                        CreateNewBtcDepositTransaction(transactionModel, transactionModelDetail,
                            btcDepositTransactionRepository, currentTime);
                    }
                    else
                    {
                        // transaction is exist
                        logger.Debug(
                            "HandleNotifiDataReceiver =>> BitcoinDepositTransaction is exist, don't create new data");
                    }
                }
                else
                {
                    logger.Debug("HandleNotifiDataReceiver with confirm > 0");
                    IBitcoinRawTransactionRepository bitcoinRawTransactionRepository = btcBussines
                        .factory.GeBitcoinRawTransactionRepository(btcBussines.dbconnect);
                    BitcoinWithdrawTransaction currentBtcWithdrawTransaction =
                        GetBtcWithdrawTransaction(bitcoinRawTransactionRepository, transactionModelDetail.Address,
                            transactionModel.Txid);
                    if (currentBtcWithdrawTransaction != null)
                    {
                        bitcoinRawTransactionRepository.Update(currentBtcWithdrawTransaction);
                    }

                    if (currentBtcDepositTransaction != null)
                    {
                        currentBtcDepositTransaction.BlockHash = transactionModel.Blockhash;
                        currentBtcDepositTransaction.Amount = transactionModel.Amount;
                        currentBtcDepositTransaction.Status = Status.StatusCompleted;
                        currentBtcDepositTransaction.UpdatedAt = currentTime;
                        btcDepositTransactionRepository.Update(currentBtcDepositTransaction);
                    }
                    else
                    {
                        CreateNewBtcDepositTransaction(transactionModel, transactionModelDetail,
                            btcDepositTransactionRepository, currentTime);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "HandleNotifiDataReceiver exception");
            }
        }

        private static void CreateNewBtcDepositTransaction(BTCTransactionModel transactionModel,
            BTCTransactionDetailModel transactionModelDetail,
            IBitcoinDepositTransactioRepository btcDepositTransactionRepository, string currentTime
        )
        {
            try
            {
                logger.Debug("cretateNewBtcDepositTransaction ");
                var btcDepositTransaction = new BitcoinDepositTransaction
                {
                    Id = CommonHelper.GenerateUuid(),
                    Hash = transactionModel.Txid,
                    BlockNumber = "",
                    BlockHash = transactionModel.Blockhash,
                    NetworkName = "Bitcoin",
                    Amount = transactionModel.Amount,
                    FromAddress = "",
                    ToAddress = transactionModelDetail.Address,
                    Fee = 0,
                    Status = Status.StatusPending,
                    CreatedAt = currentTime,
                    UpdatedAt = currentTime
                };
                logger.Debug("cretateNewBtcDepositTransaction =>> btcDepositTransaction: " +
                             btcDepositTransaction.ToJson());
                btcDepositTransactionRepository.Insert(btcDepositTransaction);
            }
            catch (Exception e)
            {
                logger.Error(e, "cretateNewBtcDepositTransaction ");
            }
        }

        private static BitcoinWithdrawTransaction GetBtcWithdrawTransaction(
            IBitcoinRawTransactionRepository bitcoinRawTransactionRepository, String address, String transactionId)
        {
            try
            {
                List<BitcoinWithdrawTransaction> listBtcRawTransactions =
                    bitcoinRawTransactionRepository.FindBySql(
                        "SELECT * FROM bitcoinwithdrawtransaction WHERE Hash = '" +
                        transactionId + "' and ToAddress = '" + address + "'");
                logger.Debug("GetBtcWithdrawTransaction from DB: " + listBtcRawTransactions.Count);
                if (listBtcRawTransactions.Count > 0)
                {
                    return listBtcRawTransactions.ElementAt(0);
                }

                return null;
            }

            catch (Exception e)
            {
                logger.Error(e, "GetBtcWithdrawTransaction exception");
                return null;
            }
        }

        private static void HandleNotifyDataSend(BTCTransactionModel transactionModel,
            BTCTransactionDetailModel transactionModelDetail, BitcoinBusiness.BitcoinBusiness btcBussines)
        {
            try
            {
                logger.Debug("HandleNotifyDataSend start");
                IBitcoinRawTransactionRepository bitcoinRawTransactionRepository = btcBussines
                    .factory.GeBitcoinRawTransactionRepository(btcBussines.dbconnect);

                BitcoinWithdrawTransaction currentBtcWithdrawTransaction =
                    GetBtcWithdrawTransaction(bitcoinRawTransactionRepository, "mwfHq6NigeDDV7MrBHdyZhQdUWeYqPCFLV",
                        "beb4bd560b68cc5c28e772322d8f2caee67bf76364c62dc62a724f46154e9b6b");
                logger.Debug("HandleNotifyDataSend =>> btcWithdrawTransaction: " + currentBtcWithdrawTransaction);
                if (currentBtcWithdrawTransaction == null)
                {
                    logger.Debug("cretateNewBtcDepositTransaction ");
                    var currentTime = CommonHelper.GetUnixTimestamp().ToString();
                    var newBtcWithdrawTransaction = new BitcoinWithdrawTransaction()
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Hash = transactionModel.Txid,
                        BlockNumber = "",
                        NetworkName = "Bitcoin",
                        Amount = transactionModel.Amount,
                        FromAddress = transactionModelDetail.Account,
                        ToAddress = transactionModelDetail.Address,
                        Fee = 0,
                        Status = Status.StatusCompleted,
                        CreatedAt = currentTime,
                        UpdatedAt = currentTime
                    };
                    logger.Debug("cretateNewBtcDepositTransaction =>> btcDepositTransaction: " +
                                 newBtcWithdrawTransaction);
                    bitcoinRawTransactionRepository.Insert(newBtcWithdrawTransaction);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "HandleNotifyDataSend exception");
            }
        }
    }
}