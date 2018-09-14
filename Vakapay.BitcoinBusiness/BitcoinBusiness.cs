using System;
using System.Data;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.BitcoinBusiness
{
    using BlockchainBusiness;

    public class BitcoinBusiness : BlockchainBusiness
    {
        private BitcoinRpc bitcoinRpc { get; set; }
        public IVakapayRepositoryFactory factory { get; set; }
        public IDbConnection dbconnect { get; set; }


        public BitcoinBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true) :
            base(_vakapayRepositoryFactory, isNewConnection)
        {
            factory = _vakapayRepositoryFactory;
            dbconnect = DbConnection;
            bitcoinRpc = new BitcoinRpc("http://127.0.0.1:18443");
            bitcoinRpc.Credentials = new NetworkCredential("bitcoinrpc", "wqfgewgewi");
        }

        // <summary>
        // Returns a new bitcoin address for receiving payments.
        // If [account] is specified (recommended), it is added to the address book so payments 
        // received with the address will be credited to [account].
        // </summary>
        // <param name="a_account"></param>
        // bitcoin ver 16 GetNewAddress(Account); ver 17 GetNewAddress(Label)
        public ReturnObject CreateNewAddAddress(string WalletId, string Account = "")
        {
            try
            {
                var walletRepository = VakapayRepositoryFactory.GetWalletRepository(DbConnection);

                var walletCheck = walletRepository.FindById(WalletId);

                if (walletCheck == null)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Wallet Not Found"
                    };

                var results = bitcoinRpc.GetNewAddress(Account);
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
                    WalletId = WalletId,
                    CreatedAt = time,
                    UpdatedAt = time
                };

                var ResultAddBitcoinAddress = bitcoinAddressRepo.Insert(bcAddress);
                //
                return ResultAddBitcoinAddress;
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


        public ReturnObject SendTransaction(BitcoinWithdrawTransaction blockchainTransaction)
        {
            try
            {
                var results = bitcoinRpc.SendToAddress(blockchainTransaction.ToAddress, blockchainTransaction.Amount);

                if (results.Status == Status.StatusError)
                    return results;

                var idTransaction = results.Data;

                //get transaction
                var transaction = bitcoinRpc.GetTransaction(idTransaction);
                if (transaction.Status == Status.StatusError)
                    return transaction;
                var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data);


                //block
                var blockInfo = new JObject();
                if (!string.IsNullOrEmpty((string) transactionInfo["blockhash"]))
                {
                    var block = bitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
                    if (block.Status == Status.StatusError)
                        return block;
                    blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data);
                }


                //add database vakaxa
                var bitcoinRawTransactionRepo =
                    VakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);

                var time = CommonHelper.GetUnixTimestamp().ToString();
                
                
                blockchainTransaction.Status = Status.StatusCompleted;
                blockchainTransaction.CreatedAt = Status.StatusCompleted;
                blockchainTransaction.UpdatedAt = Status.StatusCompleted;
                blockchainTransaction.Hash = idTransaction;
                
//                var rawTransaction = new BitcoinWithdrawTransaction
//                {
//                    Id = CommonHelper.GenerateUuid(),
//                    Hash = idTransaction,
//                    BlockNumber = (string) blockInfo["height"],
//                    BlockHash = (string) transactionInfo["blockhash"],
//                    NetworkName = "Bitcoin",
//                    Amount = blockchainTransaction.Amount,
//                    FromAddress = "",
//                    ToAddress = blockchainTransaction.ToAddress,
//                    Fee = (decimal) transactionInfo["fee"] * -1,
//                    Status = Status.StatusPending,
//                    CreatedAt = time,
//                    UpdatedAt = time
//                };

                var ResultAddBitcoinRawTransactionAddress = bitcoinRawTransactionRepo.Insert(blockchainTransaction);

