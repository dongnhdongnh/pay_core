using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vakapay.BlockchainBusiness;
using Vakapay.Models.Domains;

namespace Vakapay.BitcoinBusiness
{
    public class BitcoinRpc : IBlockchainRPC
    {
        private readonly Uri Url;

        private readonly ICredentials Credentials;

        public BitcoinRpc(string aSUri, string userName, string password)
        {
            Url = new Uri(aSUri);
            Credentials = new NetworkCredential(userName, password);
        }


        private IBlockchainRPC _blockchainRpcImplementation;

        private ReturnObject InvokeMethod(string aSMethod, params object[] aParams)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(Url);

                webRequest.Credentials = Credentials;
                webRequest.ContentType = "application/json-RPCClient";
                webRequest.Method = "POST";

                JObject joe = new JObject();
                joe["jsonrpc"] = "1.0";
                joe["id"] = "1";
                joe["method"] = aSMethod;

                if (aParams != null)
                {
                    if (aParams.Length > 0)
                    {
                        JArray props = new JArray();
                        foreach (var p in aParams)
                        {
                            props.Add(p);
                        }

                        joe.Add(new JProperty("params", props));
                    }
                }

                string s = JsonConvert.SerializeObject(joe);

                // serialize json for the request
                byte[] byteArray = Encoding.UTF8.GetBytes(s);
                webRequest.ContentLength = byteArray.Length;

                try
                {
                    using (Stream dataStream = webRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }
                }
                catch (Exception e)
                {
                    return returnError(e);
                }

