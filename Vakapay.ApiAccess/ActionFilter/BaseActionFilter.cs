using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vakapay.ApiAccess.Constants;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;

namespace Vakapay.ApiAccess.ActionFilter
{
    public class BaseActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            if (!IsValidApiKey(actionExecutedContext.HttpContext.Response.Headers))
            {
                actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.ApiKeyInvalid));
            }
            else if (IsValidApiSecret(actionExecutedContext.HttpContext.Response.Headers))
            {
                actionExecutedContext.Result = new JsonResult(CreateDataError(MessageError.ApiSecretInvalid));
            }
            else
            {
                base.OnActionExecuted(actionExecutedContext);
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

        private static bool IsValidApiKey(IHeaderDictionary headers)
        {
            
            if (headers.ContainsKey(Requests.HeaderApiKey))
            {
                //check
            }
            else
            {
                return false;
            }

            return false;
        }

        private static bool IsValidApiSecret(IHeaderDictionary headers)
        {
            return false;
        }
    }
}