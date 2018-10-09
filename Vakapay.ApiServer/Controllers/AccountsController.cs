using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vakapay.ApiServer.Models;
using Vakapay.Configuration;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
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
                        NetworkName = wallet.NetworkName,
                        AmountDecimal = wallet.Balance
                    });
                }

                var balanceResponse = new GetBalanceResponse()
                {
                    Id = id,
                    Balance = balances
                };

                return JsonHelper.SerializeObject(new ReturnDataObject()
                    {Status = Status.StatusSuccess, Message = Message.MessageSuccess, Data = balanceResponse});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return JsonHelper.SerializeObject(new ReturnObject()
                    {Status = Status.StatusError, Message = e.Message});
            }
        }

        [HttpGet]
        [Route("{id}/transactions/{limit:int?}")]
        public ActionResult<string> GetTransactions(string id, int? limit = null)
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

                var walletRepository = new WalletRepository(VakapayRepositoryFactory.GetOldConnection());
                var wallets = walletRepository.FindAllWalletByUserId(id);

                var transactions = new List<BlockchainTransaction>();

                foreach (var wallet in wallets)
                {
                    switch (wallet.NetworkName)
                    {
                        case NetworkName.BTC:
                            transactions.AddRange(bitcoinDepositTrxRepo.FindTransactionsInner(wallet.Address));
                            transactions.AddRange(bitcoinWithdrawTrxRepo.FindTransactionsInner(wallet.Address));
                            break;
                        case NetworkName.ETH:
                            transactions.AddRange(ethereumDepositTrxRepo.FindTransactionsInner(wallet.Address));
                            transactions.AddRange(ethereumWithdrawTrxRepo.FindTransactionsInner(wallet.Address));
                            break;
                        case NetworkName.VAKA:
                            transactions.AddRange(vakacoinDepositTrxRepo.FindTransactionsInner(wallet.Address));
                            transactions.AddRange(vakacoinWithdrawTrxRepo.FindTransactionsInner(wallet.Address));
                            break;
                    }
                }
                
                var sortedTransactions = transactions.OrderBy(o=>o.UpdatedAt).ToList();

                return JsonHelper.SerializeObject(new ReturnDataObject()
                    {Status = Status.StatusSuccess, Message = Message.MessageSuccess, Data = sortedTransactions});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return JsonHelper.SerializeObject(new ReturnObject()
                    {Status = Status.StatusError, Message = e.Message});
            }
        }
        
        

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject("");
        }
    }
}