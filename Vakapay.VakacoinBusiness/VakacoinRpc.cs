using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;
using Vakapay.Commons.Helpers;
using Vakapay.Cryptography;
using VakaSharp;
using VakaSharp.Api.v1;
using VakaSharp.Helpers;
using VakaSharp.Providers;
using Action=VakaSharp.Api.v1.Action;

namespace Vakapay.VakacoinBusiness
{
    public class VakacoinRpc
    {
        private string EndPointUrl { get; set; }
        private string ChainID { get; set; }
        private const string CoreSymbol = "VAKA";
        private const string SystemTokenContract = "vaka.token";
        private const string ActivePermission = "active";
        private const string TransferAction = "transfer";

        public VakacoinRpc(string endPointUrl, string chainId = null)
        {
            EndPointUrl = endPointUrl;
          
            if (string.IsNullOrEmpty(chainId))
            {
                var vakaConfig = new VakaConfigurator()
                {
                    HttpEndpoint = EndPointUrl,
                };
                var defaultApi = new VakaApi(vakaConfig);

                var getInfoResult = defaultApi.GetInfo().Result;
                chainId = getInfoResult.ChainId;
            }
            
            ChainID = chainId;
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

                var keyPair = KeyManager.GenerateKeyPair();
                
                //test TODO remove
                accountName = "liemlonglan2";

//                var args = new NewAccountxArgs()
//                {
//                    name = accountName,
//                    owner = new {
//                        threshold = 1,
//                        keys = new List<object>() {
//                            new { key = "EOS8Q8CJqwnSsV4A6HDBEqmQCqpQcBnhGME1RUvydDRnswNngpqfr", weight = 1}
//                        },
//                        accounts =  new List<object>(),
//                        waits =  new List<object>()
//                    },
//                    active = new {
//                        threshold = 1,
//                        keys = new List<object>() {
//                            new { key = "EOS8Q8CJqwnSsV4A6HDBEqmQCqpQcBnhGME1RUvydDRnswNngpqfr", weight = 1}
//                        },
//                        accounts =  new List<object>(),
//                        waits =  new List<object>()
//                    }
//                }; 
//                
//                var action = GetActionObject("", "newaccountx", "", "vaka", args);
//
//                List<string> privateKeysInWIF = new List<string> { }; // blank
//                
//                //push transaction
//                var transactionResult = ChainApiObj.PushTransaction(new [] { action }, privateKeysInWIF);
                
                return new ReturnObject
                {
                    Status = Status.StatusActive,
                    Data = accountName
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
        
        public ReturnObject SendTransaction(string sFrom, string sTo, string sAmount, string sMemo, string sPrivateKey)
        {
            try
            {
                var vakaConfig = new VakaConfigurator()
                {
                    SignProvider = new DefaultSignProvider(sPrivateKey),
                    HttpEndpoint = EndPointUrl,
                    ChainId = ChainID
                };
                var defaultApi = new VakaApi(vakaConfig);

                var getInfoResult = defaultApi.GetInfo().Result;
                var getBlockResult = defaultApi.GetBlock(new GetBlockRequest()
                {
                    BlockNumOrId = getInfoResult.LastIrreversibleBlockNum.Value.ToString()
                }).Result;

                var trx = new Transaction()
                {
                    //trx headers
                    Expiration = getInfoResult.HeadBlockTime.Value.AddSeconds(60), //expire Seconds
                    RefBlockNum = (UInt16) (getInfoResult.LastIrreversibleBlockNum.Value & 0xFFFF),
                    RefBlockPrefix = getBlockResult.RefBlockPrefix,
                    // trx info
                    MaxNetUsageWords = 0,
                    MaxCpuUsageMs = 0,
                    DelaySec = 0,
                    ContextFreeActions = new List<Action>(),
                    TransactionExtensions = new List<Extension>(),
                    Actions = new List<Action>()
                    {
                        new Action()
                        {
                            Account = SystemTokenContract,
                            Authorization = new List<PermissionLevel>()
                            {
                                new PermissionLevel() {Actor = sFrom, Permission = "active"}
                            },
                            Name = TransferAction,
                            Data = new
                            {
                                from = sFrom, to = sTo, quantity = sAmount,
                                memo = sMemo
                            }
                        }
                    }
                };

                var publicKey = KeyManager.GetVakaPublicKey(sPrivateKey);
                var abiSerializer = new AbiSerializationProvider(defaultApi);
                var packedTrx = abiSerializer.SerializePackedTransaction(trx).Result;
                var requiredKeys = new List<string>() {publicKey};
                var signatures = vakaConfig.SignProvider.Sign(defaultApi.Config.ChainId, requiredKeys, packedTrx)
                    .Result;

                var response = defaultApi.PushTransaction(new PushTransactionRequest()
                {
                    Signatures = signatures.ToArray(),
                    Compression = 0,
                    PackedContextFreeData = "",
                    PackedTrx = SerializationHelper.ByteArrayToHexString(packedTrx)
                }).Result;

                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
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
    }
}