using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using UAParser;
using Vakapay.ApiServer.ActionFilter;
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Helpers;
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
    public class ActivityController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }

        public ActivityController(
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
        [HttpGet("account-activity/get-list")]
        public string GetActivity()
        {
            try
            {
                var queryStringValue = Request.Query;
                var userModel = (User)RouteData.Values["UserModel"];

                if (!queryStringValue.ContainsKey("offset") || !queryStringValue.ContainsKey("limit"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                queryStringValue.TryGetValue("offset", out var offset);
                queryStringValue.TryGetValue("limit", out var limit);


                if (userModel != null)
                {
                    return _userBusiness.GetActionLog(userModel.Id, Convert.ToInt32(offset),
                        Convert.ToInt32(limit)).ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpGet("device-history/get-list")]
        public string GetConfirmedDevices()
        {
            try
            {
                var queryStringValue = Request.Query;

                if (!queryStringValue.ContainsKey("offset") || !queryStringValue.ContainsKey("limit"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                queryStringValue.TryGetValue("offset", out var offset);
                queryStringValue.TryGetValue("limit", out var limit);

                var ip = HelpersApi.GetIp(Request);

                var checkConfirmedDevices = new ConfirmedDevices();

                if (!string.IsNullOrEmpty(ip))
                {
                    //get location for ip

                    var uaString = Request.Headers["User-Agent"].FirstOrDefault();
                    var uaParser = Parser.GetDefault();
                    var browser = uaParser.Parse(uaString);

                    var search = new Dictionary<string, string> {{"Ip", ip}, {"Browser", browser.ToString()}};

                    //save web session
                    checkConfirmedDevices = _userBusiness.GetConfirmedDevices(search);
                }

                var userModel = (User)RouteData.Values["UserModel"];


                return _userBusiness.GetListConfirmedDevices(userModel.Id, Convert.ToInt32(offset),
                    Convert.ToInt32(limit), checkConfirmedDevices).ToJson();
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("device-history/delete")]
        public string DeleteConfirmedDevicesById([FromBody] JObject value)
        {
            try
            {
                return value.ContainsKey("Id")
                    ? _userBusiness.DeleteConfirmedDevicesById(value["Id"].ToString()).ToJson()
                    : HelpersApi.CreateDataError("ID Not exist.");
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("account-activity/delete")]
        public string DeleteUserActivityById([FromBody] JObject value)
        {
            try
            {
                return value.ContainsKey("Id")
                    ? _userBusiness.DeleteActivityById(value["Id"].ToString()).ToJson()
                    : HelpersApi.CreateDataError("ID Not exist.");
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }
    }
}