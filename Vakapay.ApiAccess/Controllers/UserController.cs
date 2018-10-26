using System;
using Microsoft.AspNetCore.Mvc;
using Vakapay.ApiAccess.ActionFilter;
using Vakapay.ApiAccess.Model;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
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
        private UserBusiness.UserBusiness UserBusiness { get; }

        public UserController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            VakapayRepositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            UserBusiness = new UserBusiness.UserBusiness(VakapayRepositoryFactory);
        }

        [HttpGet("user")]
        //wallet:user:read
        //wallet:user:email
        public ActionResult<string> GetCurrentUser()
        {
            try
            {
                var apiKeyModel = (ApiKey)RouteData.Values["ApiKeyModel"];

                if (string.IsNullOrEmpty(apiKeyModel.Permissions))
                    return CreateDataError("User Info is not permission");

                if (!apiKeyModel.Permissions.Contains(Permissions.USER_READ) ||
                    !apiKeyModel.Permissions.Contains(Permissions.USER_MAIL))
                    return CreateDataError("User Info is not permission");

                var userInfo = (User)RouteData.Values["UserModel"];

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = new UserCurrent
                    {
                        Id = userInfo.Id,
                        Resource = "User",
                        FullName = userInfo.FullName,
                        AvatarUrl = userInfo.Avatar,
                        UserName = userInfo.Email,
                    }.ToJson()
                }.ToJson();
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