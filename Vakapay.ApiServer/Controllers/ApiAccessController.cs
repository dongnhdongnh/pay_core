using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vakapay.ApiServer.ActionFilter;
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [EnableCors]
    [ApiController]
    [Authorize]
    [BaseActionFilter]
    public class ApiAccessController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private readonly WalletBusiness.WalletBusiness _walletBusiness;
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }


        public ApiAccessController(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        )
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            _userBusiness = new UserBusiness.UserBusiness(PersistenceFactory);
            _walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
        }

        [HttpGet("api-access/get-info")]
        public string GetInfo()
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (userModel != null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonConvert.SerializeObject(new InfoApi())
                    }.ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.DataNotFound);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        [HttpGet("api-access/get-list-api-access")]
        public string GetListApiAccess()
        {
            try
            {
                var queryStringValue = Request.Query;


                if (!queryStringValue.ContainsKey("offset") || !queryStringValue.ContainsKey("limit"))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                queryStringValue.TryGetValue("offset", out var offset);
                queryStringValue.TryGetValue("limit", out var limit);

                var userModel = (User) RouteData.Values["UserModel"];

                var dataApiKeys =
                    _userBusiness.GetApiKeys(userModel.Id, Convert.ToInt32(offset), Convert.ToInt32(limit));

                if (dataApiKeys.Status == Status.STATUS_SUCCESS)
                {
                    Console.WriteLine(dataApiKeys.Data);
                    var listApiKeys = JsonHelper.DeserializeObject<List<ResultApiAccess>>(dataApiKeys.Data);
                    if (listApiKeys.Count > 0)
                    {
                        foreach (var listApiKey in listApiKeys)
                        {
                            listApiKey.KeyApi = listApiKey.KeyApi.Substring(0, 10) + "...";
                        }
                    }

                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonHelper.SerializeObject(listApiKeys)
                    }.ToJson();
                }


                return HelpersApi.CreateDataError(MessageApiError.DataNotFound);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        // verify code and update when update verify
        [HttpPost("api-access/create")]
        public string AddApiAccess([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("code") || !value.ContainsKey("apis") || !value.ContainsKey("wallets"))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                    var secret = secretAuthToken.ApiAccess;

                    isVerify = HelpersApi.CheckCodeSms(secret, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                //update api permissions
                var modelApi = new ApiKey();

                modelApi.UserId = userModel.Id;

                if (value.ContainsKey("notificationUrl"))
                {
                    modelApi.CallbackUrl = value["notificationUrl"].ToString();

                    if (!HelpersApi.CheckUrlValid(modelApi.CallbackUrl))
                        HelpersApi.CreateDataError(MessageApiError.ParamInvalid);
                }

                if (value.ContainsKey("allowedIp"))
                {
                    modelApi.ApiAllow = value["allowedIp"].ToString();
                }

                modelApi.Permissions = value["apis"].ToString();
                modelApi.Wallets = value["wallets"].ToString();
                modelApi.Status = 1;


                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.API_ACCESS,
                    HelpersApi.GetIp(Request));

                return _userBusiness.SaveApiKey(modelApi).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // delete api key
        [HttpPost("api-key/delete")]
        public string DeleteApiAccess([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("code") || !value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                    var secret = secretAuthToken.ApiAccess;

                    isVerify = HelpersApi.CheckCodeSms(secret, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                var id = value["id"].ToString();
                if (!HelpersApi.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.API_ACCESS_DELETE,
                    HelpersApi.GetIp(Request));

                return _userBusiness.DeleteApikeyById(id).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // disable api key
        [HttpPost("api-key/disable")]
        public string DisableApiAccess([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("code") || !value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                    var secret = secretAuthToken.ApiAccess;

                    isVerify = HelpersApi.CheckCodeSms(secret, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                var id = value["id"].ToString();
                if (!HelpersApi.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                var apiKey = _userBusiness.GetApiKeyById(id);

                if (apiKey != null)
                {
                    apiKey.Status = 0;
                    return _userBusiness.SaveApiKey(apiKey).ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        // enable api key
        [HttpPost("api-key/enable")]
        public string EnableApiAccess([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("code") || !value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                    var secret = secretAuthToken.ApiAccess;

                    isVerify = HelpersApi.CheckCodeSms(secret, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                var id = value["id"].ToString();
                if (!HelpersApi.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                var apiKey = _userBusiness.GetApiKeyById(id);

                if (apiKey != null)
                {
                    apiKey.Status = 1;
                    return _userBusiness.SaveApiKey(apiKey).ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        // delete api key
        [HttpPost("api-key/get")]
        public string GetApiAccess([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("code") || !value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                    var secret = secretAuthToken.ApiAccess;

                    isVerify = HelpersApi.CheckCodeSms(secret, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                var id = value["id"].ToString();
                if (!HelpersApi.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                return _userBusiness.GetApiKeyById(id).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        //verify code when add api access
        [HttpPost("api-access/verify-code-sms")]
        public string VerifyCodeAdd([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                    return HelpersApi.CreateDataError(MessageApiError.UserNotFound);

                if (value.ContainsKey("code"))
                {
                    var code = value["code"].ToString();

                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                    var secret = secretAuthToken.ApiAccess;


                    var isok = authenticator.CheckCode(secret, code, userModel);

                    if (!isok) return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Message = "Verify access"
                    }.ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        // then code when add api access
        [HttpPost("api-access/require-send-code-phone")]
        public string SendCodeAdd()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel != null)
                {
                    var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.API_ACCESS);

                    if (checkSecret == null)
                        return HelpersApi.CreateDataError(MessageApiError.SmsError);

                    userModel.SecretAuthToken = checkSecret;
                    var resultUpdate = _userBusiness.UpdateProfile(userModel);

                    if (resultUpdate.Status == Status.STATUS_ERROR)
                        return resultUpdate.ToJson();

                    var secretAuthToken = ActionCode.FromJson(checkSecret);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError(MessageApiError.SmsError);

                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator(null, null, 120);
                    var code = authenticator.GetCode(secretAuthToken.ApiAccess);

                    return _userBusiness.SendSms(userModel, code)
                        .ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        /**
         *  verify code when twofa add api access
         */
        [HttpPost("api-access/verify-code-twofa")]
        public string VerifyCodeTwofa([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null) return HelpersApi.CreateDataError(MessageApiError.UserNotFound);

                if (!value.ContainsKey("code")) return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                if (!userModel.TwoFactor || string.IsNullOrEmpty(userModel.TwoFactorSecret))
                    return HelpersApi.CreateDataError(MessageApiError.ParamInvalid);

                var code = value["code"].ToString();
                if (!HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code))
                    return HelpersApi.CreateDataError(MessageApiError.SmsVerifyError);

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS
                }.ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }
    }
}