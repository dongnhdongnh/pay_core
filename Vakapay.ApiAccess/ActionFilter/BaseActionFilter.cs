using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
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
        private const int ExpirationMinutes = 10*60*1000;

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
                var request = actionExecutedContext.HttpContext.Request;
                var headers = request.Headers;
//                Console.WriteLine(JsonConvert.SerializeObject(headers));
//                Console.WriteLine("GenerateTokenKey: " + GenerateTokenKey("wk961j2jewxaz0zy", "cjon3tnigdvuosipgxm1hlu3fd6umtbm",
//                                      "1540363139699",
//                                      actionExecutedContext.HttpContext.Request.Path));
                if (!headers.ContainsKey(Requests.HeaderTokenKey))
                {
                    actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.TokenInvalid));
                }
                else
                {
                    var message = GetMessageTokenInvalid(headers, request.Path, _repositoryFactory);
                    if (string.IsNullOrEmpty(message))
                    {
                        base.OnActionExecuted(actionExecutedContext);
                        return;
                    }

                    actionExecutedContext.Result = new JsonResult(CreateDataError(message));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.TokenInvalid));
            }
        }

        /// <summary>
        /// Get message check token invalid
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="path"></param>
        /// <param name="repositoryFactory"></param>
        /// <returns></returns>
        private static string GetMessageTokenInvalid(IHeaderDictionary headers, string path,
            IVakapayRepositoryFactory repositoryFactory)
        {
            var message = MessageError.TokenInvalid;
            string clientToken = headers[Requests.HeaderTokenKey];
            var key = Encoding.UTF8.GetString(Convert.FromBase64String(clientToken));
            var parts = key.Split(new[] {':'});
            if (parts.Length != 3) return message;
            var apiKey = parts[1];
            var timeStamp = parts[2];
            var userBusiness = new UserBusiness.UserBusiness(repositoryFactory);
            var apiKeyModel = userBusiness.GetApiKeyByKey(apiKey);
            if (apiKeyModel == null || !string.Equals(apiKeyModel.KeyApi, apiKey)) return message;
            var serverToken = GenerateTokenKey(apiKeyModel.KeyApi, apiKeyModel.Secret,
                timeStamp, path);
            if (!IsTokenExpired(timeStamp))
            {
                if (string.Equals(clientToken, serverToken))
                {
                    message = null;
                }
            }
            else
            {
                message = MessageError.TokenExpider;
            }

            return message;
        }

        /// <summary>
        /// Check token is invalid (10 min)
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private static bool IsTokenExpired(string timeStamp)
        {
            var ticks = long.Parse("1540362485441");
            var serverCurrentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var expired = (serverCurrentTime - ticks) > ExpirationMinutes;
            return expired;
        }

        /// <summary>
        /// Generate Token Key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiSecret"></param>
        /// <param name="timeStamp"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GenerateTokenKey(string apiKey, string apiSecret, string timeStamp, string path)
        {
            try
            {
                string hashLeft;
                string hashRight;
                var key = string.Join(":", apiSecret, apiKey, timeStamp, path);
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(string.Join(":", apiSecret, key)));
                    hashLeft = Convert.ToBase64String(hmac.Hash);
                    hashRight = string.Join(":", apiKey, timeStamp);
                }

                return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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