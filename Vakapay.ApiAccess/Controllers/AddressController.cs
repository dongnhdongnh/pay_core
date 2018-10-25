using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vakapay.ApiAccess.ActionFilter;
using Vakapay.ApiAccess.Constants;
using Vakapay.ApiServer.Helpers;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiAccess.Controllers
{
    [Route("v1/[controller]")]
    [BaseActionFilter]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private VakapayRepositoryMysqlPersistenceFactory VakapayRepositoryFactory { get; }

        public AddressController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
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
        [HttpGet("{idAddress}/{currency}")]
        public ActionResult<string> GetAddressInfoById(string idAddress, string currency)
        {
            try
            {
                if (string.IsNullOrEmpty(idAddress) || string.IsNullOrEmpty(currency) || !ApiAccessHelper.ValidateId(idAddress))
                {
                    return CreateDataError(MessageError.ParamInvalid);
                }

                BlockchainAddress blockChainAddress;
                switch (currency.ToUpper().Trim())
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
                        return CreateDataError(MessageError.ParamInvalid);
                }

                if (blockChainAddress?.WalletId == null) return CreateDataError(MessageError.DataNotFound);
                var userModel = (User) RouteData.Values["UserModel"];
                var walletBusiness = new WalletBusiness.WalletBusiness(VakapayRepositoryFactory);
                var walletModel = walletBusiness.GetWalletByID(blockChainAddress.WalletId);
                if (walletModel != null && string.Equals(walletModel.UserId, userModel.Id))
                {
                    return CreateDataSuccess(JsonConvert.SerializeObject(blockChainAddress));
                }

                return CreateDataError(MessageError.DataNotFound);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return CreateDataError(MessageError.DataNotFound);
            }
        }

        [HttpGet("list/{currency}")]
        public ActionResult<string> GetListAddress(string currency)
        {
            try
            {
                if (string.IsNullOrEmpty(currency))
                {
                    return CreateDataError(MessageError.ParamInvalid);
                }

                var userModel = (User) RouteData.Values["UserModel"];
                List<BlockchainAddress> listBlockChainAddress = null;
                switch (currency.ToUpper().Trim())
                {
                    case CryptoCurrency.BTC:
                        var bitCoinAddressRepository =
                            new BitcoinAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        var listBitCoinAddress =
                            bitCoinAddressRepository.FindByUserIdAndCurrency(userModel.Id, currency);
                        if (listBitCoinAddress != null)
                        {
                            listBlockChainAddress = listBitCoinAddress.Cast<BlockchainAddress>().ToList();
                        }

                        break;
                    case CryptoCurrency.ETH:
                        var ethereumAddressRepository =
                            new EthereumAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        var listEthereumAddress =
                            ethereumAddressRepository.FindByUserIdAndCurrency(userModel.Id, currency);
                        if (listEthereumAddress != null)
                        {
                            listBlockChainAddress = listEthereumAddress.Cast<BlockchainAddress>().ToList();
                        }

                        break;
                    case CryptoCurrency.VAKA:
                        var vaKaCoinAccountRepository =
                            new VakacoinAccountRepository(VakapayRepositoryFactory.GetOldConnection());
                        var listVaKaCoinAddress =
                            vaKaCoinAccountRepository.FindByUserIdAndCurrency(userModel.Id, currency);
                        if (listVaKaCoinAddress != null)
                        {
                            listBlockChainAddress = listVaKaCoinAddress.Cast<BlockchainAddress>().ToList();
                        }

                        break;
                    default:
                        return CreateDataError(MessageError.ParamInvalid);
                }

                return listBlockChainAddress != null
                    ? CreateDataSuccess(JsonConvert.SerializeObject(listBlockChainAddress))
                    : CreateDataError(MessageError.DataNotFound);
            }
            catch (Exception)
            {
                return CreateDataError(MessageError.DataNotFound);
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