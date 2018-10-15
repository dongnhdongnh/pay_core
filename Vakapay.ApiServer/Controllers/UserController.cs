using System;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UAParser;
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.UserBusiness;
using Vakapay.WalletBusiness;
using IPGeographicalLocation = Vakapay.Commons.Helpers.IPGeographicalLocation;

namespace Vakaxa.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private UserBusiness _userBusiness;
        private WalletBusiness _walletBusiness;
        private VakapayRepositoryMysqlPersistenceFactory _persistenceFactory;

        private IConfiguration Configuration { get; }

        public UserController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
        }

        [HttpGet("getUserInfo")]
        public IActionResult GetUserInfo()
        {
            return new JsonResult(from c in User.Claims where c.Type == "userInfo" select new {c.Value});
        }

        [HttpPost("upload-avatar"), DisableRequestSizeLimit]
        public string UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userCheck = _userBusiness.GetUserInfo(new Dictionary<string, string>
                {
                    {"Email", email}
                });

                if (userCheck == null)
                    return CreateDataError("Can't User");

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
                _userBusiness.AddActionLog(email, userCheck.Id,
                    ActionLog.AVATAR,
                    HelpersApi.getIp(Request));

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

                string ip = HelpersApi.getIp(Request);

                if (!string.IsNullOrEmpty(ip))
                {
                    //get location for ip
                    var location =
                        await IPGeographicalLocation.QueryGeographicalLocationAsync(ip);

                    var uaString = Request.Headers["User-Agent"].FirstOrDefault();
                    var uaParser = Parser.GetDefault();
                    ClientInfo browser = uaParser.Parse(uaString);

                    var webSession = new WebSession
                    {
                        Browser = browser.ToString(),
                        Ip = ip,
                        Location = !string.IsNullOrEmpty(location.CountryName)
                            ? location.City + "," + location.CountryName
                            : "localhost",
                        UserId = userModel.Id,
                        Current = true
                    };

                    var search = new Dictionary<string, string> {{"Ip", ip}, {"Browser", browser.ToString()}};


                    //save web session
                    var checkWebSession = _userBusiness.GetWebSession(search);
                    if (checkWebSession == null)
                    {
                        _userBusiness.SaveWebSession(webSession);
                    }
                    else
                    {
                        checkWebSession.Current = true;
                        _userBusiness.SaveWebSession(checkWebSession);
                    }
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
        public string UpdateUserProfile([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};
                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                {
                    //return error
                    return CreateDataError("User not exist in DB");
                }

                if (value.ContainsKey("streetAddress1"))
                {
                    userModel.StreetAddress1 = value["streetAddress1"].ToString();
                }

                if (value.ContainsKey("streetAddress2"))
                {
                    userModel.StreetAddress2 = value["streetAddress2"].ToString();
                }

                if (value.ContainsKey("city"))
                {
                    userModel.City = value["city"].ToString();
                }

                if (value.ContainsKey("postalCode"))
                {
                    userModel.PostalCode = value["postalCode"].ToString();
                }

                var result = _userBusiness.UpdateProfile(userModel);

                //save action log
                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_PROFILE,
                    HelpersApi.getIp(Request)).ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        [HttpPost("update-preferences")]
        public string UpdatePreferences([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};
                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                {
                    //return error
                    return CreateDataError("User not exist in DB");
                }

                if (value.ContainsKey("currencyKey"))
                {
                    var currencyKey = value["currencyKey"].ToString();
                    if (!string.IsNullOrEmpty(currencyKey) && Constants.listCurrency.ContainsKey(currencyKey))
                    {
                        userModel.CurrencyKey = currencyKey;
                    }
                    else
                    {
                        return CreateDataError("Currency Key is not exist");
                    }
                }

                if (value.ContainsKey("timezoneKey"))
                {
                    var timezoneKey = value["timezoneKey"].ToString();
                    if (!string.IsNullOrEmpty(timezoneKey) && Constants.listTimeZone.ContainsKey(timezoneKey))
                    {
                        userModel.TimezoneKey = timezoneKey;
                    }
                    else
                    {
                        return CreateDataError("Timezone Key is not exist");
                    }
                }

                var result = _userBusiness.UpdateProfile(userModel);

                //save action log
                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_PREFERENCES,
                    HelpersApi.getIp(Request)).ToJson();
            }
            catch (Exception e)
            {
                return CreateDataError(e.Message);
            }
        }

        [HttpPost("update-notifications")]
        public string UpdateNotifications([FromBody] JObject value)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};
                if (_userBusiness == null)
                {
                    CreateUserBusiness();
                }

                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                {
                    //return error
                    return CreateDataError("User not exist in DB");
                }

                if (value.ContainsKey("notifications"))
                {
                    userModel.CurrencyKey = value["notifications"].ToString();
                }

                var result = _userBusiness.UpdateProfile(userModel);

                return _userBusiness.AddActionLog(userModel.Email, userModel.Id,
                    ActionLog.UPDATE_NOTIFICATION,
                    HelpersApi.getIp(Request)).ToJson();
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

            _userBusiness = new UserBusiness(_persistenceFactory);
        }

        private void CreateWalletBusiness()
        {
            if (_persistenceFactory == null)
            {
                CreateVakapayRepositoryMysqlPersistenceFactory();
            }

            _walletBusiness = new WalletBusiness(_persistenceFactory);
        }

        private void CreateVakapayRepositoryMysqlPersistenceFactory()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = Configuration.GetConnectionString("DefaultConnection")
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