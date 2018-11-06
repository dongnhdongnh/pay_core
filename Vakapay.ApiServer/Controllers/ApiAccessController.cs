using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
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
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
                _logger.Error(KeyLogger.API_ACCESS_GET_INFO + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        private string ConvertSort(string sort)
        {
            if (string.IsNullOrEmpty(sort))
                return null;
            var key = sort;
            var desc = "";
            if (key[0].Equals('-'))
            {
                desc = key[0].ToString();
                key = sort.Remove(0, 1);
            }

            switch (key)
            {
                case "id":
                    return desc + "Id";
                case "userid":
                    return desc + "UserId";
                case "keyapi":
                    return desc + "KeyApi";
                case "permissions":
                    return desc + "Permissions";
                case "wallets":
                    return desc + "Wallets";
                case "status":
                    return desc + "Status";
                case "updatedat":
                    return desc + "UpdatedAt";

                default:
                    return null;
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

                StringValues sort = "-updatedat";
                StringValues filter;
                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_OFFSET, out var offset);
                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_LIMIT, out var limit);
                if (queryStringValue.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_FILTER))
                    queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_FILTER, out filter);
                if (queryStringValue.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_SORT))
                    queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_SORT, out sort);

                sort = ConvertSort(sort);


                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];
                int numberData;
                var dataApiKeys =
                    _userBusiness.GetApiKeys(out numberData, userModel.Id, Convert.ToInt32(offset),
                        Convert.ToInt32(limit), filter.ToString(), sort);

                if (dataApiKeys.Status != Status.STATUS_SUCCESS)
                    return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);
                var listApiKeys = JsonHelper.DeserializeObject<List<ResultApiAccess>>(dataApiKeys.Data);
                if (listApiKeys.Count <= 0)
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Data = JsonHelper.SerializeObject(listApiKeys)
                    }.ToJson();
                foreach (var listApiKey in listApiKeys)
                {
                    listApiKey.KeyApi = listApiKey.KeyApi.Substring(0, 10) + "...";
                }

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = new ResultList<ResultApiAccess>
                    {
                        List = listApiKeys,
                        Total = numberData
                    }.ToJson()
                }.ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.API_ACCESS_GET_LIST + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code and update when update verify
        [HttpPost("api-key/edit")]
        public string EditApiAccess([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey(ParseDataKeyApi.KEY_API_ACCESS_DATA))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                    code = value[ParseDataKeyApi.KEY_PASS_DATA_GET_CODE].ToString();


                var data = DataApiKeyForm.FromJson(value[ParseDataKeyApi.KEY_API_ACCESS_DATA].ToString());

                if (string.IsNullOrEmpty(data.Apis) || string.IsNullOrEmpty(data.Wallets) ||
                    string.IsNullOrEmpty(data.Id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                        break;
                    case 2:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);
                        if (string.IsNullOrEmpty(secretAuthToken.ApiAccessEdit))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessEdit, code, userModel, 120);
                        break;
                    case 0:
                        isVerify = true;
                        break;
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

                if (value.ContainsKey(ParseDataKeyApi.KEY_API_ACCESS_DATA_NOTIFY))
                {
                    modelApi.CallbackUrl = value[ParseDataKeyApi.KEY_API_ACCESS_DATA_NOTIFY].ToString();

                    if (!HelpersApi.CheckUrlValid(modelApi.CallbackUrl))
                        return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                }

                if (value.ContainsKey(ParseDataKeyApi.KEY_API_ACCESS_DATA_IP))
                {
                    modelApi.ApiAllow = value[ParseDataKeyApi.KEY_API_ACCESS_DATA_IP].ToString();
                }

                modelApi.Permissions = data.Apis;
                if (!HelpersApi.ValidatePermission(modelApi.Permissions))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                modelApi.Wallets = data.Wallets;
                if (!HelpersApi.ValidateWallet(modelApi.Wallets))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.API_ACCESS_EDIT,
                    HelpersApi.GetIp(Request));

                return _userBusiness.SaveApiKey(modelApi).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.API_ACCESS_EDIT + e);
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

                if (!value.ContainsKey(ParseDataKeyApi.KEY_API_ACCESS_DATA_APIS) ||
                    !value.ContainsKey(ParseDataKeyApi.KEY_API_ACCESS_DATA_WALLETS))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);


                //check verify
                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                    code = value[ParseDataKeyApi.KEY_PASS_DATA_GET_CODE].ToString();

                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                        break;
                    case 2:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);
                        if (string.IsNullOrEmpty(secretAuthToken.ApiAccessAdd))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessAdd, code, userModel, 120);
                        break;
                    case 0:
                        isVerify = true;
                        break;
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                //update api permissions
                var modelApi = new ApiKey {UserId = userModel.Id};


                //validate data
                if (value.ContainsKey(ParseDataKeyApi.KEY_API_ACCESS_DATA_NOTIFY))
                {
                    modelApi.CallbackUrl = value[ParseDataKeyApi.KEY_API_ACCESS_DATA_NOTIFY].ToString();

                    if (!HelpersApi.CheckUrlValid(modelApi.CallbackUrl))
                        return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                }

                if (value.ContainsKey(ParseDataKeyApi.KEY_API_ACCESS_DATA_IP))
                {
                    modelApi.ApiAllow = value[ParseDataKeyApi.KEY_API_ACCESS_DATA_IP].ToString();
                }

                modelApi.Permissions = value[ParseDataKeyApi.KEY_API_ACCESS_DATA_APIS].ToString();
                if (!HelpersApi.ValidatePermission(modelApi.Permissions))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                modelApi.Wallets = value[ParseDataKeyApi.KEY_API_ACCESS_DATA_WALLETS].ToString();
                if (!HelpersApi.ValidateWallet(modelApi.Wallets))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                modelApi.Status = 1;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.API_ACCESS_ADD,
                    HelpersApi.GetIp(Request));

                return _userBusiness.SaveApiKey(modelApi).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.API_ACCESS_ADD + e);
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

                if (!value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                    code = value[ParseDataKeyApi.KEY_PASS_DATA_GET_CODE].ToString();


                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                        break;
                    case 2:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);
                        if (string.IsNullOrEmpty(secretAuthToken.ApiAccessDelete))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessDelete, code, userModel);
                        break;
                    case 0:
                        isVerify = true;
                        break;
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
                _logger.Error(KeyLogger.API_ACCESS_DELETE + e);
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

                if (!value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                    code = value[ParseDataKeyApi.KEY_PASS_DATA_GET_CODE].ToString();


                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                        break;
                    case 2:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);
                        if (string.IsNullOrEmpty(secretAuthToken.ApiAccessStatus))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessStatus, code, userModel);
                        break;
                    case 0:
                        isVerify = true;
                        break;
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
                _logger.Error(KeyLogger.API_ACCESS_DISABLE + e);
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

                if (!value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                    code = value[ParseDataKeyApi.KEY_PASS_DATA_GET_CODE].ToString();


                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                        break;
                    case 2:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                        if (string.IsNullOrEmpty(secretAuthToken.ApiAccessStatus))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessStatus, code, userModel);
                        break;
                    case 0:
                        isVerify = true;
                        break;
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
                _logger.Error(KeyLogger.API_ACCESS_ENABLE + e);
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

                if (!value.ContainsKey("id"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                    code = value[ParseDataKeyApi.KEY_PASS_DATA_GET_CODE].ToString();


                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                        break;
                    case 2:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                        if (string.IsNullOrEmpty(secretAuthToken.ApiAccessEdit))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.ApiAccessEdit, code, userModel, 120);
                        break;
                    case 0:
                        isVerify = true;
                        break;
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
                _logger.Error(KeyLogger.API_ACCESS_DETAIL + e);
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

                if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value[ParseDataKeyApi.KEY_PASS_DATA_GET_CODE].ToString();

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
                _logger.Error(KeyLogger.API_ACCESS_VERIFY + e);
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

                if (!value.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_CODE))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                if (userModel.IsTwoFactor != 1 || string.IsNullOrEmpty(userModel.TwoFactorSecret))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value[ParseDataKeyApi.KEY_PASS_DATA_GET_CODE].ToString();
                if (!HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS
                }.ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.API_ACCESS_VERIFY + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }
    }
}