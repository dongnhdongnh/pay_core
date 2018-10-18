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
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
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
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};


                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                {
                    //return error
                    return CreateDataError("User not exist in DB");
                }


                if (!value.ContainsKey("code")) return CreateDataError("Can't update options");

                if (!value.ContainsKey("option")) return CreateDataError("Can't update options");

                var code = value["code"].ToString();
                var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                if (string.IsNullOrEmpty(secretAuthToken.UpdateOptionVerification))
                    return CreateDataError("Can't send code");

                var secret = secretAuthToken.UpdateOptionVerification;

                var isok = authenticator.CheckCode(secret, code, userModel);

                if (!isok) return CreateDataError("Can't update options");


                var option = value["option"];

                userModel.Verification = (int) option;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_NOTIFICATION,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code when enable twofa
        [HttpPost("enable/update")]
        public string VerifyCodeEnable([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                {
                    //return error
                    return CreateDataError("User not exist in DB");
                }

                if (!value.ContainsKey("code")) return CreateDataError("Can't verify code");

                var code = value["code"].ToString();
                var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                if (string.IsNullOrEmpty(secretAuthToken.TwofaEnable))
                    return CreateDataError("Can't send code");

                var secret = secretAuthToken.TwofaEnable;

                var isok = authenticator.CheckCode(secret, code, userModel);

                if (!isok) return CreateDataError("Can't verify code");
                userModel.TwoFactor = true;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.TWOFA_ENABLE,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }


        // POST api/values
        // verify code when disable twofa
        [HttpPost("disable/update")]
        public string VerifyCodeDisable([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                {
                    //return error
                    return CreateDataError("User not exist in DB");
                }

                if (!value.ContainsKey("code")) return CreateDataError("Can't verify code");

                var code = value["code"].ToString();
                var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                if (string.IsNullOrEmpty(secretAuthToken.TwofaDisable))
                    return CreateDataError("Can't send code");

                var secret = secretAuthToken.TwofaDisable;

                var isok = authenticator.CheckCode(secret, code, userModel);

                if (!isok) return CreateDataError("Can't verify code");
                userModel.TwoFactor = false;

                _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.TWOFA_DISABLE,
                    HelpersApi.GetIp(Request));

                return _userBusiness.UpdateProfile(userModel).ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
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
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);


                if (userModel == null) return CreateDataError("Can't send code");

                var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.UPDATE_NOTIFICATION);

                if (checkSecret == null)
                    return CreateDataError("Can't send code");

                userModel.SecretAuthToken = checkSecret;
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.STATUS_ERROR)
                    return CreateDataError("Can't send code");


                var secretAuthToken = ActionCode.FromJson(checkSecret);

                if (string.IsNullOrEmpty(secretAuthToken.UpdateOptionVerification))
                    return CreateDataError("Can't send code");

                var secret = secretAuthToken.UpdateOptionVerification;

                var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
                var code = authenticator.GetCode(secret);

                Console.WriteLine(code);

                return _userBusiness.SendSms(userModel, code).ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
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
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);


                if (userModel == null) return CreateDataError("Can't send code");

                var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.TWOFA_DISABLE);

                if (checkSecret == null)
                    return CreateDataError("Can't send code");

                userModel.SecretAuthToken = checkSecret;
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.STATUS_ERROR)
                    return CreateDataError("Can't send code");


                var secretAuthToken = ActionCode.FromJson(checkSecret);

                if (string.IsNullOrEmpty(secretAuthToken.TwofaDisable))
                    return CreateDataError("Can't send code");

                var secret = secretAuthToken.TwofaDisable;

                var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
                var code = authenticator.GetCode(secret);

                Console.WriteLine(code);

                return _userBusiness.SendSms(userModel, code).ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("enable/require-send-code-phone")]
        public string SendCodeEnable()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);


                if (userModel == null) return CreateDataError("Can't send code");
                var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.TWOFA_ENABLE);

                if (checkSecret == null)
                    return CreateDataError("Can't send code");

                userModel.SecretAuthToken = checkSecret;
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.STATUS_ERROR)
                    return CreateDataError("Can't send code");


                var secretAuthToken = ActionCode.FromJson(checkSecret);

                if (string.IsNullOrEmpty(secretAuthToken.TwofaEnable))
                    return CreateDataError("Can't send code");

                var secret = secretAuthToken.TwofaEnable;

                var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
                var code = authenticator.GetCode(secret);

                Console.WriteLine(code);

                return _userBusiness.SendSms(userModel, code).ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }


        public string CreateDataError(string message)
        {
            return new ReturnObject
            {
                Status = Status.STATUS_ERROR,
                Message = message
            }.ToJson();
        }
    }
}