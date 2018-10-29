using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.Models.Domains;
using Vakapay.Commons.Constants;
using Newtonsoft.Json.Linq;
using Vakapay.ApiServer.Models;
using Vakapay.Models.ClientRequest;
using Vakapay.Models.Entities;

namespace Vakapay.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : Controller
    {
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }
        WalletBusiness.WalletBusiness _walletBusiness;
        UserBusiness.UserBusiness _userBusiness;

        public WalletController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };
             PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _walletBusiness =
                new Vakapay.WalletBusiness.WalletBusiness(PersistenceFactory);
            _userBusiness
                = new Vakapay.UserBusiness.UserBusiness(PersistenceFactory);
        }

        [HttpGet("test")]
        public ActionResult<string> GetTest()
        {
            return "hello";
            //  return null;
        }

        [HttpGet("all")]
        public ActionResult<ReturnObject> GetWalletsByUser([FromQuery] string userId)
        {
            try
            {
                var user = _userBusiness.GetUserById(userId);
                if (user == null)
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "No user"
                    };
                var wallets = _walletBusiness.LoadAllWalletByUser(user);
                return new ReturnObject()
                {
                    Status = Status.STATUS_COMPLETED,
                    Message = JsonHelper.SerializeObject(wallets)
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

        //[HttpPost("all")]
        //public ActionResult<string> GetWalletsByUser([FromBody]User user)
        //{
        //    var wallets = _walletBusiness.LoadAllWalletByUser(user);
        //    return JsonHelper.SerializeObject(wallets);
        //    //  return null;
        //}
        //  WalletBusiness.WalletBusiness walletBusiness = new WalletBusiness.WalletBusiness();
        [HttpGet("Infor")]
        public ActionResult<string> GetWalletInfor([FromQuery] string walletID)
        {
            var wallet = _walletBusiness.GetWalletById(walletID);
            return JsonHelper.SerializeObject(wallet);
            //  return null;
        }

        [HttpGet("AddressInfor")]
        public ActionResult<ReturnObject> GetAddresses([FromQuery] string walletId, [FromQuery] string networkName)
        {
            try
            {
                var addresses = _walletBusiness.GetAddresses(walletId, networkName);
                return new ReturnObject()
                {
                    Status = Status.STATUS_COMPLETED,
                    // Data = numberData.ToString(),
                    Message = JsonHelper.SerializeObject(addresses)
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

            //  return null;
        }

        [HttpGet("GetExchangeRate")]
        public ActionResult<ReturnObject> GetExchangeRate([FromQuery] string networkName)
        {
            try
            {
                //  var addresses = _walletBusiness.GetAddresses(walletId, networkName);
                float rate = 1.0f / 7000000.0f;
                return new ReturnObject()
                {
                    Status = Status.STATUS_COMPLETED,
                    // Data = numberData.ToString(),
                    Message = rate.ToString()
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

            //  return null;
        }

        [HttpGet("CheckSendCoin")]
        public ActionResult<ReturnObject> CheckSendCoin([FromQuery] string fromAddress, [FromQuery] string toAddress,
            [FromQuery] string networkName, [FromQuery] string amount)
        {
            try
            {
                //  var addresses = _walletBusiness.GetAddresses(walletId, networkName);
                float vakapayfee = 0.01f;
                float minerfee = 0.01f;
                float total = vakapayfee + minerfee + float.Parse(amount);
                var feeObject = new {vakapayfee = vakapayfee, minerfee = minerfee, total = total};
                return new ReturnObject()
                {
                    Status = Status.STATUS_COMPLETED,
                    // Data = numberData.ToString(),
                    Message = JsonHelper.SerializeObject(feeObject)
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

            //  return null;
        }

        [HttpPost("History")]
        public ActionResult<ReturnObject> GetWalletHistory([FromBody] HistorySearch walletSearch)
        {
            try
            {
                //  var _history = _walletBusiness.GetHistory(walletSearch.wallet, 1, 3, new string[] { nameof(BlockchainTransaction.CreatedAt) });
                int numberData;
                var history = _walletBusiness.GetHistory(out numberData, walletSearch.UserId, walletSearch.NetworkName,
                    walletSearch.Offset, walletSearch.Limit, walletSearch.OrderBy, walletSearch.Search);
                return new ReturnObject()
                {
                    Status = Status.STATUS_COMPLETED,
                    Data = numberData.ToString(),
                    Message = JsonHelper.SerializeObject(history)
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

            //  return null;
        }

        [HttpPost("sendTransactions")]
        public ActionResult<string> SendTransactions( [FromBody] JObject value)
        {
            ReturnObject result = null;
            try
            {
                var request = value.ToObject<SendTransaction>();

                var userRequest = new UserSendTransaction()
                {
                    UserId = "8377a95b-79b4-4dfb-8e1e-b4833443c306",
                    Type = "send",
                    To = request.Detail.SendByAd ? request.Detail.RecipientWalletAddress : request.Detail.RecipientEmailAddress,
                    Amount = request.Detail.VkcAmount,
                    Currency = request.NetworkName,
                    Description = request.Detail.VkcNote,
                };

                result = AddSendTransaction(userRequest);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = new ReturnObject()
                { Status = Status.STATUS_ERROR, Message = e.Message };
            }
          //  result = new ReturnObject() { Status = Status.STATUS_ERROR, Message = "test" };
            return result.ToJson();
        }



        private ReturnObject AddSendTransaction(UserSendTransaction request)
        {
            var sendTransactionBusiness =
                new UserSendTransactionBusiness.UserSendTransactionBusiness(PersistenceFactory);
            ReturnObject result = null;
            try
            {
                var res = sendTransactionBusiness.AddSendTransaction(request);

                if (res.Status == Status.STATUS_ERROR)
                {
                    result = new ReturnObject()
                    { Status = Status.STATUS_ERROR, Message = res.Message };
                }
                else
                {
                    result = new ReturnDataObject()
                    { Status = Status.STATUS_SUCCESS, Data = request };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = new ReturnObject()
                { Status = Status.STATUS_ERROR, Message = e.Message };
            }
            finally
            {
                sendTransactionBusiness.CloseDbConnection();
            }

            return result;
        }


        


        public class HistorySearch
        {
            [DataMember(Name = "userID")] public string UserId;
            [DataMember(Name = "networkName")] public string NetworkName;
            [DataMember(Name = "offset")] public int Offset = -1;
            [DataMember(Name = "limit")] public int Limit = -1;
            [DataMember(Name = "orderBy")] public string[] OrderBy;
            [DataMember(Name = "search")] public string Search;
        }
    }
}