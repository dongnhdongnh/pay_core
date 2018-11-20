using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;

namespace Vakapay.ApiServer.Controllers
{
    public abstract class CustomController : Controller
    {
        protected IConfiguration Configuration { get; }
        public IVakapayRepositoryFactory _repositoryFactory;
        protected IHostingEnvironment _env;


        protected CustomController(
            IVakapayRepositoryFactory persistenceFactory,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment)
        {
            _repositoryFactory = persistenceFactory;
            _env = hostingEnvironment;
            Configuration = configuration;
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