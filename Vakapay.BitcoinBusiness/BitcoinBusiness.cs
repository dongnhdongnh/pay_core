using System;
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


        public BitcoinBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true) :
            base(_vakapayRepositoryFactory, isNewConnection)
        {
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
        public ReturnObject CreateNewAddAddress(string Account = "", string WalletId = "")
        {
            try
            {
                var results = bitcoinRpc.GetNewAddress(Account);
                if (results.Status == Status.StatusError)
                    return results;

                var address = ConvertResult(results.Data);

                if (WalletId == "")
                {
                    //khoi tao wallet test
                    WalletId = CommonHelper.GenerateUuid();
                    var wallet = new Wallet
                    {
                        Id = WalletId,
                    };
                }


                //add database vakaxa
                var bitcoinAddressRepo = vakapayRepositoryFactory.GetBitcoinAddressRepository(DbConnection);
                var bcAddress = new BitcoinAddress
                {
                    Id = CommonHelper.GenerateUuid(),
                    Address = address,
                    WalletId = WalletId,
                    CreatedAt = (int) CommonHelper.GetUnixTimestamp(),
                    UpdatedAt = (int) CommonHelper.GetUnixTimestamp()
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
            return null;
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
            float amount,
            int minconf = 1,
            string comment = "",
            string commentTo = "")
        {
            try
            {
                var results = bitcoinRpc.SendFrom(fromAccount, toAddress, amount, minconf, comment, commentTo);

                var idTransaction = ConvertResult(results.Data);
                if (results.Status == Status.StatusError)
                    return results;

                //get transaction
                var transaction = bitcoinRpc.GetTransaction(idTransaction);
                var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data)["result"];


                //block
                var block = bitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
                var blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data)["result"];

                //add database vakaxa
                var bitcoinRawTransactionRepo =
                    vakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);
                var rawTransaction = new BitcoinWithdrawTransaction
                {
                    Id = CommonHelper.GenerateUuid(),
                    Hash = transactionInfo["blockhash"].ToString(),
                    BlockNumber = (string) blockInfo["height"],
                    NetworkName = "Bitcoin",
                    Amount = (decimal) amount,
                    FromAddress = fromAccount,
                    ToAddress = toAddress,
                    Fee = (decimal) transactionInfo["fee"],
                    Status = Status.StatusPending,
                    CreatedAt = CommonHelper.GetUnixTimestamp().ToString(),
                    UpdatedAt = CommonHelper.GetUnixTimestamp().ToString()
                };

                var ResultAddBitcoinRawTransactionAddress = bitcoinRawTransactionRepo.Insert(rawTransaction);
                //
                return ResultAddBitcoinRawTransactionAddress;
                //deposit
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = idTransaction
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

        public void test(String Id)
        {
            var transaction = bitcoinRpc.GetTransaction(Id);

            var transactionInfo = JsonConvert.DeserializeObject<JObject>(transaction.Data)["result"];

            Console.WriteLine(transaction.Data);
            Console.WriteLine((decimal) transactionInfo["fee"]);
            var block = bitcoinRpc.GetBlock((string) transactionInfo["blockhash"]);
            var blockInfo = JsonConvert.DeserializeObject<JObject>(block.Data)["result"];
            Console.WriteLine(blockInfo);
//            Console.WriteLine(JsonConvert.DeserializeObject<JObject>(transaction.Data)["result"] as JObject);
        }

        /// <summary>
        /// Send coins to address. Returns txid.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        public ReturnObject SendToAddress(string address,
            float amount,
            string comment = "",
            string commentTo = "")
        {
            try
            {
                var results = bitcoinRpc.SendToAddress(address, amount, comment, commentTo);
                if (results.Status == Status.StatusError)
                    return results;

                var idTransaction = ConvertResult(results.Data);
                //add database vakaxa
                var bitcoinRawTransactionRepo =
                    vakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);

                //
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = idTransaction
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
                var balance = ConvertResult(results.Data);
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
            return JsonConvert.DeserializeObject<JObject>(data)["result"].ToString();
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