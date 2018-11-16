using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NLog;
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
    public class UserController : ControllerBase
    {
        private UserBusiness.UserBusiness _userBusiness;
        private WalletBusiness.WalletBusiness _walletBusiness;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private VakapayRepositoryMysqlPersistenceFactory _persistenceFactory;

        public UserController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
        }

        [HttpGet("getUserInfo")]
        public IActionResult GetUserInfo()
        {
            return new JsonResult(from c in User.Claims
                where c.Type == ParseDataKeyApi.KEY_CLAIM_GET_DATA_USER_IDENTITY
                select new {c.Value});
        }


        [HttpPost("upload-avatar"), DisableRequestSizeLimit]
        [BaseActionFilter]
        public async Task<string> UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];

                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userCheck = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (file.Length > 2097152)
                    return CreateDataError("File max size 2Mb");

                if (file.Length <= 0) return CreateDataError("Can't update image");


                using (var w = new WebClient())
                {
                    w.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    try
                    {
                        using (var m = new MemoryStream())
                        {
                            file.CopyTo(m);
                            m.Close();
                            // Convert byte[] to Base64 String
                            var base64String = Convert.ToBase64String(m.GetBuffer());

                            var values = new NameValueCollection
                            {
                                {ParseDataKeyApi.KEY_USER_UPDATE_AVATAR, base64String}
                            };

                            w.Headers.Add("Authorization", "Client-ID " + AppSettingHelper.GetImgurApiKey());

                            byte[] response = await w.UploadValuesTaskAsync(AppSettingHelper.GetImgurUrl(), values);

                            var result = JsonHelper.DeserializeObject<JObject>(Encoding.UTF8.GetString(response));

                            if (!(bool) result["success"])
                                return CreateDataError("Save image fail");
                            userCheck.Avatar = result["data"]["link"].ToString();
                            var updateUser = _userBusiness.UpdateProfile(userCheck);

                            if (updateUser.Status != Status.STATUS_SUCCESS)
                                return CreateDataError("Can't update image");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }

                    //save action log
                    _userBusiness.AddActionLog(userCheck.Email, userCheck.Id,
                        ActionLog.AVATAR,
                        HelpersApi.GetIp(Request));

                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Message = "Upload avatar success ",
                        Data = userCheck.Avatar
                    }.ToJson();
                }
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.USER_AVATAR + e);
                return CreateDataError(e.Message);
            }
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

                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

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

                    if (_walletBusiness == null)
                    {
                        CreateWalletBusiness();
                    }

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
                _logger.Error(KeyLogger.USER_UPDATE + e);
                return CreateDataError(e.Message);
            }
        }

        // POST api/values
        [
            HttpPost("update-profile")]
        [BaseActionFilter]
        public string UpdateUserProfile([FromBody] JObject value)
        {
            try
            {
                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (value.ContainsKey(ParseDataKeyApi.KEY_USER_UPDATE_PROFILE_ADDRESS_1))
                {
                    userModel.StreetAddress1 = value[ParseDataKeyApi.KEY_USER_UPDATE_PROFILE_ADDRESS_1].ToString();
                }

                if (value.ContainsKey(ParseDataKeyApi.KEY_USER_UPDATE_PROFILE_ADDRESS_2))
                {
                    userModel.StreetAddress2 = value[ParseDataKeyApi.KEY_USER_UPDATE_PROFILE_ADDRESS_2].ToString();
                }

                if (value.ContainsKey(ParseDataKeyApi.KEY_USER_UPDATE_PROFILE_CITY))
                {
                    userModel.City = value[ParseDataKeyApi.KEY_USER_UPDATE_PROFILE_CITY].ToString();
                }

                if (value.ContainsKey(ParseDataKeyApi.KEY_USER_UPDATE_PROFILE_POSTAL_CODE))
                {
                    userModel.PostalCode = value[ParseDataKeyApi.KEY_USER_UPDATE_PROFILE_POSTAL_CODE].ToString();
                }

                _userBusiness.UpdateProfile(userModel);

                //save action log
                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_PROFILE,
                    HelpersApi.GetIp(Request)).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.USER_UPDATE + e);
                return CreateDataError(e.Message);
            }
        }

        [
            HttpPost("update-preferences")]
        [BaseActionFilter]
        public string UpdatePreferences([FromBody] JObject value)
        {
            try
            {
                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (value.ContainsKey(ParseDataKeyApi.KEY_USER_UPDATE_PREFERENCES_CURRENCY))
                {
                    var currencyKey = value[ParseDataKeyApi.KEY_USER_UPDATE_PREFERENCES_CURRENCY].ToString();
                    if (!string.IsNullOrEmpty(currencyKey) &&
                        PaymentCurrency.LIST_CURRENCY.ContainsKey(currencyKey))
                    {
                        userModel.CurrencyKey = currencyKey;
                    }
                    else
                    {
                        return CreateDataError("Currency Key is not exist");
                    }
                }

                if (value.ContainsKey(ParseDataKeyApi.KEY_USER_UPDATE_PREFERENCES_TIMEZONE))
                {
                    var timezoneKey = value[ParseDataKeyApi.KEY_USER_UPDATE_PREFERENCES_TIMEZONE].ToString();
                    if (!string.IsNullOrEmpty(timezoneKey) && Timezone.LIST_TIME_ZONE.ContainsKey(timezoneKey))
                    {
                        userModel.TimezoneKey = timezoneKey;
                    }
                    else
                    {
                        return CreateDataError("Timezone Key is not exist");
                    }
                }

                _userBusiness.UpdateProfile(userModel);

                //save action log
                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_PREFERENCES,
                    HelpersApi.GetIp(Request)).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.USER_UPDATE_PREFERENCES + e);
                return CreateDataError(e.Message);
            }
        }

        [
            HttpPost("update-notifications")]
        [BaseActionFilter]
        public string UpdateNotifications([FromBody] JObject value)
        {
            try
            {
                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = (User) RouteData.Values[ParseDataKeyApi.KEY_PASS_DATA_USER_MODEL];

                if (value.ContainsKey(ParseDataKeyApi.KEY_USER_UPDATE_NOTIFICATION))
                {
                    userModel.Notifications = value[ParseDataKeyApi.KEY_USER_UPDATE_NOTIFICATION].ToString();
                }

                _userBusiness.UpdateProfile(userModel);

                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_NOTIFICATION,
                    HelpersApi.GetIp(Request)).ToJson();
            }
            catch (Exception e)
            {
                _logger.Error(KeyLogger.USER_UPDATE_NOTIFICATION + e);
                return CreateDataError(e.Message);
            }
        }

        private void CreateUserBusiness()
        {
            if (_persistenceFactory == null)
            {
                CreateVakapayRepositoryMysqlPersistenceFactory();
            }

            _userBusiness = new UserBusiness.UserBusiness(_persistenceFactory);
        }

        private void CreateWalletBusiness()
        {
            if (_persistenceFactory == null)
            {
                CreateVakapayRepositoryMysqlPersistenceFactory();
            }

            _walletBusiness = new WalletBusiness.WalletBusiness(_persistenceFactory);
        }

        private void CreateVakapayRepositoryMysqlPersistenceFactory()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            _persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
        }

        public string CreateDataError(string message)
        {
            return new ReturnObject
            {
                Status = Status.STATUS_ERROR,
                Message = message
            }.ToJson();
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
    }
}