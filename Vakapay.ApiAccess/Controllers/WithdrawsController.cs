using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vakapay.ApiAccess.ActionFilter;
using Vakapay.ApiAccess.Constants;
using Vakapay.ApiAccess.Model;
using Vakapay.ApiServer.Helpers;
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

        [HttpGet("get-withdraw/{currency}/{id}")]
        //wallet:withdrawals:read
        public ActionResult<string> GetWithdraw(string currency, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(currency) || string.IsNullOrEmpty(id))
                {
                    return ApiAccessHelper.CreateDataError(MessageError.PARAM_INVALID);
                }

                if (!ApiAccessHelper.ValidateId(id))
                    return ApiAccessHelper.CreateDataError(MessageError.PARAM_INVALID);

                var apiKeyModel = (ApiKey) RouteData.Values["ApiKeyModel"];

                if (string.IsNullOrEmpty(apiKeyModel.Permissions))
                    return ApiAccessHelper.CreateDataError(MessageError.USER_PERMISSION);

                if (!apiKeyModel.Permissions.Contains(Permissions.READ_TRANSACTIONS))
                    return ApiAccessHelper.CreateDataError(MessageError.USER_PERMISSION);

                var userInfo = (User) RouteData.Values["UserModel"];

                var dataWithdraw = userBusiness.GetWithdraw(id, currency);

                if (dataWithdraw != null)
                {
                    if (!userInfo.Id.Equals(dataWithdraw.UserId))
                    {
                        return ApiAccessHelper.CreateDataError(MessageError.DATA_NOT_FOUND);
                    }
                }
                else
                {
                    return ApiAccessHelper.CreateDataError(MessageError.DATA_NOT_FOUND);
                }

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(dataWithdraw)
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

        [HttpGet("list-withdraws/{currency}/{offset:int?}/{limit:int?}")]
        //wallet:withdrawals:read
        public ActionResult<string> GetListWithdraws(string currency, int offset = 0, int limit = 8)
        {
            try
            {
                if (string.IsNullOrEmpty(currency))
                {
                    return ApiAccessHelper.CreateDataError(MessageError.PARAM_INVALID);
                }

                var apiKeyModel = (ApiKey) RouteData.Values["ApiKeyModel"];

                if (string.IsNullOrEmpty(apiKeyModel.Permissions))
                    return ApiAccessHelper.CreateDataError(MessageError.USER_PERMISSION);

                if (!apiKeyModel.Permissions.Contains(Permissions.READ_TRANSACTIONS))
                    return ApiAccessHelper.CreateDataError(MessageError.USER_PERMISSION);

                int numberData = 0;
                var withdraws = walletBusiness.GetHistory(out numberData, apiKeyModel.UserId, currency,
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
    }
}