using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Vakapay.ApiAccess.Constants;
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

        public BaseActionFilter()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            _repositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            try
            {
                var headers = actionExecutedContext.HttpContext.Request.Headers;
                Console.WriteLine(JsonConvert.SerializeObject(headers));
                if (!headers.ContainsKey(Requests.HeaderApiKey))
                {
                    actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.ApiKeyInvalid));
                }
                else if (!headers.ContainsKey(Requests.HeaderApiSecret))
                {
                    actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.ApiSecretInvalid));
                }
                else
                {
                    var userBusiness = new UserBusiness.UserBusiness(_repositoryFactory);
                    string apiKey = headers[Requests.HeaderApiKey];
                    string apiSecret = headers[Requests.HeaderApiSecret];
                    var apiKeyModel = userBusiness.GetApiKeyByKey(apiKey, apiSecret);
                    if (string.Equals(apiKeyModel.KeyApi, apiKey) && string.Equals(apiKeyModel.Secret, apiSecret))
                    {
                        base.OnActionExecuted(actionExecutedContext);
                    }
                    else
                    {
                        actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.HeaderApiInvalid));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.HeaderApiInvalid));
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