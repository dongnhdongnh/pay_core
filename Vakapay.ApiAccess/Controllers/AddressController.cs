using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vakapay.ApiAccess.ActionFilter;
using Vakapay.ApiAccess.Constants;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiAccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private VakapayRepositoryMysqlPersistenceFactory VakapayRepositoryFactory { get; }

        public AddressController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            VakapayRepositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
        }

        /// <summary>
        /// GET https://api.vakapay.com/v1/address/"address_id"/"/network"
        /// SCOPES wallet:addresses:read
        /// </summary>
        /// <param name="idAddress"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [BaseActionFilter]
        [HttpGet("getAddress/{idAddress}/{currency}")]
        public ActionResult<string> GetAddressInfoById(string idAddress, string currency)
        {
            try
            {
                if (string.IsNullOrEmpty(idAddress) || string.IsNullOrEmpty(currency))
                {
                    return CreateDataError(MessageError.PARAM_INVALID);
                }

                BlockchainAddress blockChainAddress;
                switch (currency)
                {
                    case CryptoCurrency.BTC:
                        var bitCoinAddressRepository =
                            new BitcoinAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = bitCoinAddressRepository.FindByAddress(idAddress);
                        break;
                    case CryptoCurrency.ETH:
                        var ethereumAddressRepository =
                            new EthereumAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = ethereumAddressRepository.FindByAddress(idAddress);
                        break;
                    case CryptoCurrency.VAKA:
                        var vaKaCoinAccountRepository =
                            new VakacoinAccountRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = vaKaCoinAccountRepository.FindByAddress(idAddress);
                        break;
                    default:
                        return CreateDataError(MessageError.PARAM_INVALID);
                }

                return blockChainAddress != null
                    ? CreateDataSuccess(JsonConvert.SerializeObject(blockChainAddress))
                    : CreateDataError(MessageError.DATA_NOT_FOUND);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return CreateDataError(MessageError.DATA_NOT_FOUND);
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

        /// <summary>
        /// CreateDataSuccess
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string CreateDataSuccess(string data)
        {
            return new ReturnObject
            {
                Status = Status.STATUS_SUCCESS,
                Message = null,
                Data = data
            }.ToJson();
        }
    }
}