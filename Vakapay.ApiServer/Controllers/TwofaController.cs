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
    public class TwofaController : ControllerBase
    {
        private UserBusiness _userBusiness;
        private WalletBusiness _walletBusiness;
        private VakapayRepositoryMysqlPersistenceFactory _persistenceFactory;


        private IConfiguration Configuration { get; }

        public TwofaController(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        )
        {
            Configuration = configuration;
        }


        // POST api/values
        [HttpPost("enable/verify-with-phone")]
        public string VerifyCode([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = _userBusiness.getUserInfo(query);
                if (value.ContainsKey("code"))
                {
                    var code = value["code"].ToString();
                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.TwofaEnable))
                        return CreateDataError("Can't send code");

                    var secret = secretAuthToken.TwofaEnable;

                    bool isok = authenticator.CheckCode(secret, code);

                    if (isok)
                    {
                        var resultSend = new ReturnObject
                        {
                            Status = Status.StatusSuccess,
                        };

                        return ReturnObject.ToJson(resultSend);
                    }
                }

                return CreateDataError("Can't verify code");
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("enable/require-send-code-phone")]
        public string SendCode([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};
                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = _userBusiness.getUserInfo(query);


                if (userModel != null)
                {
                    var checkSecret = CheckToken(userModel, "TwofaEnable");

                    if (checkSecret == null)
                        return CreateDataError("Can't send code");

                    var secretAuthToken = ActionCode.FromJson(checkSecret);

                    if (string.IsNullOrEmpty(secretAuthToken.TwofaEnable))
                        return CreateDataError("Can't send code");

                    var secret = secretAuthToken.TwofaEnable;

                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
                    var code = authenticator.GetCode(secret);

                    Console.WriteLine(code);

                    var dataSend = _userBusiness.SendSms(userModel, code);

                    return ReturnObject.ToJson(dataSend);
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
                        case "TwofaEnable":
                            newSecret.TwofaEnable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case "CloseAccount":
                            newSecret.CloseAccount = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                    }
                }
                else
                {
                    newSecret = ActionCode.FromJson(userModel.SecretAuthToken);

                    switch (action)
                    {
                        case "TwofaEnable":
                            if (string.IsNullOrEmpty(newSecret.TwofaEnable))
                            {
                                newSecret.TwofaEnable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case "CloseAccount":
                            if (string.IsNullOrEmpty(newSecret.CloseAccount))
                            {
                                newSecret.CloseAccount = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                    }
                }

                userModel.SecretAuthToken = ActionCode.ToJson(newSecret);
                var resultUpdate = _userBusiness.UpdateProfile(userModel);

                if (resultUpdate.Status == Status.StatusError)
                    return null;

                return JsonConvert.SerializeObject(newSecret);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // POST api/values
        [HttpPost("enable/verify-test")]
        public string VerifyTest([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = _userBusiness.getUserInfo(query);

                if (value.ContainsKey("code"))
                {
                }


                return CreateDataError(Status.StatusSuccess);
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        /*// POST api/values
        [HttpPost("enable/send-test")]
        public string SendTest([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};
                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = _userBusiness.getUserInfo(query);

                var action = value["action"].ToString();

                if (string.IsNullOrEmpty(action))
                    return CreateDataError("Can't action");

                if (userModel != null)
                {
                    var resultSend = _userBusiness.SendCode(userModel, action);

                    return ReturnObject.ToJson(resultSend);
                }

                return CreateDataError("Can't send code");
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }*/

        public string CreateDataError(string message)
        {
            var errorData = new ReturnObject
            {
                Status = Status.StatusError,
                Message = message
            };
            return ReturnObject.ToJson(errorData);
        }

        private void CreateDataActionLog(string email, string idUser, string actionLog)
        {
            //save action log
            try
            {
                _userBusiness.AddActionLog(new UserActionLog
                {
                    ActionName = actionLog,
                    Description = email,
                    Ip = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserId = idUser
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CreateUserBusiness()
        {
            if (_persistenceFactory == null)
            {
                CreateVakapayRepositoryMysqlPersistenceFactory();
            }

            _userBusiness = new UserBusiness(_persistenceFactory);
        }

        private void CreateVakapayRepositoryMysqlPersistenceFactory()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = Configuration.GetConnectionString("DefaultConnection")
            };

            _persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
        }
    }
}