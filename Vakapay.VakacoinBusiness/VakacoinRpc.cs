using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Commons.Helpers;
using Vakapay.Cryptography;
using VakaSharp;
using VakaSharp.Api.v1;
using Action=VakaSharp.Api.v1.Action;

namespace Vakapay.VakacoinBusiness
{
    public class VakacoinRpc
    {
        private string EndPointUrl { get; }
        private string ChainID { get; }
        private const string CoreSymbol = "VAKA";
        private const string SystemTokenContract = "vaka.token";
        private const string ActivePermission = "active";
        private const string TransferAction = "transfer";
        private VakaApi DefaultApi { get; }
        public Logger Logger { get; } = NLog.LogManager.GetCurrentClassLogger();

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

                this.ChainID = chainId;
                DefaultApi.Config.ChainId = ChainID;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        private string GetAccountPostUrl()
        {
            return EndPointUrl + "/v1/chain/get_account";
        }

        private string GetInfoPostUrl()
        {
            return EndPointUrl + "/v1/chain/get_info";
        }

        public ReturnObject CreateAddress(string password)
        {
            return null;
        }

        public bool CheckAccountExist(string accountName)
        {
            var values = new Dictionary<string, string>
            {
                { "account_name", accountName },
            };

            var result = HttpClientHelper.PostRequest(GetAccountPostUrl(), values);
            var jResult= JObject.Parse(result);
            var exist = jResult["error"] == null;
            return exist;
        }

        public ReturnObject CreateRandomAccount()
        {
            try
            {
                var accountName = "";
                do
                {
                    accountName = CommonHelper.RandomAccountNameVakacoin();
                } while (CheckAccountExist(accountName) == true);

                return CreateAccount(accountName);
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

        public ReturnObject CreateAccount(string accountName)
        {
            try
            {
                var keyPair = KeyManager.GenerateKeyPair();

                // start CreateTransaction
                var vakaConfig = new VakaConfigurator()
                {
                    HttpEndpoint = EndPointUrl,
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
                                        new {key = keyPair.PublicKey, weight = 1}
                                    },
                                    accounts = new List<object>(),
                                    waits = new List<object>()
                                },
                                active = new
                                {
                                    threshold = 1,
                                    keys = new List<object>()
                                    {
                                        new {key = keyPair.PublicKey, weight = 1}
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
                    Data = accountName,
                    Message = keyPair.PrivateKey
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
            try
            {
                var vakaConfig = new VakaConfigurator()
                {
                    SignProvider = new DefaultSignProvider(sPrivateKey),
                    HttpEndpoint = EndPointUrl,
                    ChainId = this.ChainID
                };
                
                var vaka = new Vaka(vakaConfig);

                var result = vaka.CreateTransaction( new Transaction()
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
                }).Result;

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
                    Symbol = "VAKA"
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

        public int GetHeadBlockNumber()
        {
            var info = DefaultApi.GetInfo().Result;
            return Int32.Parse(info.HeadBlockNum.ToString());
        }

        //Return arraylist of Transaction in a block
        public ArrayList GetAllTransactionsInBlock(string blockNumber)
        {
            try
            {
                var block = DefaultApi.GetBlock(new GetBlockRequest{BlockNumOrId =  blockNumber}).Result;
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
    }
}