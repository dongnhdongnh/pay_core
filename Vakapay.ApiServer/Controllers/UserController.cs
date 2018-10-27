﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using UAParser;
using Vakapay.ApiServer.ActionFilter;
using Vakapay.ApiServer.Helpers;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models;
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
        public string UploadFile()
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


                const string folderName = "wwwroot/upload/avatar";
                var link = "/upload/avatar/";
                var webRootPath = Directory.GetCurrentDirectory();
                var newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }


                if (file.Length <= 0) return CreateDataError("Can't update image");
                char[] myChar = {'"'};
                var fileName = CommonHelper.GetUnixTimestamp() + ContentDispositionHeaderValue
                                   .Parse(file.ContentDisposition).FileName.ToString()
                                   .Trim(myChar);

                fileName = fileName.Replace(" ", "-");

                var fullPath = Path.Combine(newPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var oldAvatar = userCheck.Avatar;

                link = link + fileName;


                //update avatar for user
                userCheck.Avatar = link;
                var updateUser = _userBusiness.UpdateProfile(userCheck);


                //delete image old
                if (!string.IsNullOrEmpty(oldAvatar))
                {
                    var oldName = oldAvatar.Split("/");

                    var oldFullPath = Path.Combine(newPath, oldName[oldName.Length - 1]);

                    if (System.IO.File.Exists(oldFullPath))
                    {
                        System.IO.File.Delete(oldFullPath);
                    }
                }

                if (updateUser.Status != Status.STATUS_SUCCESS) return CreateDataError("Can't update image");
                //save action log
                _userBusiness.AddActionLog(userCheck.Email, userCheck.Id,
                    ActionLog.AVATAR,
                    HelpersApi.GetIp(Request));

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Message = "Upload avatar success ",
                    Data = link
                }.ToJson();
            }
            catch (Exception ex)
            {
                return CreateDataError(ex.Message);
            }
        }

        [HttpGet("get-info")]
        public async Task<string> GetCurrentUser()
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

                if (string.IsNullOrEmpty(email))
                    return CreateDataError("Can't not email");

                var query = new Dictionary<string, string> {{"Email", email}};

                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = _userBusiness.GetUserInfo(query);


                if (userModel == null)
                {
                    var jsonUser = User.Claims.Where(c => c.Type == "userInfo").Select(c => c.Value).SingleOrDefault();

                    var userClaims = Vakapay.Models.Entities.User.FromJson(jsonUser);

                    var resultData = _userBusiness.Login(userClaims);

                    if (resultData.Status == Status.STATUS_ERROR)
                        return CreateDataError("Can't not created User");


                    userModel = Vakapay.Models.Entities.User.FromJson(resultData.Data);

                    if (_walletBusiness == null)
                    {
                        CreateWalletBusiness();
                    }

                    return _walletBusiness.MakeAllWalletForNewUser(userModel).ToJson();
                }

                var ip = HelpersApi.GetIp(Request);


                if (string.IsNullOrEmpty(ip))
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = Vakapay.Models.Entities.User.ToJson(userModel)
                    }.ToJson();

                //get location for ip
                var location =
                    await IpGeographicalLocation.QueryGeographicalLocationAsync(ip);

                var browser = HelpersApi.GetBrowser(Request);

                var confirmedDevices = new ConfirmedDevices
                {
                    Browser = browser,
                    Ip = ip,
                    Location = !string.IsNullOrEmpty(location.CountryName)
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

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = Vakapay.Models.Entities.User.ToJson(userModel)
                }.ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        // POST api/values
        [HttpPost("update-profile")]
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
                return CreateDataError(e.Message);
            }
        }

        [HttpPost("update-preferences")]
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
                    if (!string.IsNullOrEmpty(currencyKey) && Constants.listCurrency.ContainsKey(currencyKey))
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
                    if (!string.IsNullOrEmpty(timezoneKey) && Constants.ListTimeZone.ContainsKey(timezoneKey))
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
                return CreateDataError(e.Message);
            }
        }

        [HttpPost("update-notifications")]
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
                    userModel.CurrencyKey = value[ParseDataKeyApi.KEY_USER_UPDATE_NOTIFICATION].ToString();
                }

                _userBusiness.UpdateProfile(userModel);

                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_NOTIFICATION,
                    HelpersApi.GetIp(Request)).ToJson();
            }
            catch (Exception e)
            {
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
                ConnectionString = AppSettingHelper.GetDBConnection()
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
    }
}