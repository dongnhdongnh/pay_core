using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vakapay.ApiAccess.ActionFilter;
using Vakapay.ApiAccess.Constants;
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
                var apiKeyModel = (ApiKey) RouteData.Values["ApiKeyModel"];

                if (string.IsNullOrEmpty(apiKeyModel.Permissions))
                    return CreateDataError("User Info is not permission");

                if (!apiKeyModel.Permissions.Contains(Permissions.USER_READ) ||
                    !apiKeyModel.Permissions.Contains(Permissions.USER_MAIL))
                    return CreateDataError("User Info is not permission");

                var userInfo = (User) RouteData.Values["UserModel"];

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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/scopes")]
        public ActionResult<string> GetScopes()
        {
            try
            {
                var apiKeyModel = (ApiKey) RouteData.Values["ApiKeyModel"];
                var arrPermission = apiKeyModel.Permissions.Split(",");
                var arrCurrency = apiKeyModel.Wallets.Split(",");
                var scopeModel = new ScopesModel();
                if (arrCurrency != null && arrCurrency.Length > 0)
                {
                    scopeModel.Currencys = arrCurrency.ToList();
                }

                if (arrPermission == null || arrPermission.Length <= 0)
                    return CreateDataSuccess(JsonHelper.SerializeObject(scopeModel));
                var listPermissions = new List<string>();
                foreach (var item in arrPermission)
                {
                    if (Models.Constants.listApiAccess.ContainsKey(item.Trim()))
                    {
                        listPermissions.Add(Models.Constants.listApiAccess[item.Trim()]);
                    }
                }
                scopeModel.Permissions = listPermissions;

                return CreateDataSuccess(JsonHelper.SerializeObject(scopeModel));
            }
            catch (Exception)
            {
                return CreateDataSuccess(JsonHelper.SerializeObject(MessageError.SCOPES_ERROR));
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

        /// <summary>
        /// CreateDataSuccess
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string CreateDataSuccess(string data)
        {
            return new ReturnObject
            {
                Status = Status.STATUS_SUCCESS,
                Message = null,
                Data = data
            }.ToJson();
        }
    }
}