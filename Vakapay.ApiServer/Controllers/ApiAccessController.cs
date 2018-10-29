using System;
using System.Collections.Generic;
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
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }


        public ApiAccessController(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        )
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            _userBusiness = new UserBusiness.UserBusiness(PersistenceFactory);
        }

        [HttpGet("api-access/get-info")]
        public string GetInfo()
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (userModel != null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonConvert.SerializeObject(new InfoApi())
                    }.ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);
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
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                queryStringValue.TryGetValue("offset", out var offset);
                queryStringValue.TryGetValue("limit", out var limit);

                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                var dataApiKeys =
                    _userBusiness.GetApiKeys(userModel.Id, Convert.ToInt32(offset), Convert.ToInt32(limit));

                if (dataApiKeys.Status != Status.STATUS_SUCCESS)
                    return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);
                var listApiKeys = JsonHelper.DeserializeObject<List<ResultApiAccess>>(dataApiKeys.Data);
                if (listApiKeys.Count <= 0)
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonHelper.SerializeObject(listApiKeys)
                    }.ToJson();
                foreach (var listApiKey in listApiKeys)
                {
                    listApiKey.KeyApi = listApiKey.KeyApi.Substring(0, 10) + "...";
                }

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(listApiKeys)
                }.ToJson();


            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code and update when update verify
        [HttpPost("api-access/edit")]
        public string EditApiAccess([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey("code") || !value.ContainsKey("data"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["code"].ToString();
                var data = DataApiKeyForm.FromJson(value["data"].ToString());

                if (string.IsNullOrEmpty(data.Apis) || string.IsNullOrEmpty(data.Wallets) ||
                    string.IsNullOrEmpty(data.Id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 4);

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccessAdd))
                        return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                    isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessAdd, code, userModel, 120);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                //update api permissions
                if (!CommonHelper.ValidateId(data.Id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var modelApi = _userBusiness.GetApiKeyById(data.Id);
                if (modelApi == null)
                    return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);


                if (!modelApi.UserId.Equals(userModel.Id))
                    return HelpersApi.CreateDataError(MessageApiError.USER_NOT_EXIT);

                if (value.ContainsKey("notificationUrl"))
                {
                    modelApi.CallbackUrl = value["notificationUrl"].ToString();

                    if (!HelpersApi.CheckUrlValid(modelApi.CallbackUrl))
                        return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 1);
                }

                if (value.ContainsKey("allowedIp"))
                {
                    modelApi.ApiAllow = value["allowedIp"].ToString();
                }

                modelApi.Permissions = data.Apis;
                if (!HelpersApi.ValidatePermission(modelApi.Permissions))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 2);

                modelApi.Wallets = data.Wallets;
                if (!HelpersApi.ValidateWallet(modelApi.Wallets))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 3);

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
        // verify code and update when update verify
        [HttpPost("api-access/create")]
        public string AddApiAccess([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey("code") || !value.ContainsKey("apis") || !value.ContainsKey("wallets"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccessAdd))
                        return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                    isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessAdd, code, userModel, 120);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                //update api permissions
                var modelApi = new ApiKey {UserId = userModel.Id};


                if (value.ContainsKey("notificationUrl"))
                {
                    modelApi.CallbackUrl = value["notificationUrl"].ToString();

                    if (!HelpersApi.CheckUrlValid(modelApi.CallbackUrl))
                        return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                }

                if (value.ContainsKey("allowedIp"))
                {
                    modelApi.ApiAllow = value["allowedIp"].ToString();
                }

                modelApi.Permissions = value["apis"].ToString();
                if (!HelpersApi.ValidatePermission(modelApi.Permissions))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                modelApi.Wallets = value["wallets"].ToString();
                if (!HelpersApi.ValidateWallet(modelApi.Wallets))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

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
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey("code") || !value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccessDelete))
                        return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                    isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessDelete, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                var id = value["id"].ToString();
                if (!CommonHelper.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

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
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey("code") || !value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

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
                        return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                    isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccess, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                var id = value["id"].ToString();
                if (!CommonHelper.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var apiKey = _userBusiness.GetApiKeyById(id);

                if (apiKey == null) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                apiKey.Status = 0;
                return _userBusiness.SaveApiKey(apiKey).ToJson();

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
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey("code") || !value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

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
                        return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                    isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccess, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                var id = value["id"].ToString();
                if (!CommonHelper.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var apiKey = _userBusiness.GetApiKeyById(id);

                if (apiKey != null)
                {
                    apiKey.Status = 1;
                    return _userBusiness.SaveApiKey(apiKey).ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
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
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey("code") || !value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccessEdit))
                        return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);


                    isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessEdit, code, userModel, 120);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                var id = value["id"].ToString();
                if (!CommonHelper.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var dataApi = _userBusiness.GetApiKeyById(id);

                if (dataApi != null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonHelper.SerializeObject(dataApi)
                    }.ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);
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
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey("code")) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                var code = value["code"].ToString();

                var authenticator = new TwoStepsAuthenticator.TimeAuthenticator(null, null, 120);

                var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);


                if (string.IsNullOrEmpty(secretAuthToken.ApiAccessAdd))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                var isOk = authenticator.CheckCode(secretAuthToken.ApiAccessAdd, code, userModel);

                if (!isOk) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Message = "Verify access"
                }.ToJson();

            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        /**
         *  verify code when two Fa add api access
         */
        [HttpPost("verify-code-twofa")]
        public string VerifyCodeTwoFa([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey("code")) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                if (!userModel.TwoFactor || string.IsNullOrEmpty(userModel.TwoFactorSecret))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["code"].ToString();
                if (!HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

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