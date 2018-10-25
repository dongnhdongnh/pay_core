using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TwofaController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }


        private IConfiguration Configuration { get; }

        public TwofaController(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        )
        {
            Configuration = configuration;

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
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
                var userModel = (User) RouteData.Values["UserModel"];
                
                if (!value.ContainsKey("code")) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                if (!value.ContainsKey("option")) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["code"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.UpdateOptionVerification))
                        return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                    var secret = secretAuthToken.UpdateOptionVerification;

                    isVerify = HelpersApi.CheckCodeSms(secret, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                var option = value["option"];

                userModel.Verification = (int) option;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_NOTIFICATION,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        [HttpPost("enable/update")]
        public string VerifyCodeEnableGoogle([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("token")) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var token = value["token"].ToString();
                if (!HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, token))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                userModel.TwoFactor = true;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.TWOFA_ENABLE,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code when enable twofa
        [HttpPost("enable/verify-code-sms")]
        public string VerifyCodeEnable([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("code")) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["code"].ToString();
                var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                if (string.IsNullOrEmpty(secretAuthToken.TwofaEnable))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                var secret = secretAuthToken.TwofaEnable;

                var isok = authenticator.CheckCode(secret, code, userModel);

                if (!isok) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

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
                return HelpersApi.CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code when disable twofa
        [HttpPost("disable/update")]
        public string VerifyCodeDisable([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("code")) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["code"].ToString();
                if (!HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                userModel.TwoFactor = false;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.TWOFA_DISABLE,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
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
                var userModel = (User) RouteData.Values["UserModel"];

                if (!value.ContainsKey("SMScode")) return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var code = value["SMScode"].ToString();

                bool isVerify;

                if (userModel.TwoFactor && !string.IsNullOrEmpty(userModel.TwoFactorSecret))
                {
                    isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                }
                else
                {
                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.SendTransaction))
                        return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                    var secret = secretAuthToken.SendTransaction;

                    isVerify = HelpersApi.CheckCodeSms(secret, code, userModel);
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                // userModel.Verification = (int) option;

                // su ly data gui len
                //to do

                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_NOTIFICATION,
                    HelpersApi.GetIp(Request)).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        /**
        *  send code when update verify
        */
        [HttpPost("transaction/require-send-code-phone")]
        public string SendCodeTransaction()
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.SEND_TRSANSACTION);

                if (checkSecret == null)
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                userModel.SecretAuthToken = checkSecret;
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.STATUS_ERROR)
                    return resultUpdate.ToJson();

                var secretAuthToken = ActionCode.FromJson(checkSecret);

                if (string.IsNullOrEmpty(secretAuthToken.SendTransaction))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                return _userBusiness.SendSms(userModel, HelpersApi.SendCodeSms(secretAuthToken.SendTransaction))
                    .ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        /**
         *  send code when update verify
         */
        [HttpPost("option/require-send-code-phone")]
        public string SendCodeOption()
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.UPDATE_NOTIFICATION);

                if (checkSecret == null)
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                userModel.SecretAuthToken = checkSecret;
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.STATUS_ERROR)
                    return resultUpdate.ToJson();

                var secretAuthToken = ActionCode.FromJson(checkSecret);

                if (string.IsNullOrEmpty(secretAuthToken.UpdateOptionVerification))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                return _userBusiness
                    .SendSms(userModel, HelpersApi.SendCodeSms(secretAuthToken.UpdateOptionVerification)).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        /**
         *  send code when disable two
         */
        [HttpPost("disable/require-send-code-phone")]
        public string SendCodeDisable()
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                var google = new GoogleAuthen.TwoFactorAuthenticator();

                var startSetup = google.GenerateSetupCode(userModel.Email, userModel.TwoFactorSecret, 300, 300);

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = startSetup.ManualEntryKey
                }.ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("enable/require-send-code-phone")]
        public string SendCodeEnable()
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.TWOFA_ENABLE);

                if (checkSecret == null)
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                userModel.SecretAuthToken = checkSecret;
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.STATUS_ERROR)
                    return resultUpdate.ToJson();

                var secretAuthToken = ActionCode.FromJson(checkSecret);

                if (string.IsNullOrEmpty(secretAuthToken.TwofaEnable))
                    return HelpersApi.CreateDataError(MessageApiError.SMS_ERROR);

                return _userBusiness.SendSms(userModel, HelpersApi.SendCodeSms(secretAuthToken.TwofaEnable)).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }
    }
}