                try
                {
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (Stream str = webResponse.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(str))
                            {
                                var result = sr.ReadToEnd();

                                var results = JsonConvert.DeserializeObject<JObject>(result);


                                if (string.IsNullOrEmpty(results["error"].ToString()))
                                {
                                    return new ReturnObject
                                    {
                                        Status = Status.StatusCompleted,
                                        Data = results["result"].ToString()
                                    };
                                }

                                return new ReturnObject
                                {
                                    Status = Status.StatusError,
                                    Message = results["error"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    return returnError(e);
                }
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public void BackupWallet(string aDestination)
        {
            try
            {
                InvokeMethod("backupwallet", aDestination);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /**
         * Returns the account associated with the given address.
         */
        public ReturnObject GetAccount(string aAddress)
        {
            try
            {
                return InvokeMethod("getaccount", aAddress);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Returns the current bitcoin address for receiving payments to this account. If <account> does not exist, it will be created along with an associated new address that will be returned.
         */
        public ReturnObject GetAccountAddress(string aAccount)
        {
            try
            {
                return InvokeMethod("getaccountaddress", aAccount);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

//        public IEnumerable<string> GetAddressesByAccount(string a_account)
//        {
//            return from o in InvokeMethod("getaddressesbyaccount", a_account)["result"]
//                select o.ToString();
//        }

        /**
         * If [account] is not specified, returns the server's total available balance.
           If [account] is specified, returns the balance in the account.
         */
        public ReturnObject GetBalance(string aAccount = null, int aMinconf = 1)
        {
            try
            {
                if (aAccount == null)
                {
                    return InvokeMethod("getbalance");
                }

                return InvokeMethod("getbalance", aAccount, aMinconf);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject GetBlockByCount(int aHeight)
        {
            try
            {
                return InvokeMethod("getblockbycount", aHeight);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject GetBlockNumber()
        {
            try
            {
                return InvokeMethod("getblocknumber");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject GetConnectionCount()
        {
            try
            {
                return InvokeMethod("getconnectioncount");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject GetDifficulty()
        {
            try
            {
                return InvokeMethod("getdifficulty");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject GetGenerate()
        {
            try
            {
                return InvokeMethod("getgenerate");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject GetHashesPerSec()
        {
            try
            {
                return InvokeMethod("gethashespersec");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject GetInfo()
        {
            try
            {
                return InvokeMethod("getinfo");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Returns a new bitcoin address for receiving payments. If [account] is specified payments received with the address will be credited to [account].
         */
        public ReturnObject GetNewAddress(string aAccount = "")
        {
            try
            {
                return InvokeMethod("getnewaddress", aAccount);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Returns the total amount received by addresses with [account] in transactions with at least [minconf] confirmations. If [account] not provided return will include all transactions to all accounts.
         */
        public ReturnObject GetReceivedByAccount(string aAccount, int aMinconf = 1)
        {
            try
            {
                return InvokeMethod("getreceivedbyaccount", aAccount, aMinconf);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Returns the amount received by <bitcoinaddress> in transactions with at least [minconf] confirmations. It correctly handles the case where someone has sent to the address in multiple transactions. Keep in mind that addresses are only ever used for receiving transactions. Works only for addresses in the local wallet, external addresses will always show 0
         */
        public ReturnObject GetReceivedByAddress(string aAddress, int aMinconf = 1)
        {
            try
            {
                return InvokeMethod("getreceivedbyaddress", aAddress, aMinconf);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Returns an object about the given transaction containing:
            "amount" : total amount of the transaction
            "confirmations" : number of confirmations of the transaction
            "txid" : the transaction ID
            "time" : time associated with the transaction[1].
            "details" - An array of objects containing:
            "account"
            "address"
            "category"
            "amount"
            "fee"
         */
        public ReturnObject GetTransaction(string aTxid)
        {
            try
            {
                return InvokeMethod("gettransaction", aTxid);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * If [data] is not specified, returns formatted hash data to work on:
            "midstate" : precomputed hash state after hashing the first half of the data
            "data" : block data
            "hash1" : formatted hash buffer for second hash
            "target" : little endian hash target
           If [data] is specified, tries to solve the block and returns true if it was successful.
         */
        public ReturnObject GetWork(string aData = "")
        {
            try
            {
                return InvokeMethod("getwork", aData);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject Help(string aCommand = "")
        {
            try
            {
                return InvokeMethod("help", aCommand);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Returns Object that has account names as keys, account balances as values.
         */
        public ReturnObject ListAccounts(int aMinconf = 1)
        {
            try
            {
                return InvokeMethod("listaccounts", aMinconf);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
        * Returns an array of objects containing:
            "account" : the account of the receiving addresses
            "amount" : total amount received by addresses with this account
            "confirmations" : number of confirmations of the most recent transaction included
        */
        public ReturnObject ListReceivedByAccount(int aMinconf = 1, bool aIncludeEmpty = false)
        {
            try
            {
                return InvokeMethod("listreceivedbyaccount", aMinconf, aIncludeEmpty);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Returns an array of objects containing:
            "address" : receiving address
            "account" : the account of the receiving address
            "amount" : total amount received by the address
            "confirmations" : number of confirmations of the most recent transaction included
           To get a list of accounts on the system, execute bitcoind listreceivedbyaddress 0 true
         */
        public ReturnObject ListReceivedByAddress(int aMinconf = 1, bool aIncludeEmpty = false)
        {
            try
            {
                return InvokeMethod("listreceivedbyaddress", aMinconf, aIncludeEmpty);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Returns up to [count] most recent transactions skipping the first [from] transactions for account [account]. If [account] not provided it'll return recent transactions from all accounts.
         */
        public ReturnObject ListTransactions(string aAccount, int aCount = 10)
        {
            try
            {
                return InvokeMethod("listtransactions", aAccount, aCount);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * Move from one account in your wallet to another
         */
        public ReturnObject Move(
            string aFromAccount,
            string aToAccount,
            float aAmount,
            int aMinconf = 1,
            string aComment = ""
        )
        {
            try
            {
                return InvokeMethod(
                    "move",
                    aFromAccount,
                    aToAccount,
                    aAmount,
                    aMinconf,
                    aComment
                );
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * amounts are double-precision floating point numbers
         */
        public ReturnObject Sendmany(string fromAccount, JObject mutiAddress, int minconf = 1, string comment = "")
        {
            try
            {
                return InvokeMethod(
                    "sendmany",
                    fromAccount,
                    mutiAddress,
                    minconf,
                    comment
                );
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         *  <amount> is a real and is rounded to 8 decimal places. Will send the given amount to the given address, ensuring the account has a valid balance using [minconf] confirmations. Returns the transaction ID if successful (not in JSON object).
         */
        public ReturnObject SendFrom(string aFromAccount,
            string aToAddress,
            double aAmount,
            int aMinconf = 1,
            string aComment = "",
            string aCommentTo = "")
        {
            try
            {
                return InvokeMethod(
                    "sendfrom",
                    aFromAccount,
                    aToAddress,
                    aAmount,
                    aMinconf,
                    aComment,
                    aCommentTo
                );
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         *  <amount> is a real and is rounded to 8 decimal places. Returns the transaction ID <txid> if successful.
         */
        public ReturnObject SendToAddress(string aAddress, decimal aAmount, string aComment = "",
            string aCommentTo = "")
        {
            try
            {
                return InvokeMethod("sendtoaddress", aAddress, aAmount, aComment, aCommentTo);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         *  Sets the account associated with the given address. Assigning address that is already assigned to the same account will create a new address associated with that account.
         */
        public void SetAccount(string aAddress, string aAccount)
        {
            try
            {
                InvokeMethod("setaccount", aAddress, aAccount);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /**
         *  <generate> is true or false to turn generation on or off. Generation is limited to [genproclimit] processors, -1 is unlimited.
         */
        public void SetGenerate(bool aGenerate, int aGenproclimit = 1)
        {
            try
            {
                InvokeMethod("setgenerate", aGenerate, aGenproclimit);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                InvokeMethod("stop");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ReturnObject ValidateAddress(string aAddress)
        {
            try
            {
                return InvokeMethod("validateaddress", aAddress);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        // == Blockchain ==

        /**
         * The getbestblockhash RPC returns the header hash of the most recent block on the best block chain.
         */
        public ReturnObject GetBestBlockHash()
        {
            try
            {
                return InvokeMethod("getbestblockhash");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * The getblock RPC gets a block with a particular header hash from the local block database either as a JSON object or as a serialized block.
         */
        public ReturnObject GetBlock(string blockhash)
        {
            try
            {
                return InvokeMethod("getblock", blockhash);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * The getblockchaininfo RPC provides information about the current state of the block chain.
         */
        public ReturnObject GetBlockChainInfo()
        {
            try
            {
                return InvokeMethod("getblockchaininfo");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * The getblockcount RPC returns the number of blocks in the local best block chain.
         */
        public ReturnObject GetBlockCount()
        {
            try
            {
                return InvokeMethod("getblockcount");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }


        /**
         * The getblockheader RPC gets a block header with a particular header hash from the local block database either as a JSON object or as a serialized block header.
         */
        public ReturnObject GetBlockHeader(string hash)
        {
            try
            {
                return InvokeMethod("getblockheader", hash);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * The getblockheader RPC gets a block header with a particular header hash from the local block database either as a JSON object or as a serialized block header.(sequentially)
         */
        public ReturnObject GetBlockHeader(string hash, bool isSequentially)
        {
            try
            {
                return InvokeMethod("getblockheader", hash, isSequentially);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject ListWallets()
        {
            try
            {
                return InvokeMethod("listwallets");
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        private ReturnObject returnError(Exception e)
        {
            return new ReturnObject
            {
                Status = Status.StatusError,
                Message = e.Message
            };
        }

        public string EndPointURL { get; set; }

        public ReturnObject CreateNewAddress(string account)
        {
            try
            {
                var result = GetNewAddress(account);
                return result;
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject CreateNewAddress()
        {
            throw new NotImplementedException();
        }

        public ReturnObject CreateNewAddress(string privateKey, string publicKey)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SendRawTransaction(string data)
        {
            throw new NotImplementedException();
        }

        public ReturnObject GetBalance(string address)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SignTransaction(string privateKey, object[] transactionData)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnObject> SendTransactionAsync(BlockchainTransaction blockchainTransaction)
        {
            try
            {
                var resultSend = SendToAddress(blockchainTransaction.ToAddress, blockchainTransaction.Amount);
                return resultSend;
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        /**
         * The getblockhash RPC returns the header hash of a block at the given height in the local best block chain.
         */
        public ReturnObject GetBlockByNumber(int blockNumber)
        {
            try
            {
                return InvokeMethod("getblockhash", blockNumber);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }

        public ReturnObject FindTransactionByHash(string hash)
        {
            try
            {
                return InvokeMethod("gettransaction", hash);
            }
            catch (Exception e)
            {
                return returnError(e);
            }
        }
    }
}