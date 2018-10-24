using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vakapay.ApiAccess.ActionFilter;
using Vakapay.ApiAccess.Constants;
using Vakapay.ApiAccess.Model;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiAccess.Controllers
{
    [Route("v1")]
    [ApiController]
    [BaseActionFilter]
    public class UserController : ControllerBase
    {
        private VakapayRepositoryMysqlPersistenceFactory VakapayRepositoryFactory { get; }
        private UserBusiness.UserBusiness userBusiness { get; }

        public UserController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            VakapayRepositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            userBusiness = new UserBusiness.UserBusiness(VakapayRepositoryFactory);
        }

        [HttpGet("user")]
        //wallet:user:read
        //wallet:user:email
        public ActionResult<string> GetCurrentUser()
        {
            try
            {
                var headers = Request.Headers;
                string token = headers[Requests.HeaderTokenKey];
                var key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = key.Split(new[] {':'});
                if (parts.Length != 3)
                    return CreateDataError("User Info is not permission");
                var apiKey = parts[1];

                var apiKeyModel = userBusiness.GetApiKeyByKey(apiKey);

                if (string.IsNullOrEmpty(apiKeyModel.Permissions))
                    return CreateDataError("User Info is not permission");

                if (!apiKeyModel.Permissions.Contains(Permissions.USER_READ) ||
                    !apiKeyModel.Permissions.Contains(Permissions.USER_MAIL))
                    return CreateDataError("User Info is not permission");

                var userInfo = userBusiness.GetUserById(apiKeyModel.UserId);
                var status = userInfo == null ? Status.STATUS_ERROR : Status.STATUS_SUCCESS;


                if (userInfo != null)
                    return new ReturnObject
                    {
                        Status = status,
                        Data = new UserCurrent
                        {
                            Id = userInfo.Id,
                            Resource = "User",
                            FullName = userInfo.FullName,
                            AvatarUrl = userInfo.Avatar,
                            UserName = userInfo.Email,
                        }.ToJson()
                    }.ToJson();
                return CreateDataError("User Info is fail");
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                }.ToJson();
            }
        }

        /// <summary>
        /// CreateDataError
        /// </summary>
        /// <param name="message"></param>
        /// <returns>string</returns>
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