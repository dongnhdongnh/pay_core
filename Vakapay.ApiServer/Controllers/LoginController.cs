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
    public class LoginController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private readonly WalletBusiness.WalletBusiness _walletBusiness;
        protected IConfiguration Configuration { get; }
        private IVakapayRepositoryFactory _repositoryFactory;
        protected IHostingEnvironment _env;


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

                var ip = HelpersApi.GetIp(Request);
                IpGeographicalLocation location = null;
                if (ip != null)
                {
                    //get location for ip
                    location = await IpGeographicalLocation.QueryGeographicalLocationAsync(ip);
                }

                if (userModel == null)
                {
                    var jsonUser = User.Claims.Where(c => c.Type == "userInfo").Select(c => c.Value)
                        .SingleOrDefault();

                    var userClaims = Vakapay.Models.Entities.User.FromJson(jsonUser);
                    userClaims.Notifications = "1,2,3";
                    if (location != null)
                    {
                        if (location.Currency?.Code != null)
                        {
                            userClaims.CurrencyKey = location.Currency.Code;
                        }

                        if (location.TimeZone?.Id != null)
                        {
                            userClaims.TimezoneKey = location.TimeZone.Id;
                        }
                    }

                    var resultData = _userBusiness.Login(userClaims);

                    if (resultData.Status == Status.STATUS_ERROR)
                        return CreateDataError(resultData.Message);


                    userModel = Vakapay.Models.Entities.User.FromJson(resultData.Data);


                    return _walletBusiness.MakeAllWalletForNewUser(userModel).ToJson();
                }

                if (string.IsNullOrEmpty(ip))
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = Vakapay.Models.Entities.User.ToJson(userModel)
                    }.ToJson();

                UpdateCurrencyAndTimeZone(userModel, location);

                var browser = HelpersApi.GetBrowser(Request);

                var confirmedDevices = new ConfirmedDevices
                {
                    Browser = browser,
                    Ip = ip,
                    Location = location != null && !string.IsNullOrEmpty(location.CountryName)
                        ? location.City + "," + location.CountryName
                        : "localhost",
                    UserId = userModel.Id
                };

                var search = new Dictionary<string, string> {{"Ip", ip}, {"Browser", browser}};


                //save devices
                var checkConfirmedDevices = _userBusiness.GetConfirmedDevices(search);
                if (checkConfirmedDevices == null)
                {
                    _userBusiness.SaveConfirmedDevices(confirmedDevices);
                }


                userModel.SecretAuthToken = null;
                userModel.TwoFactorSecret = null;
                userModel.SecondPassword = null;
                userModel.Id = null;
                userModel.PhoneNumber = !string.IsNullOrEmpty(userModel.PhoneNumber)
                    ? "*********" + userModel.PhoneNumber.Remove(0, 9)
                    : null;
                if (userModel.Birthday.Contains("1900-01-01"))
                    userModel.Birthday = null;

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