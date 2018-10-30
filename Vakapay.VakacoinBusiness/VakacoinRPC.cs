using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using Vakapay.BlockchainBusiness;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Entities.VAKA;
using Vakapay.Repositories.Mysql;
using VakaSharp;
using VakaSharp.Api.v1;
using Action = VakaSharp.Api.v1.Action;

namespace Vakapay.VakacoinBusiness
{
    public class VakacoinRpc : IBlockchainRpc
    {
        public string EndPointUrl { get; set; }
        private string ChainId { get; }
        private VakaApi DefaultApi { get; }
        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        private const string CORE_SYMBOL = "VAKA";
        private const string SYSTEM_TOKEN_CONTRACT = "vaka.token";
        private const string ACTIVE_PERMISSION = "active";
        private const string TRANSFER_ACTION = "transfer";

        public VakacoinAccountRepository AccountRepository { get; set; }

        public VakacoinRpc(string endPointUrl, string chainId = null)
        {
            try
            {
                EndPointUrl = endPointUrl;

                var vakaConfig = new VakaConfigurator()
                {
                    HttpEndpoint = EndPointUrl,
                };
                DefaultApi = new VakaApi(vakaConfig);

                if (string.IsNullOrEmpty(chainId))
                {
                    var getInfoResult = DefaultApi.GetInfo().Result;
                    chainId = getInfoResult.ChainId;
                }

                ChainId = chainId;
                DefaultApi.Config.ChainId = ChainId;
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
                return !string.IsNullOrEmpty(account.AccountName);
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

                string accountName;
                do
                {
                    accountName = CommonHelper.RandomAccountNameVakacoin();
                } while (CheckAccountExist(accountName));

                return CreateAccount(accountName, ownerPublicKey, activePublicKey);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
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
                    HttpEndpoint = EndPointUrl,
                    ChainId = ChainId
                };

                var vaka = new Vaka(vakaConfig);

                vaka.CreateTransaction(new Transaction()
                {
                    Actions = new List<Action>()
                    {
                        new Action()
                        {
                            Account = "vaka",
                            Authorization = new List<PermissionLevel>(),
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
                });


                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = accountName
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        public ReturnObject SendTransaction(string sFrom, string sTo, string sAmount, string sMemo, string sPrivateKey)
        {
            return SendTransactionAsync(sFrom, sTo, sAmount, sMemo, sPrivateKey).Result;
        }

        public async Task<ReturnObject> SendTransactionAsync(string sFrom, string sTo, string sAmount, string sMemo,
            string sPrivateKey)
        {
            try
            {
                var vakaConfig = new VakaConfigurator()
                {
                    SignProvider = new DefaultSignProvider(sPrivateKey),
                    HttpEndpoint = EndPointUrl,
                    ChainId = ChainId
                };

                var vaka = new Vaka(vakaConfig);

                var result = await vaka.CreateTransaction(new Transaction()
                {
                    Actions = new List<Action>()
                    {
                        new Action()
                        {
                            Account = SYSTEM_TOKEN_CONTRACT,
                            Authorization = new List<PermissionLevel>()
                            {
                                new PermissionLevel() {Actor = sFrom, Permission = ACTIVE_PERMISSION}
                            },
                            Name = TRANSFER_ACTION,
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
                    Status = Status.STATUS_COMPLETED,
                    Data = result
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
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
                    Code = SYSTEM_TOKEN_CONTRACT,
                    Account = username,
                    Symbol = CORE_SYMBOL
                }).Result;
                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = result.Assets[0]
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
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
            return CreateRandomAccount(publicKey);
        }

        public ReturnObject SendRawTransaction(string data)
        {
            throw new NotImplementedException();
        }

        public ReturnObject GetBalance(string address)
        {
            return GetCurrencyBalance(address);
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
            try
            {
                var transaction = (VakacoinTransaction)blockchainTransaction;

                var senderInfo = AccountRepository.FindByAddress(blockchainTransaction.FromAddress);

                if (senderInfo == null)
                {
                    throw new Exception("Not found sender key!");
                }

                var memo = transaction.Memo ?? "";

                return await SendTransactionAsync(transaction.FromAddress, transaction.ToAddress,
                    transaction.GetStringAmount(), memo, senderInfo.ActivePrivateKey);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message,
                };
            }
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

        public ReturnObject GetInfo()
        {
            try
            {
                var rest = DefaultApi.GetInfo().Result;
                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = rest.ToString()
                };
            }
            catch (Exception e)
            {
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }
    }
}