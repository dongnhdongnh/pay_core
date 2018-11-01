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
    public class SecurityController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }


        public SecurityController(
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

        [HttpGet("get-info")]
        public string GetInfo()
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(new SecurityModel
                    {
                        TwofaOption = userModel.Verification,
                        IsEnableTwofa = userModel.IsTwoFactor
                    })
                }.ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.SECURITY_GET_INFO + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("lock-screen/update")]
        public string UpdateCloseAccount([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (
                    !value.ContainsKey(ParseDataKeyApi.KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_STATUS) ||
                    !value.ContainsKey(ParseDataKeyApi.KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_PASSWORD))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 1);

                var code = "";
                if (value.ContainsKey(ParseDataKeyApi.KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_CODE))
                    code = value[ParseDataKeyApi.KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_CODE].ToString();

                var status = value[ParseDataKeyApi.KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_STATUS];

                if (!int.TryParse((string) status, out var outStatus))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 2);

                var password = value[ParseDataKeyApi.KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_PASSWORD].ToString();

                if (!HelpersApi.ValidateSecondPass(password))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 3);

                bool isVerify = false;

                switch (userModel.IsTwoFactor)
                {
                    case 1:
                        if (!value.ContainsKey("code"))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 4);

                        isVerify = HelpersApi.CheckCodeGoogle(userModel.TwoFactorSecret, code);
                        break;
                    case 2:
                        if (!value.ContainsKey("code"))
                            return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID + 5);

                        var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);
                        if (string.IsNullOrEmpty(secretAuthToken.LockScreen))
                            return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                        isVerify = HelpersApi.CheckCodeSms(secretAuthToken.LockScreen, code, userModel, 120);
                        break;
                    case 0:
                        isVerify = true;
                        break;
                }

                if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                userModel.IsLockScreen = outStatus;
                userModel.SecondPassword = !string.IsNullOrEmpty(password)
                    ? CommonHelper.Md5(password)
                    : "";

                _userBusiness.UpdateProfile(userModel);

                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.LOCK_SCREEN,
                    HelpersApi.GetIp(Request)).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.SECURITY_LOCK_SCREEN_UPDATE + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        [HttpPost("lock-screen/unlock")]
        public string VerifyPassword([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (userModel.IsLockScreen == 0)
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                    }.ToJson();


                if (!value.ContainsKey(ParseDataKeyApi.KEY_SECURITY_VERIFY_PASSWORD))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
                var password = value[ParseDataKeyApi.KEY_SECURITY_VERIFY_PASSWORD].ToString();

                if (!HelpersApi.ValidateSecondPass(password))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                if (CommonHelper.Md5(password).Equals(userModel.SecondPassword))
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                    }.ToJson();


                return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.SECURITY_LOCK_SCREEN_UNLOCK + e);
                return HelpersApi.CreateDataError(e.Message);
            }
        }
    }
}