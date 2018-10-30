using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    [BaseActionFilter]
    public class TwoFaController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }


        public TwoFaController(
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

        // POST api/values
        // verify code and update when update verify
        [HttpPost("option/update")]
        public string UpdateOption([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_UPDATE_OPTION))

                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_UPDATE_OPTION_CODE))
                    code = value[ParseDataKeyApi.KEY_TWO_FA_UPDATE_OPTION_CODE].ToString();


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
                        if (string.IsNullOrEmpty(secretAuthToken.UpdateOptionVerification))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.UpdateOptionVerification, code, userModel,
                            120);
                        break;
                    case 0:
                        isVerify = true;
                        break;
                }


                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                var option = value[ParseDataKeyApi.KEY_TWO_FA_UPDATE_OPTION];

                userModel.Verification = (int) option;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_OPTION_VETIFY,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.TWOFA_OPTION_UPDATE + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        [HttpPost("enable/update")]
        public string VerifyCodeEnableGoogle([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_ENABLE_GOOGLE_TOKEN))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var token = value[ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_ENABLE_GOOGLE_TOKEN].ToString();
                if (!HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, token))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                userModel.IsTwoFactor = 1;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.TWOFA_ENABLE,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.TWOFA_ENABLE_UPDATE + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code when enable twoFa
        [HttpPost("enable/verify-code-sms")]
        public string VerifyCodeEnable([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (userModel.IsTwoFactor == 2)
                {
                    if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_ENABLE_CODE))
                        return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                    var code = value[ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_ENABLE_CODE].ToString();
                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.TwofaEnable))
                        return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                    var isOk = authenticator.CheckCode(secretAuthToken.TwofaEnable, code, userModel);

                    if (!isOk) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);
                }


                var google = new GoogleAuthen.TwoFactorAuthenticator();

                var secretKey = CommonHelper.RandomString(32);

                var startSetup = google.GenerateSetupCode(userModel.Email, secretKey, 300, 300);

                userModel.TwoFactorSecret = secretKey;

                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.STATUS_ERROR)
                    return resultUpdate.ToJson();

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = startSetup.ManualEntryKey
                }.ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.TWOFA_ENABLE_VERIFY + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code when disable two Fa
        [HttpPost("disable/update")]
        public string VerifyCodeDisable([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_DISABLE_CODE))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value[ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_DISABLE_CODE].ToString();
                if (!HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                userModel.IsTwoFactor = 0;
                userModel.SecretAuthToken = null;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.TWOFA_DISABLE,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.TWOFA_DISABLE_VERIFY + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code and update when update verify
        [HttpPost("transaction/verify-code")]
        public string VerifyCodeTransaction([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];


                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS))
                    code = value[ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS].ToString();


                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                        break;
                    case 2:
                        if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);
                        if (string.IsNullOrEmpty(secretAuthToken.SendTransaction))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.SendTransaction, code, userModel, 120);
                        break;
                    case 0:
                        isVerify = true;
                        break;
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                // userModel.Verification = (int) option;

                // su ly data gui len
                //to do

                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.SEND_TRANSACTION,
                    HelpersApi.GetIp(Request)).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.TWOFA_SEND_TRANSACTION_VERIFY + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        [HttpPost("require-send-code-phone")]
        public string SendCode([FromBody] JObject value)
        {
            try
            {
                if (!value.ContainsKey(ParseDataKeyApi.KEY_TWO_FA_SEND_CODE_ACTION))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var action = value[ParseDataKeyApi.KEY_TWO_FA_SEND_CODE_ACTION].ToString();
                string secret;
                var time = 30;

                switch (action)
                {
                    case ActionLog.TWOFA_ENABLE:
                        secret = ActionLog.TWOFA_ENABLE;
                        break;
                    case ActionLog.AVATAR:
                        secret = ActionLog.AVATAR;
                        break;
                    case ActionLog.UPDATE_PREFERENCES:
                        secret = ActionLog.UPDATE_PREFERENCES;
                        break;
                    case ActionLog.UPDATE_OPTION_VETIFY:
                        secret = ActionLog.UPDATE_OPTION_VETIFY;
                        break;
                    case ActionLog.UPDATE_PROFILE:
                        secret = ActionLog.UPDATE_PROFILE;
                        break;
                    case ActionLog.TWOFA_DISABLE:
                        secret = ActionLog.TWOFA_DISABLE;
                        break;
                    case ActionLog.LOCK_SCREEN:
                        secret = ActionLog.LOCK_SCREEN;
                        break;
                    case ActionLog.SEND_TRANSACTION:
                        secret = ActionLog.SEND_TRANSACTION;
                        break;
                    case ActionLog.API_ACCESS_ADD:
                        secret = ActionLog.API_ACCESS_ADD;
                        time = 120;
                        break;
                    case ActionLog.API_ACCESS_EDIT:
                        secret = ActionLog.API_ACCESS_EDIT;
                        time = 120;
                        break;
                    case ActionLog.API_ACCESS:
                        secret = ActionLog.API_ACCESS;
                        break;
                    case ActionLog.API_ACCESS_DELETE:
                        secret = ActionLog.API_ACCESS_DELETE;
                        break;
                    default:
                        return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                }

                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                var checkSecret = HelpersApi.CheckToken(userModel, secret);

                if (checkSecret == null)
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                if (checkSecret.NewSecret == null)
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                if (checkSecret.Secret == null)
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                userModel.SecretAuthToken = checkSecret.NewSecret;
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                return resultUpdate.Status == Status.STATUS_ERROR
                    ? resultUpdate.ToJson()
                    : _userBusiness.SendSms(userModel, HelpersApi.SendCodeSms(checkSecret.Secret, time)).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.TWOFA_REQUIRED_SEND_CODE + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }
    }
}