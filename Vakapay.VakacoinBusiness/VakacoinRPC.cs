using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.BlockchainBusiness;
using Vakapay.Models.Domains;
using Vakapay.Commons.Helpers;
using Vakapay.Cryptography;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using VakaSharp;
using VakaSharp.Api.v1;
using Action=VakaSharp.Api.v1.Action;

namespace Vakapay.VakacoinBusiness
{
    public class VakacoinRPC: IBlockchainRPC
    {
        public string EndPointURL { get; set; }
        private string ChainID { get; }
        private VakaApi DefaultApi { get; }
        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        
        private const string CoreSymbol = "VAKA";
        private const string SystemTokenContract = "vaka.token";
        private const string ActivePermission = "active";
        private const string TransferAction = "transfer";

        public VakacoinAccountRepository AccountRepository { get; set; }

        public VakacoinRPC(string endPointUrl, string chainId = null)
        {
            try
            {
                EndPointURL = endPointUrl;

                var vakaConfig = new VakaConfigurator()
                {
                    HttpEndpoint = EndPointURL,
                };
                DefaultApi = new VakaApi(vakaConfig);

                if (string.IsNullOrEmpty(chainId))
                {
                    var getInfoResult = DefaultApi.GetInfo().Result;
                    chainId = getInfoResult.ChainId;
                }

                ChainID = chainId;
                DefaultApi.Config.ChainId = ChainID;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        public bool CheckAccountExist(string accountName)
        {
            try
            {
                var account = DefaultApi.GetAccount(new GetAccountRequest() {AccountName = accountName}).Result;
                return !account.AccountName.Equals("");
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ReturnObject CreateRandomAccount(string ownerPublicKey, string activePublicKey = "")
        {
            try
            {
                if (string.IsNullOrEmpty(activePublicKey))
                {
                    activePublicKey = ownerPublicKey;
                }
                
                var accountName = "";
                do
                {
                    accountName = CommonHelper.RandomAccountNameVakacoin();
                } while (CheckAccountExist(accountName) == true);

                return CreateAccount(accountName, ownerPublicKey, activePublicKey);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        public ReturnObject CreateAccount(string accountName, string ownerPublicKey, string activePublicKey = "")
        {
            try
            {
                if (string.IsNullOrEmpty(activePublicKey))
                {
                    activePublicKey = ownerPublicKey;
                }

                // start CreateTransaction
                var vakaConfig = new VakaConfigurator()
                {
                    HttpEndpoint = EndPointURL,
                    ChainId = this.ChainID
                };

                var vaka = new Vaka(vakaConfig);

                var result = vaka.CreateTransaction(new Transaction()
                {
                    Actions = new List<Action>()
                    {
                        new Action()
                        {
                            Account = "vaka",
                            Authorization = new List<PermissionLevel>() { },
                            Name = "newaccountx",
                            Data = new
                            {
                                name = accountName,
                                owner = new
                                {
                                    threshold = 1,
                                    keys = new List<object>()
                                    {
                                        new {key = ownerPublicKey, weight = 1}
                                    },
                                    accounts = new List<object>(),
                                    waits = new List<object>()
                                },
                                active = new
                                {
                                    threshold = 1,
                                    keys = new List<object>()
                                    {
                                        new {key = activePublicKey, weight = 1}
                                    },
                                    accounts = new List<object>(),
                                    waits = new List<object>()
                                }
                            }
                        }
                    }
                }).Result;


                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = accountName
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }
        
        public ReturnObject SendTransaction(string sFrom, string sTo, string sAmount, string sMemo, string sPrivateKey)
        {
            return SendTransactionAsync(sFrom, sTo, sAmount, sMemo, sPrivateKey).Result;
        }
        
        public async Task<ReturnObject> SendTransactionAsync(string sFrom, string sTo, string sAmount, string sMemo, string sPrivateKey)
        {
            try
            {
                var vakaConfig = new VakaConfigurator()
                {
                    SignProvider = new DefaultSignProvider(sPrivateKey),
                    HttpEndpoint = EndPointURL,
                    ChainId = this.ChainID
                };
                
                var vaka = new Vaka(vakaConfig);

                var result = await vaka.CreateTransaction( new Transaction()
                {
                    Actions = new List<Action>()
                    {
                        new Action()
                        {
                            Account = SystemTokenContract,
                            Authorization = new List<PermissionLevel>()
                            {
                                new PermissionLevel() {Actor = sFrom, Permission = ActivePermission}
                            },
                            Name = TransferAction,
                            Data = new
                            {
                                from = sFrom, to = sTo, quantity = sAmount,
                                memo = sMemo
                            }
                        }
                    }
                });

                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = result
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }
        
        public ReturnObject GetCurrencyBalance(string username)
        {
            try
            {
                var result = DefaultApi.GetCurrencyBalance(new GetCurrencyBalanceRequest()
                {
                    Code = SystemTokenContract,
                    Account = username,
                    Symbol = CoreSymbol
                }).Result;
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = result.Assets[0]
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// Get last trusted block number
        /// </summary>
        /// <returns>Block number (can null)</returns>
        public UInt32? GetLastIrreversibleBlockNum()
        {
            return DefaultApi.GetInfo().Result.LastIrreversibleBlockNum;
        }

        /// <summary>
        /// Get block info of specify number or id
        /// </summary>
        /// <param name="blockNumber">Specify number or id of block</param>
        /// <returns>Block info</returns>
        public GetBlockResponse GetBlockByNumber(uint blockNumber)
        {
            return DefaultApi.GetBlock(new GetBlockRequest {BlockNumOrId = blockNumber.ToString()}).Result;
        }

        public ArrayList GetAllTransactionsInBlock(uint blockNumber)
        {
            try
            {
                var block = DefaultApi.GetBlock(new GetBlockRequest{BlockNumOrId =  blockNumber.ToString()}).Result;
                List<TransactionReceipt> transactions = block.Transactions;
                ArrayList jsonTrxs = new ArrayList();
                foreach (var transaction in transactions)
                {
                    if (JsonHelper.IsValidJson(transaction.Trx.ToString())) //true if transaction.trx is json format
                    {
                        var json = transaction.Trx;
                        jsonTrxs.Add(json);
                    }
                }
                return jsonTrxs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
                return null;
            }
        }
        
        public ReturnObject CreateNewAddress(string password)
        {
            throw new NotImplementedException();
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

        /// <summary>
        /// Send asyn transaction with transaction data
        /// </summary>
        /// <param name="blockchainTransaction"></param>
        /// <returns></returns>
        public async Task<ReturnObject> SendTransactionAsync(BlockchainTransaction blockchainTransaction)
        {
            var transaction = (VakacoinTransaction) blockchainTransaction;
            
            var senderInfo = AccountRepository.FindByAddress(blockchainTransaction.FromAddress);
            
            return await SendTransactionAsync(transaction.FromAddress, transaction.ToAddress,
                transaction.GetStringAmount(), transaction.Memo, senderInfo.GetSecret());
        }

        public ReturnObject GetBlockByNumber(int blockNumber)
        {
            throw new NotImplementedException();
        }

        public ReturnObject GetBlockNumber()
        {
            throw new NotImplementedException();
        }

        public ReturnObject GetBlockHaveTransactionByNumber(int i)
        {
            throw new NotImplementedException();
        }

        public ReturnObject FindTransactionByHash(string hash)
        {
            throw new NotImplementedException();
        }
    }
}