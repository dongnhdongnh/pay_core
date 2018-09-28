using System;
using System.Collections.Generic;
using System.Data;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.BitcoinBusiness
{
    using BlockchainBusiness;

    public class BitcoinBusinessOld : BlockchainBusiness
    {
        private BitcoinRpc BitcoinRpc { get; }
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public IVakapayRepositoryFactory Factory { get; }
        public IDbConnection Dbconnect { get; }


        public BitcoinBusinessOld(IVakapayRepositoryFactory vakapayRepositoryFactory, BitcoinRPCConnect rpConnect,
            bool isNewConnection = true) :
            base(vakapayRepositoryFactory, isNewConnection)
        {
            Factory = vakapayRepositoryFactory;
            Dbconnect = DbConnection;
            BitcoinRpc = new BitcoinRpc(rpConnect.Host, rpConnect.UserName, rpConnect.Password);
            //bitcoinRpc.Credentials = new NetworkCredential("bitcoinrpc", "wqfgewgewi");
        }


        // <summary>
        // Returns a new bitcoin address for receiving payments.
        // If [account] is specified (recommended), it is added to the address book so payments 
        // received with the address will be credited to [account].
        // </summary>
        // <param name="a_account"></param>
        // bitcoin ver 16 GetNewAddress(Account); ver 17 GetNewAddress(Label)
        public ReturnObject CreateNewAddAddress(string walletId, string account = "")
        {
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

                var results = BitcoinRpc.GetNewAddress(account);
                if (results.Status == Status.StatusError)
                    return results;

                var address = results.Data;

                //add database vakaxa
                var bitcoinAddressRepo = VakapayRepositoryFactory.GetBitcoinAddressRepository(DbConnection);
                var time = (int) CommonHelper.GetUnixTimestamp();
                var bcAddress = new BitcoinAddress
                {
                    Id = CommonHelper.GenerateUuid(),
                    Address = address,
                    Status = Status.StatusActive,
                    WalletId = walletId,
                    CreatedAt = time,
                    UpdatedAt = time
                };

                var resultAddBitcoinAddress = bitcoinAddressRepo.Insert(bcAddress);
                //
                return resultAddBitcoinAddress;
            }
            catch (Exception e)
            {
                Logger.Error(e, "CreateNewAddAddress exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }


        public ReturnObject RunSendTransaction()
        {
            try
            {
                var bitcoinRawTransactionRepo =
                    VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);

                var search =
                    new Dictionary<string, string>
                    {
                        {"Status", Status.StatusPending},
                        {"InProcess", 0.ToString()},
                        {"Version", 0.ToString()}
                    };

                var pendings =
                    bitcoinRawTransactionRepo.FindBySql(bitcoinRawTransactionRepo.QuerySearch(search));
                var data = new JArray();
                if (pendings.Count <= 0) throw new Exception("NO PENING");

                foreach (var pending in pendings)
                {
                    // send 
                    Logger.Debug("SendTransaction before" + JsonHelper.SerializeObject(pending));
                    var result = SendTransaction(pending);
                    Logger.Debug("SendTransaction result" + JsonHelper.SerializeObject(result));
                    data.Add(result.Data);
                }

                return new ReturnObject
                {
                    Status = Status.StatusCompleted,
                    Data = JsonConvert.SerializeObject(data)
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "runSendTransaction exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        private ReturnObject SendTransaction(BitcoinWithdrawTransaction blockchainTransaction)
        {
            try
            {
                //check withraw database
                var bitcoinRawTransactionRepo =
                    VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);

                var raw = bitcoinRawTransactionRepo.FindById(blockchainTransaction.Id);
                if (raw == null)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "WithRawtransaction Not Found",
                        Data = blockchainTransaction.Id
                    };

                var currentVersionWithRaw = blockchainTransaction.Version.ToString();

                // set wwhere
                var blockchainTransactionWhere =
                    new Dictionary<string, string>
                    {
                        {"Id", blockchainTransaction.Id},
                        {"Version", currentVersionWithRaw},
                        {"InProcess", 0.ToString()}
                    };


                blockchainTransaction.InProcess = 1;
                blockchainTransaction.Version = blockchainTransaction.Version + 1;

                //get sql update withDraw
                var queryUpdate =
                    bitcoinRawTransactionRepo.QueryUpdate(blockchainTransaction, blockchainTransactionWhere);

                // tranh send 2 lan
                var resulUpdate = bitcoinRawTransactionRepo.ExcuteSQL(queryUpdate);

                if (resulUpdate.Status == Status.StatusError)
                    return resulUpdate;

                //send 
                var results = BitcoinRpc.SendToAddress(blockchainTransaction.ToAddress,
                    blockchainTransaction.Amount);

                if (results.Status == Status.StatusError)
                {
                    // RollbackWithSraw(blockchainTransaction);
                    throw new Exception("Can't send transaction : " +
                                        JsonHelper.SerializeObject(blockchainTransaction));
                }

                var idTransaction = results.Data;

                //get transaction
                var transaction = BitcoinRpc.GetTransaction(idTransaction);
                if (transaction.Status == Status.StatusError)
                    throw new Exception("Can't gettransaction : " + JsonHelper.SerializeObject(blockchainTransaction));
                var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data);

                //update database vakaxa
                blockchainTransaction.Status = Status.StatusCompleted;
                blockchainTransaction.UpdatedAt = CommonHelper.GetUnixTimestamp();
                blockchainTransaction.Hash = idTransaction;
                blockchainTransaction.BlockHash = (string) transactionInfo["blockhash"];


                //update where 
                var resultAddBitcoinRawTransactionAddress = bitcoinRawTransactionRepo.Update(blockchainTransaction);
                if (resultAddBitcoinRawTransactionAddress.Status == Status.StatusError)
                {
                    // RollbackWithSraw(blockchainTransaction);
                    throw new Exception("Can't update status bitcoin withdraw : " +
                                        JsonHelper.SerializeObject(blockchainTransaction));
                }

                //
                return resultAddBitcoinRawTransactionAddress;

                //deposit
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendTransaction exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /**
         * rollback version when send error
         */
        private void RollbackWithSraw(BitcoinWithdrawTransaction blockchainTransaction)
        {
            try
            {
                var bitcoinRawTransactionRepo =
                    VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);

                blockchainTransaction.Version = 0;
                blockchainTransaction.InProcess = 0;

                bitcoinRawTransactionRepo.Update(blockchainTransaction);
            }
            catch (Exception e)
            {
                Logger.Error(e, "RollbackWithSraw exception");
                throw;
            }
        }

        /// <summary>
        /// amounts are double-precision floating point numbers. Returns the transaction ID if successful.
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="mutiAddress"></param>
        /// <param name="minconf"></param>
        /// <param name="comment"></param>
        public ReturnObject Sendmany(string fromAccount, JObject mutiAddress, int minconf = 1, string comment = "")
        {
            try
            {
                var results = BitcoinRpc.Sendmany(fromAccount, mutiAddress, minconf, comment);

                if (results.Status == Status.StatusError)
                    return results;

                var idTransaction = results.Data;

                //get transaction
                var transaction = BitcoinRpc.GetTransaction(idTransaction);
                if (transaction.Status == Status.StatusError)
                    return transaction;
                var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data);


                // get block
                var blockInfo = new JObject();
                if (!string.IsNullOrEmpty((string) transactionInfo["blockhash"]))
                {
                    var block = BitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
                    if (block.Status == Status.StatusError)
                        return block;
                    blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data);
                }


                var bitcoinRawTransactionRepo =
                    VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);
                var details = transactionInfo["details"];

                var data = new JArray();

                foreach (var detail in details)
                {
                    var time = CommonHelper.GetUnixTimestamp();
                    var rawTransaction = new BitcoinWithdrawTransaction
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Hash = idTransaction,
                        BlockNumber = (int) blockInfo["height"],
                        BlockHash = (string) transactionInfo["blockhash"],
//                        NetworkName = "Bitcoin",
                        Amount = (decimal) detail["amount"] * -1,
                        FromAddress = "",
                        ToAddress = (string) detail["address"],
                        Fee = (decimal) detail["fee"] * -1,
                        Status = Status.StatusCompleted,
                        CreatedAt = time,
                        UpdatedAt = time
                    };

                    var resultAddBitcoinRawTransactionAddress = bitcoinRawTransactionRepo.Insert(rawTransaction);

                    data.Add(resultAddBitcoinRawTransactionAddress.Data);
                }

//                // update balance wallet
//                decimal balanceChange = (decimal) transactionInfo["amount"] + (decimal) transactionInfo["fee"];
//                
//                UpdateBalanceWallet(balanceChange, "97f3f010-658c-46eb-92a4-52b6a3f51e15", 159900);

                return new ReturnObject
                {
                    Status = Status.StatusCompleted,
                    Data = JsonConvert.SerializeObject(data)
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendManyTransaction exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// Amount is a real and is rounded to the nearest 0.01. Returns the transaction ID if successful.
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="minconf"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        public ReturnObject SendFrom(string fromAccount,
            string toAddress,
            double amount,
            int minconf = 1,
            string comment = "",
            string commentTo = "")
        {
            try
            {
                var results = BitcoinRpc.SendFrom(fromAccount, toAddress, amount, minconf, comment, commentTo);

                if (results.Status == Status.StatusError)
                    return results;

                var idTransaction = results.Data;

                //get transaction
                var transaction = BitcoinRpc.GetTransaction(idTransaction);
                if (transaction.Status == Status.StatusError)
                    return transaction;
                var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data);


                //block
                var blockInfo = new JObject();
                if (!string.IsNullOrEmpty((string) transactionInfo["blockhash"]))
                {
                    var block = BitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
                    if (block.Status == Status.StatusError)
                        return block;
                    blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data);
                }


                //add database vakaxa
                var bitcoinRawTransactionRepo =
                    VakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(DbConnection);

                var time = CommonHelper.GetUnixTimestamp();
                var rawTransaction = new BitcoinWithdrawTransaction
                {
                    Id = CommonHelper.GenerateUuid(),
                    Hash = idTransaction,
                    BlockNumber = (int) blockInfo["height"],
                    BlockHash = (string) transactionInfo["blockhash"],
//                    NetworkName = "Bitcoin",
                    Amount = (decimal) amount,
                    FromAddress = "",
                    ToAddress = toAddress,
                    Fee = (decimal) transactionInfo["fee"] * -1,
                    Status = Status.StatusCompleted,
                    CreatedAt = time,
                    UpdatedAt = time
                };

                var resultAddBitcoinRawTransactionAddress = bitcoinRawTransactionRepo.Insert(rawTransaction);
                //

//                //update balance wallet
//                var balanceChange = (decimal) transactionInfo["amount"] + (decimal) transactionInfo["fee"];
//                // Id va version dang tam fix cung de test
//                UpdateBalanceWallet(balanceChange, "97f3f010-658c-46eb-92a4-52b6a3f51e15", 159900);
                return resultAddBitcoinRawTransactionAddress;
                //deposit
            }
            catch (Exception e)
            {
                Logger.Error(e, "SendFromTransaction exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// gettransaction by txid
        /// </summary>
        /// <param name="txid"></param>
        public ReturnObject TransactionByTxidBlockchain(string txid)
        {
            try
            {
                var results = BitcoinRpc.GetTransaction(txid);
                if (results.Status == Status.StatusError)
                    return results;

                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = results.Data
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "TransactionByTxidBlockchain exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// gettransaction by Account
        /// </summary>
        /// <param name="account"></param>
        public ReturnObject ListtransactionByAccount(string account)
        {
            try
            {
                var results = BitcoinRpc.ListTransactions(account);
                if (results.Status == Status.StatusError)
                    return results;

                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = results.Data
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "ListtransactionByAccount exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /**
         * update balance Wallet
         */
        public ReturnObject UpdateBalanceWallet(decimal balance, string walletId, int version)
        {
            try
            {
                var walletRepository = VakapayRepositoryFactory.GetWalletRepository(DbConnection);

                //chua biet walletid, version lay tu dau
                var walletCheck = walletRepository.FindById(walletId);


                if (walletCheck == null)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Wallet not found"
                    };

                return walletRepository.UpdateBalanceWallet(balance, walletId, version);
            }
            catch (Exception e)
            {
                Logger.Error(e, "UpdateBalanceWallet exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// test 
        /// </summary>
        /// <param name="id"></param>
        public void Test(string id)
        {
            //add database vakaxa
//            var bitcoinRawTransactionRepo =
//                VakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);
//            var resultAddBitcoinRawTransactionAddress =
//                bitcoinRawTransactionRepo.FindBySql("SELECT * FROM bitcoinwithdrawtransaction");
//            Dictionary<string, string> openWith =
//                new Dictionary<string, string> {{"Status", Status.StatusPending}};
//            var resultAddBitcoinRawTransactionAddress =
//                bitcoinRawTransactionRepo.QuerySearch(openWith);

            Logger.Error("aaaaaaaaaaaaa", "SendTransaction exception");
            Console.WriteLine(1);
//            var transaction = bitcoinRpc.GetTransaction(Id);
//            var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data);
//            //Console.WriteLine(transaction);
//            //Console.WriteLine(transaction.Data);
//            var block = bitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
//            var blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data)["result"];
        }


        /// <summary>
        /// If [account] is not specified, returns the server's total available balance.
        /// If [account] is specified, returns the balance in the account.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="minconf"></param>
        public ReturnObject GetBalance(string account = "", int minconf = 1)
        {
            try
            {
                var results = BitcoinRpc.GetBalance(account, minconf);
                if (results.Status == Status.StatusError)
                    return results;
                var balance = results.Data;
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = balance
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "GetBalance exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }


        public ReturnObject GetTransaction(string txid)
        {
            try
            {
                var results = BitcoinRpc.GetTransaction(txid);
                return results;
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
    }
}