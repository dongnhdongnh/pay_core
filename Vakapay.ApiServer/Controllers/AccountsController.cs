using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Vakapay.ApiServer.ActionFilter;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.ClientRequest;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    [BaseActionFilter]
    public class AccountsController : Controller
    {
        private VakapayRepositoryMysqlPersistenceFactory VakapayRepositoryFactory { get; }

        public AccountsController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            VakapayRepositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
        }

        [HttpGet("GetBalance/{id}")]
        public ActionResult<string> GetBalance(string id)
        {
            try
            {
                var walletRepository = new WalletRepository(VakapayRepositoryFactory.GetOldConnection());
                var wallets = walletRepository.FindAllWalletByUserId(id);

                var balances = new List<CurrencyBalance>();

                foreach (var wallet in wallets)
                {
                    balances.Add(new CurrencyBalance()
                    {
                        Currency = wallet.Currency,
                        AmountDecimal = wallet.Balance
                    });
                }

                var balanceResponse = new GetBalanceResponse()
                {
                    Id = id,
                    Balance = balances
                };

                return new ReturnDataObject()
                    {Status = Status.STATUS_SUCCESS, Data = balanceResponse}.ToJson();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject()
                    {Status = Status.STATUS_ERROR, Message = e.Message}.ToJson();
            }
        }

        [HttpGet("{userId}/addresses")]
        public ActionResult<string> GetAddresses(string userId)
        {
            try
            {
                var walletRepository = new WalletRepository(VakapayRepositoryFactory.GetOldConnection());

                var addresses = walletRepository.GetAddressesByUserId(userId);

                if (addresses == null || addresses.Count == 0)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "No address found, userId is not existed!"
                    }.ToJson();
                }

                return new ReturnDataObject()
                    {Status = Status.STATUS_SUCCESS, Data = addresses}.ToJson();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject()
                    {Status = Status.STATUS_ERROR, Message = e.Message}.ToJson();
            }
        }

        [HttpGet("{userId}/addresses/{addressIdOrAddress}")]
        public ActionResult<string> ShowAddress(string userId, string addressIdOrAddress)
        {
            try
            {
                var walletRepository = new WalletRepository(VakapayRepositoryFactory.GetOldConnection());

                var addresses = walletRepository.GetAddressesByUserId(userId);

                if (addresses == null || addresses.Count == 0)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "No address found, userId is not existed!"
                    }.ToJson();
                }

                BlockchainAddress address = null;

                foreach (var blockchainAddress in addresses)
                {
                    if (blockchainAddress.Id == addressIdOrAddress || blockchainAddress.Address == addressIdOrAddress)
                    {
                        address = blockchainAddress;
                        break;
                    }
                }

                if (address == null)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "AddressId or regular blockchain address input is not found"
                    }.ToJson();
                }

                return new ReturnDataObject()
                    {Status = Status.STATUS_SUCCESS, Data = address}.ToJson();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject()
                    {Status = Status.STATUS_ERROR, Message = e.Message}.ToJson();
            }
        }

        [HttpGet("{userId}/addresses/{addressIdOrAddress}/transactions")]
        public ActionResult<string> GetAddressTransactions(string userId, string addressIdOrAddress)
        {
            try
            {
                var walletRepository = new WalletRepository(VakapayRepositoryFactory.GetOldConnection());

                var addresses = walletRepository.GetAddressesByUserId(userId);

                if (addresses == null || addresses.Count == 0)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "No address found, userId is not existed!"
                    }.ToJson();
                }

                BlockchainAddress address = null;

                foreach (var blockchainAddress in addresses)
                {
                    if (blockchainAddress.Id == addressIdOrAddress || blockchainAddress.Address == addressIdOrAddress)
                    {
                        address = blockchainAddress;
                        break;
                    }
                }

                if (address == null)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "AddressId or regular blockchain address input is not found"
                    }.ToJson();
                }

                return new ReturnDataObject()
                    {Status = Status.STATUS_SUCCESS, Data = address}.ToJson();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject()
                    {Status = Status.STATUS_ERROR, Message = e.Message}.ToJson();
            }
        }

//        [HttpPost("{userId}/coinbase_transactions")]
//        public ActionResult<string> SendTransactionsCoinbase(string userId, [FromBody] JObject value)
//        {
//            ReturnObject result = null;
//            try
//            {
//                var request = value.ToObject<UserSendTransaction>();
//                request.UserId = userId;
//                result = AddSendTransaction(request);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//                result = new ReturnObject()
//                    {Status = Status.STATUS_ERROR, Message = e.Message};
//            }
//
//            return result.ToJson();
//        }

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject("");
        }
    }
}