using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public ActionResult<ReturnObject> GetAddresses([FromQuery] string networkName)
        {
            try
            {
                if (!HelpersApi.ValidateCurrency(networkName))
                    return CreateDataErrorObject(MessageApiError.PARAM_INVALID);

                var userModel = (User) RouteData.Values["UserModel"];

                var wallet = _walletBusiness.FindByUserAndNetwork(userModel.Id, networkName);

                var listAddresses = _walletBusiness.GetAddressesFull(wallet.Id, networkName);

                return new ReturnObject()
                {
                    Status = Status.STATUS_COMPLETED,
                    Data = JsonHelper.SerializeObject(listAddresses)
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

        [HttpPost("create-addresses")]
        public ActionResult<ReturnObject> CreateAddresses([FromBody] JObject value)
        {
            try
            {
                if (!value.ContainsKey(ParseDataKeyApi.KEY_TOOLS_NETWORK))
                    return CreateDataErrorObject(MessageApiError.PARAM_INVALID);
                
                var networkName =  value[ParseDataKeyApi.KEY_TOOLS_NETWORK].ToString();
                
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