using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.ClientRequest;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiServer.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        private VakapayRepositoryMysqlPersistenceFactory VakapayRepositoryFactory { get; }
        public AccountsController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            VakapayRepositoryFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
        }

        [HttpGet("GetBalance/{id}")]
        public ActionResult<string> GetBalance(string id)
        {
            try
            {

                var walletRepository = new WalletRepository(VakapayRepositoryFactory.GetOldConnection());
                var userRepository = new UserRepository(VakapayRepositoryFactory.GetOldConnection());
                var wallets = walletRepository.FindAllWalletByUserId(id);
                var user = userRepository.FindById(id);

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
                    if (blockchainAddress.Id == addressIdOrAddress || blockchainAddress.GetAddress() == addressIdOrAddress)
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
                    if (blockchainAddress.Id == addressIdOrAddress || blockchainAddress.GetAddress() == addressIdOrAddress)
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

        [HttpPost("{userId}/transactions")]
        public ActionResult<string> SendTransactions(string userId, [FromBody] JObject value)
        {
            try
            {
                var walletRepository = new WalletRepository(VakapayRepositoryFactory.GetOldConnection());

//                try
//                {
//                    var to = value["to"].ToString();
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine(e);
//                    return new ReturnObject()
//                        {Status = Status.STATUS_ERROR, Message = "Recipient not exist"}.ToJson();
//                }

                var request = value.ToObject<SendCoinRequest>();

                return new ReturnDataObject()
                    {Status = Status.STATUS_SUCCESS, Data = request}.ToJson();
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject()
                    {Status = Status.STATUS_ERROR, Message = e.Message}.ToJson();
            }
        }

        [HttpGet]
        [Route("{userId}/transactions/{limit:int?}")]
        public ActionResult<string> GetTransactions(string userId, int? limit = null)
        {
            try
            {
                var bitcoinDepositTrxRepo =
                    new BitcoinDepositTransactionRepository(VakapayRepositoryFactory.GetOldConnection());
                var bitcoinWithdrawTrxRepo =
                    new BitcoinWithdrawTransactionRepository(VakapayRepositoryFactory.GetOldConnection());
                var ethereumDepositTrxRepo =
                    new EthereumDepositTransactionRepository(VakapayRepositoryFactory.GetOldConnection());
                var ethereumWithdrawTrxRepo =
                    new EthereumWithdrawnTransactionRepository(VakapayRepositoryFactory.GetOldConnection());
                var vakacoinDepositTrxRepo =
                    new VakacoinDepositTransactionRepository(VakapayRepositoryFactory.GetOldConnection());
                var vakacoinWithdrawTrxRepo =
                    new VakacoinWithdrawTransactionRepository(VakapayRepositoryFactory.GetOldConnection());

                var transactions = new List<BlockchainTransaction>();

                transactions.AddRange(bitcoinDepositTrxRepo.FindTransactionsByUserId(userId));
                transactions.AddRange(bitcoinWithdrawTrxRepo.FindTransactionsByUserId(userId));
                transactions.AddRange(ethereumDepositTrxRepo.FindTransactionsByUserId(userId));
                transactions.AddRange(ethereumWithdrawTrxRepo.FindTransactionsByUserId(userId));
                transactions.AddRange(vakacoinDepositTrxRepo.FindTransactionsByUserId(userId));
                transactions.AddRange(vakacoinWithdrawTrxRepo.FindTransactionsByUserId(userId));

                var sortedTransactions = transactions.OrderByDescending(o=>o.UpdatedAt).ToList();

                if ( limit != null && limit > 0 && limit < sortedTransactions.Count )
                {
                    sortedTransactions = sortedTransactions.GetRange(0, (int) limit);
                }

                return new ReturnDataObject()
                    {Status = Status.STATUS_SUCCESS, Data = sortedTransactions}.ToJson();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject()
                    {Status = Status.STATUS_ERROR, Message = e.Message}.ToJson();
            }
        }

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject("");
        }
    }
}