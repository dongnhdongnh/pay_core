using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.UserBusiness;
using Vakapay.WalletBusiness;

namespace Vakaxa.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly UserBusiness _userBusiness;
        private WalletBusiness _walletBusiness;
        private VakapayRepositoryMysqlPersistenceFactory _persistenceFactory { get; }


        private IConfiguration Configuration { get; }

        public SecurityController(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        )
        {
            Configuration = configuration;

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = Configuration.GetConnectionString("DefaultConnection")
            };

            _persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            _userBusiness = new UserBusiness(_persistenceFactory);
        }

        [HttpGet("get-info")]
        public string GetInfo()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};


                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                {
                    return CreateDataError("Can't User");
                }

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
                return CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("lock-screen/update")]
        public string UpdateCloseAccount([FromBody] JObject value)
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


                if (value.ContainsKey("code") && value.ContainsKey("status") && value.ContainsKey("password"))
                {
                    var code = value["code"].ToString();
                    var status = value["status"];

                    if (!int.TryParse((string) status, out int outStatus))
                        return CreateDataError("Status invalid");

                    var password = value["password"];
                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.LockScreen))
                        return CreateDataError("Can't send code");

                    var secret = secretAuthToken.LockScreen;

                    bool isok = authenticator.CheckCode(secret, code);
                    Console.WriteLine(isok);
                    if (isok)
                    {
                        userModel.IsLockScreen = (int) status;
                        userModel.SecondPassword = !string.IsNullOrEmpty(password.ToString())
                            ? CommonHelper.Md5(password.ToString())
                            : "";

                        var resultUpdate = _userBusiness.UpdateProfile(userModel);

                        return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                            ActionLog.LockScreen,
                            HelpersApi.getIp(Request)).ToJson();
                    }
                }


                return CreateDataError("Can't update options");
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }


        // POST api/values
        [HttpPost("lock-screen/require-send-code-phone")]
        public string SendCodeLock()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);


                if (userModel != null)
                {
                    var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.LockScreen);
                    
                    if (checkSecret == null)
                        return CreateDataError("Can't send code");

                    userModel.SecretAuthToken = checkSecret;
                    var resultUpdate = _userBusiness.UpdateProfile(userModel);
                    
                    if (resultUpdate.Status == Status.STATUS_ERROR)
                        return CreateDataError("Can't send code");


                    var secretAuthToken = ActionCode.FromJson(checkSecret);

                    if (string.IsNullOrEmpty(secretAuthToken.LockScreen))
                        return CreateDataError("Can't send code");

                    var secret = secretAuthToken.LockScreen;

                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
                    var code = authenticator.GetCode(secret);

                    Console.WriteLine(code);

                    return _userBusiness.SendSms(userModel, code).ToJson();
                }

                return CreateDataError("Can't send code");
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