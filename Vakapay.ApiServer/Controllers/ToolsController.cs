using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Vakapay.ApiServer.ActionFilter;
using Vakapay.ApiServer.Helpers;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    [BaseActionFilter]
    public class ToolsController : ControllerBase
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private readonly WalletBusiness.WalletBusiness _walletBusiness;
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory { get; }


        private IConfiguration Configuration { get; }

        public ToolsController(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        )
        {
            Configuration = configuration;

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            _userBusiness = new UserBusiness.UserBusiness(PersistenceFactory);

            _walletBusiness =
                new WalletBusiness.WalletBusiness(PersistenceFactory);
        }


        [HttpGet("get-addresses")]
        public string GetAddresses()
        {
            try
            {
                var queryStringValue = Request.Query;


                if (!queryStringValue.ContainsKey("offset") || !queryStringValue.ContainsKey("networkName") ||
                    !queryStringValue.ContainsKey("limit"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                StringValues sort;
                StringValues filter;
                StringValues networkName;
                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_OFFSET, out var offset);
                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_LIMIT, out var limit);
                if (queryStringValue.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_FILTER))
                    queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_FILTER, out filter);
                if (queryStringValue.ContainsKey(ParseDataKeyApi.KEY_PASS_DATA_GET_SORT))
                    queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_SORT, out sort);

                queryStringValue.TryGetValue(ParseDataKeyApi.KEY_PASS_DATA_GET_NETWORK, out networkName);
                //sort = ConvertSortLog(sort);

                if (!HelpersApi.ValidateCurrency(networkName))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var userModel = (User) RouteData.Values["UserModel"];

                var wallet = _walletBusiness.FindByUserAndNetwork(userModel.Id, networkName);
                int numberData;
                var listAddresses = _walletBusiness.GetAddressesFull(out numberData, wallet.Id, networkName,
                    Convert.ToInt32(offset),
                    Convert.ToInt32(limit), filter.ToString(), sort);

                return new ReturnObject()
                {
                    Status = Status.STATUS_COMPLETED,
                    Data = new ResultList<BlockchainAddress>
                    {
                        List = listAddresses,
                        Total = numberData
                    }.ToJson()
                }.ToJson();
            }
            catch (Exception e)
            {
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                }.ToJson();
            }

        }

        [HttpPost("update-addresses-info")]
        public string UpdateLabel([FromBody] JObject value)
        {
            try
            {
                if (!value.ContainsKey("id") || !value.ContainsKey("networkName"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var id = value["id"].ToString();
                var networkName = value["networkName"].ToString();

                if (!HelpersApi.ValidateCurrency(networkName))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                if (!CommonHelper.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                if (value.ContainsKey("label"))
                {
                    var label = value["label"].ToString();
                    var result = _walletBusiness.UpdateAddress(id, networkName, label);

                    if (result.Status == Status.STATUS_SUCCESS)

                        return new ReturnObject
                        {
                            Status = Status.STATUS_SUCCESS
                        }.ToJson();
                }


                return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);
            }
            catch (Exception e)
            {
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                }.ToJson();
            }
        }

        [HttpPost("get-addresses-info")]
        public string GetAddressesInfo([FromBody] JObject value)
        {
            try
            {
                if (!value.ContainsKey("id") || !value.ContainsKey("networkName"))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var id = value["id"].ToString();
                var networkName = value["networkName"].ToString();

                if (!HelpersApi.ValidateCurrency(networkName))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                if (!CommonHelper.ValidateId(id))
                    return HelpersApi.CreateDataError(MessageApiError.PARAM_INVALID);

                var address = _walletBusiness.GetAddressesInfo(id, networkName);

                if (address != null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonHelper.SerializeObject(address)
                    }.ToJson();
                }

                return HelpersApi.CreateDataError(MessageApiError.DATA_NOT_FOUND);
            }
            catch (Exception e)
            {
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                }.ToJson();
            }

            //  return null;
        }

        [HttpPost("create-addresses")]
        public ActionResult<ReturnObject> CreateAddresses([FromBody] JObject value)
        {
            try
            {
                if (!value.ContainsKey(ParseDataKeyApi.KEY_TOOLS_NETWORK))
                    return CreateDataErrorObject(MessageApiError.PARAM_INVALID);

                var networkName = value[ParseDataKeyApi.KEY_TOOLS_NETWORK].ToString();

                if (!HelpersApi.ValidateCurrency(networkName))
                    return CreateDataErrorObject(MessageApiError.PARAM_INVALID);

                var userModel = (User) RouteData.Values["UserModel"];

                var wallet = _walletBusiness.FindByUserAndNetwork(userModel.Id, networkName);

                if (wallet == null)
                    return CreateDataErrorObject("Wallet is null");

                var address = _walletBusiness.CreateAddressForWallet(wallet);

                return new ReturnObject()
                {
                    Status = Status.STATUS_COMPLETED,
                    Data = JsonHelper.SerializeObject(address)
                };
            }
            catch (Exception e)
            {
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }

            //  return null;
        }

        public static ReturnObject CreateDataErrorObject(string message)
        {
            return new ReturnObject
            {
                Status = Status.STATUS_ERROR,
                Message = message
            };
        }
    }
}