using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;
using Vakapay.Commons.Helpers;
using VakacoinCore;
using VakacoinCore.ActionArgs;
using VakacoinCore.Response;
using VakacoinCore.Response.API;
using VakacoinCore.Utilities;
using Action=VakacoinCore.Params.Action;


namespace Vakapay.VakacoinBusiness
{
    public class VakacoinRpc
    {
        private string EndPointUrl { get; set; }
        private ChainAPI ChainApiObj { get; }
        private const string CoreSymbol = "VAKA";
        private const string SystemTokenContract = "vaka.token";
        private const string ActivePermission = "active";
        private const string TransferAction = "transfer";

        public VakacoinRpc(string endPointUrl)
        {
            ChainApiObj = new ChainAPI(endPointUrl);
            EndPointUrl = endPointUrl;
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

        private Action GetActionObject(string accountName, string actionName, string permission, string code, object args)
        {
            return new ActionUtility(EndPointUrl).GetActionObject(accountName, actionName, permission, code, args);
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

        public ReturnObject CreateRandomAccount(out Account account)
        {
            account = null;
            try
            {
                var accountName = "";
                do
                {
                    accountName = CommonHelper.RandomAccountNameVakacoin();
                } while (CheckAccountExist(accountName) == true);

                var keyPair = KeyManager.GenerateKeyPair();

                var args = new NewAccountxArgs() {name = accountName}; 
                
                var action = GetActionObject("", "newaccountx", "", "vaka", args);

                List<string> privateKeysInWIF = new List<string> { }; // blank
                
                //push transaction
                var transactionResult = ChainApiObj.PushTransaction(new [] { action }, privateKeysInWIF);
                
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
        
        public ReturnObject SendTransaction(string from, string to, string amount, string memo, string privateKey)
        {
            try
            {
                //var sAmount = string.Format("{0:0.0000}", amount) + " " + CoreSymbol;
                
                var args = new TransferArgs() {from = from, to = to, quantity = amount, memo = memo};
                
                //prepare action object
                var action = GetActionObject(from, TransferAction, ActivePermission, SystemTokenContract, args);

                List<string> privateKeysInWIF = new List<string> { privateKey };
                
                //push transaction
                var transactionResult = ChainApiObj.PushTransaction(new [] { action }, privateKeysInWIF);
                
                return new ReturnObject(){Status = Status.StatusSuccess, Data = transactionResult.transaction_id};
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