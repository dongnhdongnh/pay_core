using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.Models.Domains;
using Vakapay.Commons.Constants;
using Newtonsoft.Json.Linq;
using Vakapay.ApiServer.ActionFilter;
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Models.Entities;
using Vakapay.Models.ClientRequest;
using System.IO;
using CsvHelper;

namespace Vakapay.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : CustomController
    {
        readonly WalletBusiness.WalletBusiness _walletBusiness;
        readonly UserBusiness.UserBusiness _userBusiness;
        readonly SendMailBusiness.SendMailBusiness _sendMailBusiness;
        public WalletController(
            IVakapayRepositoryFactory persistenceFactory,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        ) : base(persistenceFactory, configuration, hostingEnvironment)
        {
            _userBusiness = new UserBusiness.UserBusiness(persistenceFactory);
            _walletBusiness = new Vakapay.WalletBusiness.WalletBusiness(persistenceFactory);
            _sendMailBusiness = new Vakapay.SendMailBusiness.SendMailBusiness(persistenceFactory);
        }


        [HttpGet("test")]
        public ActionResult<string> GetTest()
        {
            return "hello";
            //  return null;
        }


        // POST api/values
        // verify code and update when update verify
        [HttpPost("transaction/verify-code")]
        public string VerifyCodeTransaction([FromBody] JObject value)
        {
            try
            {
                var userModel = (User)RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];


                var code = "";
                var codeGG = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS))
                    code = value[ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS].ToString();
                if (value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_2FA))
                    codeGG = value[ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_2FA].ToString();

                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, codeGG);
                        break;
                    case 2:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);
                        if (string.IsNullOrEmpty(secretAuthToken.SendTransaction))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.SendTransaction, code, userModel);
                        break;
                    case 0:
                        isVerify = true;
                        break;
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                // userModel.Verification = (int) option;

                // su ly data gui len
                //to do

                var request = value.ToObject<SendTransaction>();

                var userRequest = new UserSendTransaction()
                {
                    UserId = userModel.Id,
                    Type = "send",
                    To = request.Detail.SendByAd
                        ? request.Detail.RecipientWalletAddress
                        : request.Detail.RecipientEmailAddress,
                    SendByBlockchainAddress = request.Detail.SendByAd,
                    Amount = request.Detail.VkcAmount,
                    PricePerCoin = request.Detail.PricePerCoin,
                    Currency = request.NetworkName,
                    Description = request.Detail.VkcNote,
                };
                ReturnObject result = null;
                result = AddSendTransaction(userRequest);

                return JsonHelper.SerializeObject(result);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        [HttpGet("all")]
        public ActionResult<ReturnObject> GetWalletsByUser()
        {
            try
            {
                var userModel = (User)RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];
                var wallets = _walletBusiness.LoadAllWalletByUser(userModel);
                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS,
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
        public ActionResult<string> GetWalletInfor([FromQuery] string networkName)
        {
            var userModel = (User)RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];
            var wallet = _walletBusiness.FindByUserAndNetwork(userModel.Id, networkName);
            return JsonHelper.SerializeObject(wallet);
            //  return null;
        }

        [HttpGet("AddressInfor")]
        public ActionResult<ReturnObject> GetAddresses([FromQuery] string networkName)
        {
            try
            {
                var userModel = (User)RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];
                var wallet = _walletBusiness.FindByUserAndNetwork(userModel.Id, networkName);

                if (wallet == null)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't not wallet"
                    };
                }

                var addresses = _walletBusiness.GetAddresses(wallet.Id, networkName);
                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS,
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

        [HttpGet("CheckUserMail")]
        public ActionResult<ReturnObject> CheckUserMail([FromQuery] string userMail)
        {
            try
            {
                var _userData = _userBusiness.GetUserByEmail(userMail);
                if (_userData == null)
                    throw new Exception("Not found user data" + userMail);
                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS,
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
                //CacheHelper.GetCacheString(String.Format(
                //RedisCacheKey.COINMARKET_PRICE_CACHEKEY, DashboardConfig.VAKACOIN,
                //DashboardConfig.CURRENT));
                //  var addresses = _walletBusiness.GetAddresses(walletId, networkName);
                float rate = 1.0f / 7000000.0f;
                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS,
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
        public ActionResult<ReturnObject> CheckSendCoin([FromQuery] string toAddress,
            [FromQuery] string networkName, [FromQuery] string amount)
        {
            try
            {
                //  var addresses = _walletBusiness.GetAddresses(walletId, networkName);
                float vakapayfee = 0.01f;
                float minerfee = 0.01f;
                float total = vakapayfee + minerfee + float.Parse(amount);
                var feeObject = new { vakapayfee = vakapayfee, minerfee = minerfee, total = total };
                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS,
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
                var userModel = (User)RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                int numberData;
                var history = _walletBusiness.GetHistory(out numberData, userModel.Id, walletSearch.NetworkName,
                    walletSearch.Offset, walletSearch.Limit, walletSearch.OrderBy, walletSearch.Search);
                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS,
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

        [HttpPost("Report")]
        public ActionResult<ReturnObject> GetWalletReport([FromBody] HistoryReport walletReport)
        {
            try
            {
                //  var _history = _walletBusiness.GetHistory(walletSearch.wallet, 1, 3, new string[] { nameof(BlockchainTransaction.CreatedAt) });
                var userModel = (User)RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];
                // CommonHelper.GetUnixTimestamp
                DateTime dateForButton = DateTime.Now.AddDays(-1 * walletReport.DayTime);
                var _searchTime = UnixTimestamp.ToUnixTimestamp(dateForButton);
                int numberData;
                var history = _walletBusiness.GetHistory(out numberData, userModel.Id, walletReport.NetworkName, -1, -1, null, null, _searchTime);
                if (walletReport.Email != null)
                {
                    string fileName = userModel.Id + UnixTimestamp.ToUnixTimestamp(DateTime.Now) + ".csv";
                    using (StreamWriter sw = new StreamWriter(System.IO.File.Open(AppSettingHelper.GetReportStoreUrl() + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        var csv = new CsvWriter(sw);
                        csv.WriteRecords(history);
                    }

                    EmailQueue email = new EmailQueue()
                    {
                        ToEmail = walletReport.Email,
                        NetworkName = walletReport.NetworkName,
                        // Amount = addedBalance,
                        Template = EmailTemplate.Report,
                        Subject = EmailConfig.SUBJECT_REPORT,
                        SendFileList = fileName
                        //							Content = networkName + "+" + addedBlance
                    };
                    _sendMailBusiness.CreateEmailQueueAsync(email);
                }
                //  Console.WriteLine("csv " + csv);
                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(history),
                    Message = numberData.ToString()
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


        public ReturnObject AddSendTransaction(UserSendTransaction request)
        {
            var sendTransactionBusiness =
                new UserSendTransactionBusiness.UserSendTransactionBusiness(_repositoryFactory);
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
        public class HistoryReport
        {
            [DataMember(Name = "networkName")] public string NetworkName;
            [DataMember(Name = "dayTime")] public int DayTime = 1;
            [DataMember(Name = "email")] public string Email;

        }
    }
}