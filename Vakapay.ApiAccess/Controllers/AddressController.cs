using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vakapay.ApiAccess.ActionFilter;
using Vakapay.ApiAccess.Constants;
using Vakapay.ApiAccess.Model;
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
        /// Permission: wallet:addresses:read
        /// </summary>
        /// <param name="idAddress"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpGet("{idAddress}/{currency}")]
        public ActionResult<string> GetAddressInfoById(string idAddress, string currency)
        {
            try
            {
                var checkIdAddress = CheckIdAddress(idAddress);
                if (!string.IsNullOrEmpty(checkIdAddress))
                {
                    return CreateDataError(checkIdAddress);
                }

                var apiKey = (ApiKey) RouteData.Values[Requests.KEY_PASS_DATA_API_KEY_MODEL];
                var checkCurrency = CheckCurrency(currency, apiKey);
                if (!string.IsNullOrEmpty(checkCurrency))
                {
                    return checkCurrency;
                }

                if (!apiKey.Permissions.Contains(Permissions.READ_ADDRESSES))
                {
                    return CreateDataError(MessageError.READ_ADDRESS_NOT_PERMISSION);
                }

                BlockchainAddress blockChainAddress;
                switch (currency)
                {
                    case CryptoCurrency.BTC:
                        var bitCoinAddressRepository =
                            new BitcoinAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = bitCoinAddressRepository.FindById(idAddress);
                        break;
                    case CryptoCurrency.ETH:
                        var ethereumAddressRepository =
                            new EthereumAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = ethereumAddressRepository.FindById(idAddress);
                        break;
                    case CryptoCurrency.VAKA:
                        var vaKaCoinAccountRepository =
                            new VakacoinAccountRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = vaKaCoinAccountRepository.FindById(idAddress);
                        break;
                    default:
                        return CreateDataError(MessageError.PARAM_INVALID);
                }

                if (blockChainAddress?.WalletId == null) return CreateDataError(MessageError.DATA_NOT_FOUND);
                var userModel = (User) RouteData.Values[Requests.KEY_PASS_DATA_USER_MODEL];
                var walletBusiness = new WalletBusiness.WalletBusiness(VakapayRepositoryFactory);
                var walletModel = walletBusiness.GetWalletByID(blockChainAddress.WalletId);
                if (walletModel != null && string.Equals(walletModel.UserId, userModel.Id))
                {
                    return CreateDataSuccess(JsonConvert.SerializeObject(blockChainAddress));
                }

                return CreateDataError(MessageError.DATA_NOT_FOUND);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return CreateDataError(MessageError.DATA_NOT_FOUND);
            }
        }

        /// <summary>
        /// Get all list address by user
        /// https://192.168.1.185:5004/v1/address/list/BTC (ETH, VAKA)
        /// permission: wallet:addresses:read
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpGet("list/{currency}")]
        public ActionResult<string> GetListAddress(string currency)
        {
            try
            {
                var apiKey = (ApiKey) RouteData.Values[Requests.KEY_PASS_DATA_API_KEY_MODEL];
                var checkCurrency = CheckCurrency(currency, apiKey);
                if (!string.IsNullOrEmpty(checkCurrency))
                {
                    return checkCurrency;
                }

                if (!apiKey.Permissions.Contains(Permissions.READ_ADDRESSES))
                {
                    return CreateDataError(MessageError.READ_ADDRESS_NOT_PERMISSION);
                }

                var userModel = (User) RouteData.Values[Requests.KEY_PASS_DATA_USER_MODEL];
                List<BlockchainAddress> listBlockChainAddress = null;
                switch (currency)
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
                        return CreateDataError(MessageError.PARAM_INVALID);
                }

                return listBlockChainAddress != null
                    ? CreateDataSuccess(JsonConvert.SerializeObject(listBlockChainAddress))
                    : CreateDataError(MessageError.DATA_NOT_FOUND);
            }
            catch (Exception)
            {
                return CreateDataError(MessageError.DATA_NOT_FOUND);
            }
        }

        /// <summary>
        /// GetTransactionByAddress
        /// https://192.168.1.185:5004/v1/address/transactions/311a02f6-6103-4078-8c55-a8f5157e0414/Withdraw/BTC
        /// Permission: wallet:transactions:read
        /// </summary>
        /// <param name="idAddress"></param>
        /// <param name="currency"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("transactions/{idAddress}/{type}/{currency}")]
        public ActionResult<string> GetTransactionByAddress(string idAddress, string currency, string type)
        {
            try
            {
                var checkIdAddress = CheckIdAddress(idAddress);
                if (!string.IsNullOrEmpty(checkIdAddress))
                {
                    return CreateDataError(checkIdAddress);
                }

                var apiKey = (ApiKey) RouteData.Values[Requests.KEY_PASS_DATA_API_KEY_MODEL];
                var checkCurrency = CheckCurrency(currency, apiKey);
                if (!string.IsNullOrEmpty(checkCurrency))
                {
                    return checkCurrency;
                }

                if (!apiKey.Permissions.Contains(Permissions.READ_TRANSACTIONS))
                {
                    return CreateDataError(MessageError.READ_TRANSACION_NOT_PERMISSION);
                }

                List<BlockchainTransaction> listTransaction = null;
                switch (currency)
                {
                    case CryptoCurrency.BTC:
                        var btcAddressRepository =
                            new BitcoinAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        var btcAddress = btcAddressRepository.FindById(idAddress);
                        if (btcAddress?.Id != null)
                            if (string.Equals(type, Requests.TYPE_DEPOSIT))
                            {
                                var btcDepositRepository = new BitcoinDepositTransactionRepository(
                                    VakapayRepositoryFactory.GetOldConnection());
                                listTransaction = btcDepositRepository.FindTransactionsToAddress(btcAddress.Address);
                            }
                            else if (string.Equals(type, Requests.TYPE_WITH_DRAW))
                            {
                                var btcWithdrawRepository =
                                    new BitcoinWithdrawTransactionRepository(
                                        VakapayRepositoryFactory.GetOldConnection());
                                listTransaction = btcWithdrawRepository.FindTransactionsFromAddress(btcAddress.Address);
                            }

                        break;
                    case CryptoCurrency.ETH:
                        var ethAddressRepository =
                            new BitcoinAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        var ethAddress = ethAddressRepository.FindById(idAddress);
                        if (ethAddress?.Id != null)
                            if (string.Equals(type, Requests.TYPE_DEPOSIT))
                            {
                                var ethDepositRepository = new EthereumDepositTransactionRepository(
                                    VakapayRepositoryFactory.GetOldConnection());
                                listTransaction =
                                    ethDepositRepository.FindTransactionsToAddress(ethAddress.Address);
                            }
                            else if (string.Equals(type, Requests.TYPE_WITH_DRAW))
                            {
                                var ethWithdrawRepository =
                                    new EthereumWithdrawnTransactionRepository(
                                        VakapayRepositoryFactory.GetOldConnection());
                                listTransaction = ethWithdrawRepository
                                    .FindTransactionsFromAddress(ethAddress.Address);
                            }

                        break;
                    case CryptoCurrency.VAKA:
                        var vakaAddressRepository =
                            new BitcoinAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        var vakaAddress = vakaAddressRepository.FindById(idAddress);
                        if (vakaAddress?.Id != null)
                            if (string.Equals(type, Requests.TYPE_DEPOSIT))
                            {
                                var vakaDepositRepository = new VakacoinDepositTransactionRepository(
                                    VakapayRepositoryFactory.GetOldConnection());
                                listTransaction =
                                    vakaDepositRepository.FindTransactionsToAddress(vakaAddress.Address);
                            }
                            else if (string.Equals(type, Requests.TYPE_WITH_DRAW))
                            {
                                var vakaWithdrawRepository =
                                    new VakacoinWithdrawTransactionRepository(
                                        VakapayRepositoryFactory.GetOldConnection());
                                listTransaction = vakaWithdrawRepository
                                    .FindTransactionsFromAddress(vakaAddress.Address);
                            }

                        break;
                    default:
                        return CreateDataError(MessageError.PARAM_INVALID);
                }

                return listTransaction != null
                    ? CreateDataSuccess(JsonConvert.SerializeObject(listTransaction))
                    : CreateDataError(MessageError.DATA_NOT_FOUND);
            }
            catch (Exception)
            {
                return CreateDataError(MessageError.DATA_NOT_FOUND);
            }
        }

        /// <summary>
        /// https://192.168.1.185:5004/v1/address/create/BTC
        /// Permission: wallet:addresses:create
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpPost("create/{currency}")]
        public async Task<ActionResult<string>> CreateAddress(string currency)
        {
            try
            {
                var apiKey = (ApiKey) RouteData.Values[Requests.KEY_PASS_DATA_API_KEY_MODEL];
                var checkCurrency = CheckCurrency(currency, apiKey);
                if (!string.IsNullOrEmpty(checkCurrency))
                {
                    return checkCurrency;
                }

                if (!apiKey.Permissions.Contains(Permissions.CREATED_ADDRESSES))
                {
                    return CreateDataError(MessageError.CREATE_TRANSACION_NOT_PERMISSION);
                }

                var userModel = (User) RouteData.Values[Requests.KEY_PASS_DATA_USER_MODEL];
                var walletBusiness = new WalletBusiness.WalletBusiness(VakapayRepositoryFactory);
                var wallet = walletBusiness.FindByUserAndNetwork(userModel.Id, currency);
                if (wallet == null) return CreateDataError(MessageError.CREATE_ADDRESS_FAIL);
                var result = await walletBusiness.CreateAddressAsync(wallet);
                if (result.Status != Status.STATUS_SUCCESS)
                    return CreateDataError(MessageError.CREATE_ADDRESS_FAIL);
                BlockchainAddress blockChainAddress;
                switch (currency)
                {
                    case CryptoCurrency.BTC:
                        var bitCoinAddressRepository =
                            new BitcoinAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = bitCoinAddressRepository.FindByAddress(result.Data);
                        break;
                    case CryptoCurrency.ETH:
                        var ethereumAddressRepository =
                            new EthereumAddressRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = ethereumAddressRepository.FindById(result.Data);
                        break;
                    case CryptoCurrency.VAKA:
                        var vaKaCoinAccountRepository =
                            new VakacoinAccountRepository(VakapayRepositoryFactory.GetOldConnection());
                        blockChainAddress = vaKaCoinAccountRepository.FindById(result.Data);
                        break;
                    default:
                        return CreateDataError(MessageError.CREATE_ADDRESS_FAIL);
                }

                return blockChainAddress != null
                    ? CreateDataSuccess(JsonConvert.SerializeObject(blockChainAddress))
                    : CreateDataError(MessageError.CREATE_ADDRESS_FAIL);
            }
            catch (Exception)
            {
                return CreateDataError(MessageError.CREATE_ADDRESS_FAIL);
            }
        }

        private string CheckCurrency(string currency, ApiKey apiKey)
        {
            if (string.IsNullOrEmpty(currency) || !CryptoCurrency.AllNetwork.Contains(currency))
            {
                return CreateDataError(MessageError.PARAM_INVALID);
            }

            if (apiKey.Wallets == null || !apiKey.Wallets.Contains(currency))
            {
                return CreateDataError(MessageError.CURRENCY_NOT_PERMISSION);
            }

            return null;
        }

        private string CheckIdAddress(string idAddress)
        {
            if (string.IsNullOrEmpty(idAddress) || !CommonHelper.ValidateId(idAddress))
            {
                return CreateDataError(MessageError.ADDRESS_INVALID);
            }

            return null;
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