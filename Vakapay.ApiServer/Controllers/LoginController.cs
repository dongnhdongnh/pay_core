using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    public class LoginController : Controller
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private readonly WalletBusiness.WalletBusiness _walletBusiness;
        private IConfiguration Configuration { get; }
        private IVakapayRepositoryFactory _repositoryFactory;
        private IHostingEnvironment _env;


        public LoginController(
            IVakapayRepositoryFactory persistenceFactory,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment)
        {
            _repositoryFactory = persistenceFactory;
            _env = hostingEnvironment;
            Configuration = configuration;

            _userBusiness = new UserBusiness.UserBusiness(persistenceFactory);
            _walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);
        }


        [HttpGet("get-info")]
        public async Task<string> GetCurrentUser()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value)
                    .SingleOrDefault();

                if (string.IsNullOrEmpty(email))
                    return CreateDataError("Can't not email");

                var query = new Dictionary<string, string> {{"Email", email}};


                var userModel = _userBusiness.GetUserInfo(query);

              

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = userModel.ToJson()
                }.ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        private void UpdateCurrencyAndTimeZone(User userModel, IpGeographicalLocation location)
        {
            var isUpdate = false;
            if (userModel.CurrencyKey == null && location?.Currency != null)
            {
                userModel.CurrencyKey = location.Currency.Code;
                isUpdate = true;
            }

            if (userModel.TimezoneKey == null && location?.TimeZone != null)
            {
                userModel.TimezoneKey = location.TimeZone.Id;
                isUpdate = true;
            }

            if (isUpdate)
            {
                _userBusiness.UpdateProfile(userModel);
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