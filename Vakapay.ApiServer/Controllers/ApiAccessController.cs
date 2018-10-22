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
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models;
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
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            _userBusiness = new UserBusiness.UserBusiness(PersistenceFactory);
        }

        [HttpGet("api-access/get-list")]
        public string GetListApiAccess()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel != null)
                {
                    var list = Constants.listApiAccess;

                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonConvert.SerializeObject(list)
                    }.ToJson();
                }

                return HelpersApi.CreateDataError("Can't get list api access");
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
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                    return HelpersApi.CreateDataError("User not exist in DB");

                if (value.ContainsKey("code"))
                {
                    var code = value["code"].ToString();

                    var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

                    var secretAuthToken = ActionCode.FromJson(userModel.SecretAuthToken);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError("Can't send code");

                    var secret = secretAuthToken.ApiAccess;

                    Console.WriteLine(secret);
                    var isok = authenticator.CheckCode(secret, code, userModel);

                    if (!isok) return HelpersApi.CreateDataError("Can't update options");

                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Message = "Verify access"
                    }.ToJson();
                }

                return HelpersApi.CreateDataError("Can't update options");
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        // then code when add api access
        [HttpPost("api-access/require-send-code-phone")]
        public string SendCodeAdd()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel != null)
                {
                    var checkSecret = HelpersApi.CheckToken(userModel, ActionLog.API_ACCESS);

                    if (checkSecret == null)
                        return HelpersApi.CreateDataError("Can't CheckToken");

                    userModel.SecretAuthToken = checkSecret;
                    var resultUpdate = _userBusiness.UpdateProfile(userModel);

                    if (resultUpdate.Status == Status.STATUS_ERROR)
                        return resultUpdate.ToJson();

                    var secretAuthToken = ActionCode.FromJson(checkSecret);

                    if (string.IsNullOrEmpty(secretAuthToken.ApiAccess))
                        return HelpersApi.CreateDataError("Can't secretAuthToken ApiAccess");
                    Console.WriteLine(secretAuthToken.ApiAccess);
                    return _userBusiness.SendSms(userModel, HelpersApi.SendCodeSms(secretAuthToken.ApiAccess))
                        .ToJson();
                }

                return HelpersApi.CreateDataError("Can't send code");
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }
    }
}