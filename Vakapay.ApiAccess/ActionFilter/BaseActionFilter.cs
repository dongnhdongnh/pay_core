using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vakapay.ApiAccess.Constants;
using Vakapay.ApiAccess.Model;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiAccess.ActionFilter
{
    public class BaseActionFilter : ActionFilterAttribute
    {
        private readonly VakapayRepositoryMysqlPersistenceFactory _repositoryFactory;
        private const int ExpirationMinutes = 100 * 10 * 60 * 1000;

        public BaseActionFilter()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            _repositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutedContext)
        {
            try
            {
                var request = actionExecutedContext.HttpContext.Request;
                var headers = request.Headers;

                if (!headers.ContainsKey(Requests.HEADER_TOKEN_KEY))
                {
                    actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.TOKEN_INVALID));
                }
                else
                {
                    var filterModel = GetMessageTokenInvalid(headers, request.Path, _repositoryFactory);
                    if (filterModel.Status)
                    {
                        actionExecutedContext.RouteData.Values.Add("UserModel", filterModel.UserModel);
                        actionExecutedContext.RouteData.Values.Add("ApiKeyModel", filterModel.ApiKeyModel);
                    }
                    else
                    {
                        actionExecutedContext.Result = new JsonResult(CreateDataError(filterModel.Message));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.TOKEN_INVALID));
            }
        }

        /// <summary>
        /// Get message check token invalid
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="path"></param>
        /// <param name="repositoryFactory"></param>
        /// <returns></returns>
        private static FilterModel GetMessageTokenInvalid(IHeaderDictionary headers, string path,
            IVakapayRepositoryFactory repositoryFactory)
        {
            Console.WriteLine("GenerateTokenKey: " + CommonHelper.GenerateTokenKey("jm1ule1uvhp1s827",
                                  "f6lyadz0qb7pdf6rrfjocq7adv4e151m", "1540372506277", path));
            var filterModel = new FilterModel
            {
                Message = MessageError.TOKEN_INVALID,
                Status = false
            };
            string clientToken = headers[Requests.HEADER_TOKEN_KEY];
            var key = Encoding.UTF8.GetString(Convert.FromBase64String(clientToken));
            var parts = key.Split(new[] {':'});
            if (parts.Length != 3) return filterModel;
            var apiKey = parts[1];
            var timeStamp = parts[2];
            var userBusiness = new UserBusiness.UserBusiness(repositoryFactory);
            var apiKeyModel = userBusiness.GetApiKeyByKey(apiKey);
            if (apiKeyModel == null || string.IsNullOrEmpty(apiKeyModel.UserId)) return filterModel;
            var userModel = userBusiness.GetUserById(apiKeyModel.UserId);
            if (userModel == null)
            {
                filterModel.Message = MessageError.USER_NOT_EXIT;
                return filterModel;
            }

            var serverToken = CommonHelper.GenerateTokenKey(apiKeyModel.KeyApi, apiKeyModel.Secret, timeStamp, path);

            Console.WriteLine(serverToken);

            if (!IsTokenExpired(timeStamp))
            {
                if (!string.Equals(clientToken, serverToken)) return filterModel;
                filterModel.Message = null;
                filterModel.Status = true;
                filterModel.ApiKeyModel = apiKeyModel;
                filterModel.UserModel = userModel;
            }
            else
            {
                filterModel.Message = MessageError.TOKEN_EXPIDER;
            }

            return filterModel;
        }

        /// <summary>
        /// Check token is invalid (10 min)
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private static bool IsTokenExpired(string timeStamp)
        {
            var ticks = long.Parse(timeStamp);
            var serverCurrentTime = CommonHelper.GetUnixTimestamp();

            var expired = (serverCurrentTime - ticks) > ExpirationMinutes;
            return expired;
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