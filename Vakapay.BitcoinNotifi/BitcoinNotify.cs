using System;
using System.Linq;
using NLog;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.BitcoinNotifi
{
    internal class BitcoinNotify
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static WalletBusiness.WalletBusiness _walletBusiness = null;

        public static void Main(string[] args)
        {
            try
            {
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = AppSettingHelper.GetDbConnection()
                };

                var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

                var btcBusiness = new BitcoinBusiness.BitcoinBusiness(persistenceFactory);
                var rpc = new BitcoinRpc(AppSettingHelper.GetBitcoinNode(),
                    AppSettingHelper.GetBitcoinRpcAuthentication());

                var transaction = rpc.FindTransactionByHash(args[0]);
                _logger.Debug("BitcoinNotify =>> BTCTransactionModel: " + transaction.Data);
                var transactionModel = BtcTransactionModel.FromJson(transaction.Data);
                if (transactionModel.BtcTransactionDetailsModel != null &&
                    transactionModel.BtcTransactionDetailsModel.Length > 0)
                {
                    foreach (var transactionModelDetail in transactionModel.BtcTransactionDetailsModel)
                    {
                        if (transactionModelDetail.Category.Equals("receive"))
                        {
                            HandleNotifyDataReceiver(transactionModel, transactionModelDetail, btcBusiness);
                        }
                        else if (transactionModelDetail.Category.Equals("send"))
                        {
                            // if isExist(by address and transactionId) then update, else insert
                            HandleNotifyDataSend(transactionModel, transactionModelDetail, btcBusiness);
                        }
                    }
                }
                else
                {
                    _logger.Debug("BitcoinNotify BtcTransactionDetailsModel is not exist");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "BitcoinNotify exception");
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
            IBitcoinDepositTransactionRepository btcDepositTransactionRepository, string address, string transactionId)
        {
            try
            {
                var depositTransaction = btcDepositTransactionRepository.FindOneWhere(
                    new BitcoinDepositTransaction
                    {
                        Hash = transactionId,
                        ToAddress = address
                    });
                _logger.Debug("GetDepositTransaction from DB: " + depositTransaction.ToJson());

                return depositTransaction;
            }

            catch (Exception e)
            {
                _logger.Error(e, "GetDepositTransaction exception");
                return null;
            }
        }


        /// <summary>
        /// handle notify transaction receiver
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <param name="transactionModelDetail"></param>
        /// <param name="btcBusiness"></param>
        private static void HandleNotifyDataReceiver(BtcTransactionModel transactionModel,
            BtcTransactionDetailModel transactionModelDetail, BitcoinBusiness.BitcoinBusiness btcBusiness)
        {
            try
            {
                _logger.Debug("HandleNotifiDataReceiver start");

                var btcDepositTransactionRepository = btcBusiness
                    .VakapayRepositoryFactory.GetBitcoinDepositTransactionRepository(btcBusiness.DbConnection);

                _logger.Debug("HandleNotifiDataReceiver start1");
                var currentBtcDepositTransaction = GetDepositTransaction(btcDepositTransactionRepository,
                    transactionModelDetail.Address, transactionModel.Txid);
                _logger.Debug("HandleNotifiDataReceiver =>> currentBtcDepositTransaction: " +
                              currentBtcDepositTransaction);
                var currentTime = CommonHelper.GetUnixTimestamp();
                if (transactionModel.Confirmations == 0)
                {
                    _logger.Debug("HandleNotifiDataReceiver with confirm = 0");
                    if (currentBtcDepositTransaction == null)
                    {
                        CreateNewBtcDepositTransaction(transactionModel, transactionModelDetail,
                            btcDepositTransactionRepository);
                    }
                    else
                    {
                        // transaction is exist
                        _logger.Debug(
                            "HandleNotifiDataReceiver =>> BitcoinDepositTransaction is exist, don't create new data");
                    }
                }
                else
                {
                    _logger.Debug("HandleNotifiDataReceiver with confirm > 0");
                    if (currentBtcDepositTransaction != null)
                    {
                        currentBtcDepositTransaction.BlockHash = transactionModel.BlockHash;
                        currentBtcDepositTransaction.Amount = transactionModel.Amount;
                        currentBtcDepositTransaction.Status = Status.STATUS_COMPLETED;
                        currentBtcDepositTransaction.UpdatedAt = currentTime;
                        btcDepositTransactionRepository.Update(currentBtcDepositTransaction);
                    }
                    else
                    {
                        CreateNewBtcDepositTransaction(transactionModel, transactionModelDetail,
                            btcDepositTransactionRepository);
                    }

                    // update balance 
                    _walletBusiness?.UpdateBalanceDeposit(transactionModelDetail.Address, transactionModelDetail.Amount,
                        "Bitcoin");

                    //insert new email data
                    CreateDataEmail(btcBusiness, transactionModelDetail);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "HandleNotifiDataReceiver exception");
            }
        }

        /// <summary>
        /// CreateDataEmail
        /// </summary>
        /// <param name="btcBusiness"></param>
        /// <param name="transactionModelDetail"></param>
        private static void CreateDataEmail(BitcoinBusiness.BitcoinBusiness btcBusiness,
            BtcTransactionDetailModel transactionModelDetail)
        {
            try
            {
                var userRepository = btcBusiness.VakapayRepositoryFactory.GetUserRepository(btcBusiness.DbConnection);
                var email = userRepository.FindEmailByBitcoinAddress(transactionModelDetail.Address);

                if (email != null)
                {
                    const string title = "Notify receiver Bitcoin";
//                    btcBusiness.CreateDataEmail(title, email, transactionModelDetail.Amount,
//                        Constants.TEMPLATE_EMAIL_SENT, Constants.NETWORK_NAME_BITCOIN, Constants.TYPE_RECEIVER);
                    var res = SendMailBusiness.SendMailBusiness.CreateDataEmail(title, email, transactionModelDetail.Amount, "", //TODO add transaction Id
                        EmailTemplate.Received, CryptoCurrency.BTC, btcBusiness.VakapayRepositoryFactory, false);
                }
                else
                {
                    _logger.Debug("CreateDataEmail =>> error because email fail");
                }
            }
            catch (Exception e)
            {
                _logger.Error("CreateDataEmail error: ", e.Message);
            }
        }

        /// <summary>
        /// CreateNewBtcDepositTransaction
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <param name="transactionModelDetail"></param>
        /// <param name="btcDepositTransactionRepository"></param>
        private static void CreateNewBtcDepositTransaction(BtcTransactionModel transactionModel,
            BtcTransactionDetailModel transactionModelDetail,
            IBitcoinDepositTransactionRepository btcDepositTransactionRepository
        )
        {
            try
            {
                _logger.Debug("cretateNewBtcDepositTransaction ");
                var btcDepositTransaction = new BitcoinDepositTransaction
                {
                    Hash = transactionModel.Txid,
                    BlockNumber = 0,
                    BlockHash = transactionModel.BlockHash,
//                    NetworkName = "Bitcoin",
                    Amount = transactionModel.Amount,
                    FromAddress = "",
                    ToAddress = transactionModelDetail.Address,
                    Fee = 0,
                    Status = Status.STATUS_PENDING,
                };
                _logger.Debug("cretateNewBtcDepositTransaction =>> btcDepositTransaction: " +
                              btcDepositTransaction.ToJson());
                btcDepositTransactionRepository.Insert(btcDepositTransaction);
            }
            catch (Exception e)
            {
                _logger.Error(e, "cretateNewBtcDepositTransaction ");
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
            IBitcoinWithdrawTransactionRepository bitcoinRawTransactionRepository, string address, string transactionId)
        {
            try
            {
                var listBtcRawTransactions = bitcoinRawTransactionRepository.FindWhere(new BitcoinWithdrawTransaction
                {
                    Hash = transactionId,
                    ToAddress = address
                });
                _logger.Debug("GetBtcWithdrawTransaction from DB: " + listBtcRawTransactions.Count);
                return listBtcRawTransactions.Count > 0 ? listBtcRawTransactions.ElementAt(0) : null;
            }

            catch (Exception e)
            {
                _logger.Error(e, "GetBtcWithdrawTransaction exception");
                return null;
            }
        }

        /// <summary>
        /// HandleNotifyDataSend
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <param name="transactionModelDetail"></param>
        /// <param name="btcBusiness"></param>
        private static void HandleNotifyDataSend(BtcTransactionModel transactionModel,
            BtcTransactionDetailModel transactionModelDetail, BitcoinBusiness.BitcoinBusiness btcBusiness)
        {
            try
            {
                _logger.Debug("HandleNotifyDataSend start");
                if (transactionModel.Confirmations > 0)
                {
                    var bitcoinRawTransactionRepository = btcBusiness
                        .VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(btcBusiness.DbConnection);

                    var currentBtcWithdrawTransaction =
                        GetBtcWithdrawTransaction(bitcoinRawTransactionRepository, transactionModelDetail.Address,
                            transactionModel.Txid);

                    _logger.Debug("HandleNotifyDataSend =>> btcWithdrawTransaction: " + currentBtcWithdrawTransaction);
                    if (currentBtcWithdrawTransaction != null)
                    {
                        _logger.Debug("HandleNotifyDataSend ==>> Update hash and time update ");
                        var currentTime = CommonHelper.GetUnixTimestamp();
                        currentBtcWithdrawTransaction.BlockHash = transactionModel.BlockHash;
                        currentBtcWithdrawTransaction.UpdatedAt = currentTime;
                        bitcoinRawTransactionRepository.Update(currentBtcWithdrawTransaction);
                    }
                }
                else
                {
                    _logger.Debug("HandleNotifyDataSend =>> confirm == 0");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "HandleNotifyDataSend exception");
            }
        }
    }
}