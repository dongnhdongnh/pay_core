using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using UAParser;
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
                var userModel = (User) RouteData.Values["UserModel"];

                if (!queryStringValue.ContainsKey("offset") || !queryStringValue.ContainsKey("limit"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                StringValues sort;
                StringValues filter;
                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_OFFSET, out var offset);
                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_LIMIT, out var limit);
                if (queryStringValue.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_FILTER))
                    queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_FILTER, out filter);
                if (queryStringValue.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_SORT))
                    queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_SORT, out sort);
                sort = ConvertSortLog(sort);

                if (userModel != null)
                {
                    int numberData;
                    var resultLogs = _userBusiness.GetActionLog(out numberData, userModel.Id, Convert.ToInt32(offset),
                        Convert.ToInt32(limit), filter.ToString(), sort);
                    if (resultLogs.Status != Status.STATUS_SUCCESS)
                        return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);

                    var listLogs = JsonHelper.DeserializeObject<List<UserActionLog>>(resultLogs.Data);
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = new ResultList<UserActionLog>
                        {
                            List = listLogs,
                            Total = numberData
                        }.ToJson()
                    }.ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);
            }
            catch (Exception e)
            {
                return HelpersApi.CreateDataError(e.Message);
            }
        }

        private string ConvertSortLog(string sort)
        {
            if (string.IsNullOrEmpty(sort))
                return null;
            var key = sort;
            var desc = "";
            if (key[0].Equals('-'))
            {
                desc = key[0].ToString();
                key = sort.Remove(0, 1);
            }

            switch (key)
            {
                case "id":
                    return desc + "Id";

                case "actionname":
                    return desc + "ActionName";

                case "ip":
                    return desc + "Ip";

                case "userid":
                    return desc + "UserId";

                case "location":
                    return desc + "Location";

                case "createdAt":
                    return desc + "CreatedAt";

                default:
                    return null;
            }
        }

        private string ConvertSortDevice(string sort)
        {
            if (string.IsNullOrEmpty(sort))
                return null;

            var key = sort;
            var desc = "";
            if (key[0].Equals('-'))
            {
                desc = key[0].ToString();
                key = sort.Remove(0, 1);
            }

            switch (key)
            {
                case "id":
                    return desc + "Id";
                case "userid":
                    return desc + "UserId";
                case "browser":
                    return desc + "Browser";
                case "ip":
                    return desc + "Ip";
                case "location":
                    return desc + "Location";
                case "signedin":
                    return desc + "SignedIn";

                default:
                    return null;
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

                StringValues sort;
                StringValues filter;
                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_OFFSET, out var offset);
                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_LIMIT, out var limit);
                if (queryStringValue.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_FILTER))
                    queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_FILTER, out filter);
                if (queryStringValue.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_SORT))
                    queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_SORT, out sort);

                sort = ConvertSortDevice(sort);

                var ip = HelpersApi.GetIp(Request);

                var checkConfirmedDevices = new ConfirmedDevices();

                if (!string.IsNullOrEmpty(ip))
                {
                    var browser = HelpersApi.GetBrowser(Request);

                    var search = new Dictionary<string, string> {{"Ip", ip}, {"Browser", browser}};

                    //save web session
                    checkConfirmedDevices = _userBusiness.GetConfirmedDevices(search);
                }

                var userModel = (User) RouteData.Values["UserModel"];

                int numberData;
                var resultDevice = _userBusiness.GetListConfirmedDevices(out numberData, userModel.Id,
                    checkConfirmedDevices,
                    Convert.ToInt32(offset),
                    Convert.ToInt32(limit), sort, filter);

                if (resultDevice.Status != Status.STATUS_SUCCESS)
                    return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);

                var listDevice = JsonHelper.DeserializeObject<List<ConfirmedDevices>>(resultDevice.Data);
                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = new ResultList<ConfirmedDevices>
                    {
                        List = listDevice,
                        Total = numberData
                    }.ToJson()
                }.ToJson();
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