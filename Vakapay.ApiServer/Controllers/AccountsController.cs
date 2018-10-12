using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Configuration;
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
                ConnectionString = VakapayConfiguration.DefaultSqlConnection
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

                return JsonHelper.SerializeObject(new ReturnObject()
                    {Status = Status.STATUS_SUCCESS, Data = JsonHelper.SerializeObject(balanceResponse)});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return JsonHelper.SerializeObject(new ReturnObject()
                    {Status = Status.STATUS_ERROR, Message = e.Message});
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

                if ( limit != null && limit > 0 )
                {
                    sortedTransactions = sortedTransactions.GetRange(0, (int) limit);
                }

                return JsonHelper.SerializeObject(new ReturnObject()
                    {Status = Status.STATUS_SUCCESS, Data = JsonHelper.SerializeObject(sortedTransactions)});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return JsonHelper.SerializeObject(new ReturnObject()
                    {Status = Status.STATUS_ERROR, Message = e.Message});
            }
        }
        
        

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject("");
        }
    }
}