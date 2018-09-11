using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;

namespace Vakapay.BitcoinBusiness
{
    public class BitcoinRpc
    {
        public BitcoinRpc()
        {
        }

        public BitcoinRpc(string a_sUri)
        {
            Url = new Uri(a_sUri);
        }

        public Uri Url;

        public ICredentials Credentials;

        public ReturnObject InvokeMethod(string a_sMethod, params object[] a_params)
        {
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(Url);

            webRequest.Credentials = Credentials;
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            JObject joe = new JObject();
            joe["jsonrpc"] = "1.0";
            joe["id"] = "1";
            joe["method"] = a_sMethod;

            if (a_params != null)
            {
                if (a_params.Length > 0)
                {
                    JArray props = new JArray();
                    foreach (var p in a_params)
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
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
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
                            return new ReturnObject
                            {
                                Status = Status.StatusCompleted,
                                Data = result,
                            };
                            // return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                        }
                    }
                }
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

        public void BackupWallet(string a_destination)
        {
            InvokeMethod("backupwallet", a_destination);
        }
        
        /**
         * Returns the account associated with the given address.
         */
        public ReturnObject GetAccount(string a_address)
        {
            return InvokeMethod("getaccount", a_address);
        }
        
        /**
         * Returns the current bitcoin address for receiving payments to this account. If <account> does not exist, it will be created along with an associated new address that will be returned.
         */
        public ReturnObject GetAccountAddress(string a_account)
        {
            return InvokeMethod("getaccountaddress", a_account);
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
        public ReturnObject GetBalance(string a_account = null, int a_minconf = 1)
        {
            if (a_account == null)
            {
                return InvokeMethod("getbalance");
            }

            return InvokeMethod("getbalance", a_account, a_minconf);
        }

        public ReturnObject GetBlockByCount(int a_height)
        {
            return InvokeMethod("getblockbycount", a_height);
        }

        public ReturnObject GetBlockNumber()
        {
            return InvokeMethod("getblocknumber");
        }

        public ReturnObject GetConnectionCount()
        {
            return InvokeMethod("getconnectioncount");
        }

        public ReturnObject GetDifficulty()
        {
            return InvokeMethod("getdifficulty");
        }

        public ReturnObject GetGenerate()
        {
            return InvokeMethod("getgenerate");
        }

        public ReturnObject GetHashesPerSec()
        {
            return InvokeMethod("gethashespersec");
        }

        public ReturnObject GetInfo()
        {
            return InvokeMethod("getinfo");
        }
        
        /**
         * Returns a new bitcoin address for receiving payments. If [account] is specified payments received with the address will be credited to [account].
         */
        public ReturnObject GetNewAddress(string a_account = "")
        {
            return InvokeMethod("getnewaddress", a_account);
        }

        public ReturnObject GetReceivedByAccount(string a_account, int a_minconf = 1)
        {
            return InvokeMethod("getreceivedbyaccount", a_account, a_minconf);
        }

        public ReturnObject GetReceivedByAddress(string a_address, int a_minconf = 1)
        {
            return InvokeMethod("getreceivedbyaddress", a_address, a_minconf);
        }

        public ReturnObject GetTransaction(string a_txid)
        {
            return InvokeMethod("gettransaction", a_txid);
        }

        public ReturnObject GetWork()
        {
            return InvokeMethod("getwork");
        }

        public ReturnObject GetWork(string a_data)
        {
            return InvokeMethod("getwork", a_data);
        }

        public ReturnObject Help(string a_command = "")
        {
            return InvokeMethod("help", a_command);
        }
        
        /**
         * Returns Object that has account names as keys, account balances as values.
         */
        public ReturnObject ListAccounts(int a_minconf = 1)
        {
            return InvokeMethod("listaccounts", a_minconf);
        }
        
        /**
        * Returns an array of objects containing:
            "account" : the account of the receiving addresses
            "amount" : total amount received by addresses with this account
            "confirmations" : number of confirmations of the most recent transaction included
        */
        public ReturnObject ListReceivedByAccount(int a_minconf = 1, bool a_includeEmpty = false)
        {
            return InvokeMethod("listreceivedbyaccount", a_minconf, a_includeEmpty);
        }
        
        /**
         * Returns an array of objects containing:
            "address" : receiving address
            "account" : the account of the receiving address
            "amount" : total amount received by the address
            "confirmations" : number of confirmations of the most recent transaction included
           To get a list of accounts on the system, execute bitcoind listreceivedbyaddress 0 true
         */
        public ReturnObject ListReceivedByAddress(int a_minconf = 1, bool a_includeEmpty = false)
        {
            return InvokeMethod("listreceivedbyaddress", a_minconf, a_includeEmpty);
        }
        
        /**
         * Returns up to [count] most recent transactions skipping the first [from] transactions for account [account]. If [account] not provided it'll return recent transactions from all accounts.
         */
        public ReturnObject ListTransactions(string a_account, int a_count = 10)
        {
            return InvokeMethod("listtransactions", a_account, a_count);
        }
        
        /**
         * Move from one account in your wallet to another
         */
        public ReturnObject Move(
            string a_fromAccount,
            string a_toAccount,
            float a_amount,
            int a_minconf = 1,
            string a_comment = ""
        )
        {
            return InvokeMethod(
                "move",
                a_fromAccount,
                a_toAccount,
                a_amount,
                a_minconf,
                a_comment
            );
        }
        
        /**
         *  <amount> is a real and is rounded to 8 decimal places. Will send the given amount to the given address, ensuring the account has a valid balance using [minconf] confirmations. Returns the transaction ID if successful (not in JSON object).
         */
        public ReturnObject SendFrom(string a_fromAccount,
            string a_toAddress,
            double a_amount,
            int a_minconf = 1,
            string a_comment = "",
            string a_commentTo = "")
        {
            return InvokeMethod(
                "sendfrom",
                a_fromAccount,
                a_toAddress,
                a_amount,
                a_minconf,
                a_comment,
                a_commentTo
            );
        }
        
        /**
         *  <amount> is a real and is rounded to 8 decimal places. Returns the transaction ID <txid> if successful.
         */
        public ReturnObject SendToAddress(string a_address, double a_amount, string a_comment, string a_commentTo)
        {
            return InvokeMethod("sendtoaddress", a_address, a_amount, a_comment, a_commentTo);
        }
        
        /**
         *  Sets the account associated with the given address. Assigning address that is already assigned to the same account will create a new address associated with that account.
         */
        public void SetAccount(string a_address, string a_account)
        {
            InvokeMethod("setaccount", a_address, a_account);
        }

        /**
         *  <generate> is true or false to turn generation on or off. Generation is limited to [genproclimit] processors, -1 is unlimited.
         */
        public void SetGenerate(bool a_generate, int a_genproclimit = 1)
        {
            InvokeMethod("setgenerate", a_generate, a_genproclimit);
        }

        public void Stop()
        {
            InvokeMethod("stop");
        }

        public ReturnObject ValidateAddress(string a_address)
        {
            return InvokeMethod("validateaddress", a_address);
        }

        // == Blockchain ==

        /**
         * The getbestblockhash RPC returns the header hash of the most recent block on the best block chain.
         */
        public ReturnObject getBestBlockHash()
        {
            return InvokeMethod("getbestblockhash");
        }

        /**
         * The getblock RPC gets a block with a particular header hash from the local block database either as a JSON object or as a serialized block.
         */
        public ReturnObject GetBlock(string blockhash)
        {
            return InvokeMethod("validateaddress", blockhash);
        }

        /**
         * The getblockchaininfo RPC provides information about the current state of the block chain.
         */
        public ReturnObject GetBlockChainInfo()
        {
            return InvokeMethod("getblockchaininfo");
        }

        /**
         * The getblockcount RPC returns the number of blocks in the local best block chain.
         */
        public ReturnObject GetBlockCount()
        {
            return InvokeMethod("getblockcount");
        }

        /**
         * The getblockhash RPC returns the header hash of a block at the given height in the local best block chain.
         */
        public ReturnObject GetBlockHash(int height)
        {
            return InvokeMethod("getblockhash", height);
        }

        /**
         * The getblockheader RPC gets a block header with a particular header hash from the local block database either as a JSON object or as a serialized block header.
         */
        public ReturnObject getBlockHeader(string hash)
        {
            return InvokeMethod("getblockheader", hash);
        }

        /**
         * The getblockheader RPC gets a block header with a particular header hash from the local block database either as a JSON object or as a serialized block header.(sequentially)
         */
        public ReturnObject getBlockHeader(string hash, bool isSequentially)
        {
            return InvokeMethod("getblockheader", hash, isSequentially);
        }

        public ReturnObject ListWallets()
        {
            return InvokeMethod("listwallets");
        }
    }
}