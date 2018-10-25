using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class SecurityController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }


        public SecurityController(
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
        }

        [HttpGet("get-info")]
        public string GetInfo()
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(new SecurityModel
                    {
                        twofaOption = userModel.Verification,
                        isEnableTwofa = userModel.TwoFactor
                    })
                }.ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("lock-screen/update")]
        public string UpdateCloseAccount([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (value.ContainsKey("code") && value.ContainsKey("status") && value.ContainsKey("password"))
                {
                    var code = value["code"].ToString();
                    var status = value["status"];

                    if (!int.TryParse((string) status, out int outStatus))
                        return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                    var password = value["password"];
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

                        var secret = secretAuthToken.ApiAccess;

                        isVerify = HelpersApi.CheckCodeSms(secret, code, userModel);
                    }

                    if (!isVerify) return HelpersApi.CreateDataError(MessageApiError.SMS_VERIFY_ERROR);

                    userModel.IsLockScreen = outStatus;
                    userModel.SecondPassword = !string.IsNullOrEmpty(password.ToString())
                        ? CommonHelper.Md5(password.ToString())
                        : "";

                    _userBusiness.UpdateProfile(userModel);

                    return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                        ActionLog.LOCK_SCREEN,
                        HelpersApi.GetIp(Request)).ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        [HttpPost("lock-screen/unlock")]
        public string VerifyPassword([FromBody] JObject value)
        {
            try
            {
                var userModel = (User) RouteData.Values["UserModel"];

                if (userModel.IsLockScreen == 0)
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                    }.ToJson();


                if (value.ContainsKey("password"))
                {
                    var password = value["password"].ToString();

                    if (CommonHelper.Md5(password).Equals(userModel.SecondPassword))
                        return new ReturnObject
                        {
                            Status = Status.STATUS_SUCCESS,
                        }.ToJson();
                }


                return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }
    }
}