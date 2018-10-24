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
    public class WithdrawsController : ControllerBase
    {
        private VakapayRepositoryMysqlPersistenceFactory VakapayRepositoryFactory { get; }
        private UserBusiness.UserBusiness userBusiness { get; }
        private WalletBusiness.WalletBusiness walletBusiness { get; }

        public WithdrawsController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            VakapayRepositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            userBusiness = new UserBusiness.UserBusiness(VakapayRepositoryFactory);
            walletBusiness =
                new WalletBusiness.WalletBusiness(VakapayRepositoryFactory);
        }

        [HttpGet("list-withdraws/{currency}/{offset:int?}/{limit:int?}")]
        //wallet:user:read
        //wallet:user:email
        public ActionResult<string> GetListWithdraws(string currency, int offset = 0, int limit = 8)
        {
            try
            {
                if (string.IsNullOrEmpty(currency))
                {
                    return CreateDataError(MessageError.ParamInvalid);
                }


                var headers = Request.Headers;
                string token = headers[Requests.HeaderTokenKey];
                var key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = key.Split(new[] {':'});
                if (parts.Length != 3)
                    return CreateDataError(MessageError.TokenInvalid);
                var apiKey = parts[1];

                var apiKeyModel = userBusiness.GetApiKeyByKey(apiKey);

                if (string.IsNullOrEmpty(apiKeyModel.Permissions))
                    return CreateDataError(MessageError.UserPermissions);

                if (!apiKeyModel.Permissions.Contains(Permissions.USER_READ) ||
                    !apiKeyModel.Permissions.Contains(Permissions.USER_MAIL))
                    return CreateDataError(MessageError.UserPermissions);

                var userInfo = userBusiness.GetUserById(apiKeyModel.UserId);

                int numberData = 0;
                var withdraws = walletBusiness.GetHistory(out numberData, userInfo.Id, currency,
                    offset, limit);

                var data = new ListWithdraws
                {
                    total = numberData,
                    listWithdraws = withdraws
                };

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(data)
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