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
using Vakapay.Commons.Constants;
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
    public class ActivityController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }


        private IConfiguration Configuration { get; }

        public ActivityController(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        )
        {
            Configuration = configuration;

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = Configuration.GetConnectionString("DefaultConnection")
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

                if (!queryStringValue.ContainsKey("offset") || !queryStringValue.ContainsKey("limit"))
                    return CreateDataError("Offset or limit not found");

                queryStringValue.TryGetValue("offset", out var offset);
                queryStringValue.TryGetValue("limit", out var limit);

                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel != null)
                {
                    return _userBusiness.GetActionLog(userModel.Id, Convert.ToInt32(offset),
                        Convert.ToInt32(limit)).ToJson();
                }

                return CreateDataError("Can't get list account activity");
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpGet("web-session/get-list")]
        public string GetWebSession()
        {
            try
            {
                var queryStringValue = Request.Query;

                if (!queryStringValue.ContainsKey("offset") || !queryStringValue.ContainsKey("limit"))
                    return CreateDataError("Offset or limit not found");

                queryStringValue.TryGetValue("offset", out var offset);
                queryStringValue.TryGetValue("limit", out var limit);

                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel != null)
                {
                    return _userBusiness.GetListWebSession(userModel.Id, Convert.ToInt32(offset),
                        Convert.ToInt32(limit)).ToJson();
                }

                return CreateDataError("Can't get list Web Session activity");
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("web-session/delete")]
        public string DeleteWebSessionById([FromBody] JObject value)
        {
            try
            {
                if (value.ContainsKey("Id"))
                {
                  return  _userBusiness.DeleteWebSessionById(value["Id"].ToString()).ToJson();

                }

                return CreateDataError("ID Not exist.");
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }
        
        // POST api/values
        [HttpPost("account-activity/delete")]
        public string DeleteUserActivityById([FromBody] JObject value)
        {
            try
            {
                if (value.ContainsKey("Id"))
                {
                    return  _userBusiness.DeleteActivityById(value["Id"].ToString()).ToJson();

                }

                return CreateDataError("ID Not exist.");
                
                
                
                
                
                
                
                
                
                
                
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