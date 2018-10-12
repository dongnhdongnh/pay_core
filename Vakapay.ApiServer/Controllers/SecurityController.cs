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


                var userModel = _userBusiness.getUserInfo(query);

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
        [HttpPost("close-account/update")]
        public string UpdateCloseAccount([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};


                var userModel = _userBusiness.getUserInfo(query);

                if (userModel == null)
                {
                    //return error
                    return CreateDataError("User not exist in DB");
                }


                if (value.ContainsKey("code"))
                {
                    var code = value["code"].ToString();
                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.UpdateOptionVerification))
                        return CreateDataError("Can't send code");

                    var secret = secretAuthToken.UpdateOptionVerification;

                    bool isok = authenticator.CheckCode(secret, code);

                    if (isok)
                    {
                        if (value.ContainsKey("option"))
                        {
                            var option = value["option"];

                            userModel.Verification = (int) option;

                            var resultUpdate = _userBusiness.UpdateProfile(userModel);
                            // resultUpdate.Data = JsonHelper.SerializeObject(userModel);
                            return _userBusiness.AddActionLog(email, userModel.Id, ActionLog.TwofaEnable,
                                Request.Headers["X-Original-Forwarded-For"].FirstOrDefault()).ToJson();
                        }
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
        [HttpPost("close-account/require-send-code-phone")]
        public string SendCodeClose()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.getUserInfo(query);


                if (userModel != null)
                {
                    var checkSecret = CheckToken(userModel, ActionLog.CloseAccount);

                    if (checkSecret == null)
                        return CreateDataError("Can't send code");

                    var secretAuthToken = ActionCode.FromJson(checkSecret);

                    if (string.IsNullOrEmpty(secretAuthToken.CloseAccount))
                        return CreateDataError("Can't send code");

                    var secret = secretAuthToken.CloseAccount;

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


        private string CheckToken(User userModel, string action)
        {
            try
            {
                var newSecret = new ActionCode();

                if (string.IsNullOrEmpty(userModel.SecretAuthToken))
                {
                    switch (action)
                    {
                        case ActionLog.TwofaEnable:
                            newSecret.TwofaEnable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case ActionLog.UpdateOptionVerification:
                            newSecret.UpdateOptionVerification = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case ActionLog.CloseAccount:
                            newSecret.CloseAccount = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                    }
                }
                else
                {
                    newSecret = ActionCode.FromJson(userModel.SecretAuthToken);

                    switch (action)
                    {
                        case ActionLog.TwofaEnable:
                            if (string.IsNullOrEmpty(newSecret.TwofaEnable))
                            {
                                newSecret.TwofaEnable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case ActionLog.UpdateOptionVerification:
                            if (string.IsNullOrEmpty(newSecret.UpdateOptionVerification))
                            {
                                newSecret.UpdateOptionVerification = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case ActionLog.CloseAccount:
                            if (string.IsNullOrEmpty(newSecret.CloseAccount))
                            {
                                newSecret.CloseAccount = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                    }
                }

                userModel.SecretAuthToken = ActionCode.ToJson(newSecret);
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.STATUS_ERROR)
                    return null;

                return JsonHelper.SerializeObject(newSecret);
            }
            catch (Exception e)
            {
                return null;
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