using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiServer.ActionFilter
{
    public class BaseActionFilter : ActionFilterAttribute
    {
        private readonly VakapayRepositoryMysqlPersistenceFactory _repositoryFactory;

        public BaseActionFilter()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            _repositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutedContext)
        {
            try
            {
                var email = actionExecutedContext.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email)
                    .Select(c => c.Value).SingleOrDefault();

                email = "ngochuan2212@gmail.com";

                if (!string.IsNullOrEmpty(email))
                {
                    var query = new Dictionary<string, string> {{"Email", email}};
                    var userBusiness = new UserBusiness.UserBusiness(_repositoryFactory);
                    var userModel = userBusiness.GetUserInfo(query);

                    if (userModel != null)
                    {
                        actionExecutedContext.RouteData.Values.Add("UserModel", userModel);
                        return;
                    }
                }


                actionExecutedContext.Result = new JsonResult(CreateDataError(MessageApiError.USER_NOT_EXIT));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                actionExecutedContext.Result = new JsonResult(CreateDataError(MessageApiError.USER_NOT_EXIT));
            }
        }


        /// <summary>
        /// CreateDataError
        /// </summary>
        /// <param name="message"></param>
        /// <returns>string</returns>
        private static ReturnObject CreateDataError(string message)
        {
            return new ReturnObject
            {
                Status = Status.STATUS_ERROR,
                Message = message
            };
        }
    }
}