//                //update balance wallet
//                decimal balanceChange = (decimal) transactionInfo["amount"] + (decimal) transactionInfo["fee"];
//                // Id va version dang tam fix cung de test
//                UpdateBalanceWallet(balanceChange, "97f3f010-658c-46eb-92a4-52b6a3f51e15", 159900);

                //
                return ResultAddBitcoinRawTransactionAddress;
                //deposit
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
                var results = bitcoinRpc.Sendmany(fromAccount, mutiAddress, minconf, comment);

                if (results.Status == Status.StatusError)
                    return results;

                var idTransaction = results.Data;

                //get transaction
                var transaction = bitcoinRpc.GetTransaction(idTransaction);
                if (transaction.Status == Status.StatusError)
                    return transaction;
                var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data);


                // get block
                var blockInfo = new JObject();
                if (!string.IsNullOrEmpty((string) transactionInfo["blockhash"]))
                {
                    var block = bitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
                    if (block.Status == Status.StatusError)
                        return block;
                    blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data);
                }


                var bitcoinRawTransactionRepo =
                    VakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);
                var details = transactionInfo["details"];

                var data = new JArray();

                foreach (var detail in details)
                {
                    var time = CommonHelper.GetUnixTimestamp().ToString();
                    var rawTransaction = new BitcoinWithdrawTransaction
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Hash = idTransaction,
                        BlockNumber = (string) blockInfo["height"],
                        BlockHash = (string) transactionInfo["blockhash"],
                        NetworkName = "Bitcoin",
                        Amount = (decimal) detail["amount"] * -1,
                        FromAddress = "",
                        ToAddress = (string) detail["address"],
                        Fee = (decimal) detail["fee"] * -1,
                        Status = Status.StatusCompleted,
                        CreatedAt = time,
                        UpdatedAt = time
                    };

                    var ResultAddBitcoinRawTransactionAddress = bitcoinRawTransactionRepo.Insert(rawTransaction);

                    data.Add(ResultAddBitcoinRawTransactionAddress.Data);
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
                var results = bitcoinRpc.SendFrom(fromAccount, toAddress, amount, minconf, comment, commentTo);

                if (results.Status == Status.StatusError)
                    return results;

                var idTransaction = results.Data;

                //get transaction
                var transaction = bitcoinRpc.GetTransaction(idTransaction);
                if (transaction.Status == Status.StatusError)
                    return transaction;
                var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data);


                //block
                var blockInfo = new JObject();
                if (!string.IsNullOrEmpty((string) transactionInfo["blockhash"]))
                {
                    var block = bitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
                    if (block.Status == Status.StatusError)
                        return block;
                    blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data);
                }


                //add database vakaxa
                var bitcoinRawTransactionRepo =
                    VakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);

                var time = CommonHelper.GetUnixTimestamp().ToString();
                var rawTransaction = new BitcoinWithdrawTransaction
                {
                    Id = CommonHelper.GenerateUuid(),
                    Hash = idTransaction,
                    BlockNumber = (string) blockInfo["height"],
                    BlockHash = (string) transactionInfo["blockhash"],
                    NetworkName = "Bitcoin",
                    Amount = (decimal) amount,
                    FromAddress = "",
                    ToAddress = toAddress,
                    Fee = (decimal) transactionInfo["fee"] * -1,
                    Status = Status.StatusCompleted,
                    CreatedAt = time,
                    UpdatedAt = time
                };

                var ResultAddBitcoinRawTransactionAddress = bitcoinRawTransactionRepo.Insert(rawTransaction);
                //

//                //update balance wallet
//                var balanceChange = (decimal) transactionInfo["amount"] + (decimal) transactionInfo["fee"];
//                // Id va version dang tam fix cung de test
//                UpdateBalanceWallet(balanceChange, "97f3f010-658c-46eb-92a4-52b6a3f51e15", 159900);
                return ResultAddBitcoinRawTransactionAddress;
                //deposit
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
        /// gettransaction by txid
        /// </summary>
        /// <param name="Txid"></param>
        public ReturnObject TransactionByTxidBlockchain(String Txid)
        {
            try
            {
                var results = bitcoinRpc.GetTransaction(Txid);
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
        /// <param name="Account"></param>
        public ReturnObject ListtransactionByAccount(String Account)
        {
            try
            {
                var results = bitcoinRpc.ListTransactions(Account);
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
        public ReturnObject UpdateBalanceWallet(decimal balance, string WalletId, int version)
        {
            try
            {
                var walletRepository = VakapayRepositoryFactory.GetWalletRepository(DbConnection);

                //chua biet walletid, version lay tu dau
                var walletCheck = walletRepository.FindById(WalletId);


                if (walletCheck == null)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Wallet not found"
                    };

                return walletRepository.UpdateBalanceWallet(balance, WalletId, version);
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
        /// test 
        /// </summary>
        /// <param name="Id"></param>
        public void test(String Id)
        {
            //add database vakaxa
            var bitcoinRawTransactionRepo =
                VakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);
            var ResultAddBitcoinRawTransactionAddress =
                bitcoinRawTransactionRepo.FindBySql("SELECT * FROM bitcoinwithdrawtransaction");
//            var transaction = bitcoinRpc.GetTransaction(Id);
//            var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data);
//            //Console.WriteLine(transaction);
//            //Console.WriteLine(transaction.Data);
//            var block = bitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
//            var blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data)["result"];
        }

        /// <summary>
        /// Send coins to address. Returns txid.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
//        public ReturnObject SendToAddress(string address,
//            float amount,
//            string comment = "",
//            string commentTo = "")
//        {
//            try
//            {
//                var results = bitcoinRpc.SendToAddress(address, amount, comment, commentTo);
//                if (results.Status == Status.StatusError)
//                    return results;
//
//                var idTransaction = results.Data;
//                //add database vakaxa
//                var bitcoinRawTransactionRepo =
//                    vakapayRepositoryvakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);
//
//                //
//                return new ReturnObject
//                {
//                    Status = Status.StatusSuccess,
//                    Data = idTransaction
//                };
//            }
//            catch (Exception e)
//            {
//                return new ReturnObject
//                {
//                    Status = Status.StatusError,
//                    Message = e.Message
//                };
//            }
//        }

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
                var results = bitcoinRpc.GetBalance(account, minconf);
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
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        private String ConvertResult(String data)
        {
            return JsonConvert.DeserializeObject<JObject>(data).ToString();
        }

        public ReturnObject GetTransaction(string txid)
        {
            try
            {
                var results = bitcoinRpc.GetTransaction(txid);
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