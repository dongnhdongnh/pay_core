using System;
using System.Collections.Generic;
using System.Dynamic;
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
            var walletRepository = new WalletRepository(VakapayRepositoryFactory.GetOldConnection());
            var userRepository = new UserRepository(VakapayRepositoryFactory.GetOldConnection());
            var wallets = walletRepository.FindAllWalletByUserId(id);
            var user = userRepository.FindById(id);

            var balance = new List<CurrencyBalance>();

            foreach (var wallet in wallets)
            {
                balance.Add(new CurrencyBalance()
                {
                    Currency = NetworkName.CurrencySymbols[wallet.NetworkName],
                    Amount = wallet.GetAmount()
                });
            }

            var balanceResponse = new GetBalanceResponse()
            {
                Id = id,
                Balance = balance
            };


            return JsonHelper.SerializeObject(new Response(){Data = balanceResponse});
        }

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject("");
        }
    }
}