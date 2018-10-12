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

                var success = new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = JsonConvert.SerializeObject(new SecurityModel
                    {
                        twofaOption = userModel.Verification,
                        isEnableTwofa = userModel.TwoFactor
                    })
                };
                return ReturnObject.ToJson(success);
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        public string CreateDataError(string message)
        {
            var errorData = new ReturnObject
            {
                Status = Status.StatusError,
                Message = message
            };
            return ReturnObject.ToJson(errorData);
        }
    }
}