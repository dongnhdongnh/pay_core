﻿using System;
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
        private static readonly WalletBusiness.WalletBusiness WalletBusiness = null;

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
                var bitcoinConnect = new BitcoinRPCConnect
                {
                    Host = "http://127.0.0.1:18443",
                    UserName = "bitcoinrpc",
                    Password = "wqfgewgewi"
                };
                var btcBusiness = new BitcoinBusiness.BitcoinBusiness(persistenceFactory, bitcoinConnect);
                ReturnObject transaction = btcBusiness.GetTransaction(args[0]);
                logger.Debug("BitcoinNotify =>> BTCTransactionModel: " + transaction.Data);
                BTCTransactionModel transactionModel = BTCTransactionModel.FromJson(transaction.Data);
                if (transactionModel.BtcTransactionDetailsModel != null &&
                    transactionModel.BtcTransactionDetailsModel.Length > 0)
                {
                    foreach (var transactionModelDetail in transactionModel.BtcTransactionDetailsModel)
                    {
                        if (transactionModelDetail.Category.Equals("receive"))
                        {
                            HandleNotifyDataReceiver(transactionModel, transactionModelDetail, btcBusiness);
                        }
                        else
                        {
                            // if isExist(by address and transactionId) then update, else insert
                            HandleNotifyDataSend(transactionModel, transactionModelDetail, btcBusiness);
                        }
                    }
                }
                else
                {
                    logger.Debug("BitcoinNotify BtcTransactionDetailsModel is not exist");
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "BitcoinNotify exception");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="btcDepositTransactionRepository"></param>
        /// <param name="address"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        private static BitcoinDepositTransaction GetDepositTransaction(
            IBitcoinDepositTransactioRepository btcDepositTransactionRepository, String address, String transactionId)
        {
            try
            {
                BitcoinDepositTransaction depositTransaction = btcDepositTransactionRepository.FindOneWhere(
                    new BitcoinDepositTransaction
                    {
                        Hash = transactionId,
                        ToAddress = address
                    });
                logger.Debug("GetDepositTransaction from DB: " + depositTransaction.ToJson());

                return depositTransaction;
            }

            catch (Exception e)
            {
                logger.Error(e, "GetDepositTransaction exception");
                return null;
            }
        }


        /// <summary>
        /// handle notify transaction receiver
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <param name="transactionModelDetail"></param>
        /// <param name="btcBusiness"></param>
        private static void HandleNotifyDataReceiver(BTCTransactionModel transactionModel,
            BTCTransactionDetailModel transactionModelDetail, BitcoinBusiness.BitcoinBusiness btcBusiness)
        {
            try
            {
                logger.Debug("HandleNotifiDataReceiver start");

                var btcDepositTransactionRepository = btcBusiness
                    .Factory.GetBitcoinDepositTransactioRepository(btcBusiness.Dbconnect);

                logger.Debug("HandleNotifiDataReceiver start1");
                var currentBtcDepositTransaction = GetDepositTransaction(btcDepositTransactionRepository,
                    transactionModelDetail.Address, transactionModel.Txid);
                logger.Debug("HandleNotifiDataReceiver =>> currentBtcDepositTransaction: " +
                             currentBtcDepositTransaction);
                var currentTime = CommonHelper.GetUnixTimestamp();
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
                    IBitcoinRawTransactionRepository bitcoinRawTransactionRepository = btcBusiness
                        .Factory.GeBitcoinRawTransactionRepository(btcBusiness.Dbconnect);
                    BitcoinWithdrawTransaction currentBtcWithdrawTransaction =
                        GetBtcWithdrawTransaction(bitcoinRawTransactionRepository, transactionModelDetail.Address,
                            transactionModel.Txid);
                    if (currentBtcWithdrawTransaction != null)
                    {
                        currentBtcWithdrawTransaction.BlockHash = transactionModel.Blockhash;
                        currentBtcDepositTransaction.UpdatedAt = currentTime;
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

                    // update balance 
                    WalletBusiness?.UpdateBalance(transactionModelDetail.Address, transactionModelDetail.Amount,
                        "Bitcoin");
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "HandleNotifiDataReceiver exception");
            }
        }

        /// <summary>
        /// CreateNewBtcDepositTransaction
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <param name="transactionModelDetail"></param>
        /// <param name="btcDepositTransactionRepository"></param>
        /// <param name="currentTime"></param>
        private static void CreateNewBtcDepositTransaction(BTCTransactionModel transactionModel,
            BTCTransactionDetailModel transactionModelDetail,
            IBitcoinDepositTransactioRepository btcDepositTransactionRepository, long currentTime
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

        /// <summary>
        /// GetBtcWithdrawTransaction
        /// </summary>
        /// <param name="bitcoinRawTransactionRepository"></param>
        /// <param name="address"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        private static BitcoinWithdrawTransaction GetBtcWithdrawTransaction(
            IBitcoinRawTransactionRepository bitcoinRawTransactionRepository, String address, String transactionId)
        {
            try
            {
                List<BitcoinWithdrawTransaction> listBtcRawTransactions =
                    bitcoinRawTransactionRepository.FindWhere(new BitcoinWithdrawTransaction
                    {
                        Hash = transactionId,
                        ToAddress = address
                    });
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

        /// <summary>
        /// HandleNotifyDataSend
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <param name="transactionModelDetail"></param>
        /// <param name="btcBusiness"></param>
        private static void HandleNotifyDataSend(BTCTransactionModel transactionModel,
            BTCTransactionDetailModel transactionModelDetail, BitcoinBusiness.BitcoinBusiness btcBusiness)
        {
            try
            {
                logger.Debug("HandleNotifyDataSend start");
                IBitcoinRawTransactionRepository bitcoinRawTransactionRepository = btcBusiness
                    .Factory.GeBitcoinRawTransactionRepository(btcBusiness.Dbconnect);

                BitcoinWithdrawTransaction currentBtcWithdrawTransaction =
                    GetBtcWithdrawTransaction(bitcoinRawTransactionRepository, transactionModelDetail.Address,
                        transactionModel.Txid);
                logger.Debug("HandleNotifyDataSend =>> btcWithdrawTransaction: " + currentBtcWithdrawTransaction);
                if (currentBtcWithdrawTransaction == null)
                {
                    logger.Debug("cretateNewBtcDepositTransaction ");
                    var currentTime = CommonHelper.GetUnixTimestamp();
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
                        CreatedAt =  currentTime,
                        UpdatedAt =  currentTime
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