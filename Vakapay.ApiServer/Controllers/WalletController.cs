using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.WalletBusiness;
using Vakapay.Models.Entities;
using Vakapay.Models.Domains;

namespace Vakapay.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : Controller
    {

        WalletBusiness.WalletBusiness _walletBusiness;
        public WalletController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = VakapayConfig.ConnectionString
            };
            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _walletBusiness =
                    new Vakapay.WalletBusiness.WalletBusiness(PersistenceFactory);
        }
        [HttpGet("test")]
        public ActionResult<string> GetTest()
        {
            return "hello";
            //  return null;
        }
        [HttpPost("all")]
        public ActionResult<string> GetWalletsByUser([FromQuery]User user)
        {
            var wallets = _walletBusiness.LoadAllWalletByUser(user);
            return JsonHelper.SerializeObject(wallets);
            //  return null;
        }
        //  WalletBusiness.WalletBusiness walletBusiness = new WalletBusiness.WalletBusiness();
        [HttpGet("Infor")]
        public ActionResult<string> GetWalletInfor([FromQuery]string walletID)
        {
            var wallet = _walletBusiness.GetWalletByID(walletID);
            return JsonHelper.SerializeObject(wallet);
            //  return null;
        }

        [HttpPost("History")]
        public ActionResult<string> GetWalletHistory([FromBody]Wallet walletID)
        {
            var _history = _walletBusiness.GetHistory(walletID, 1, 3, new string[] { nameof(BlockchainTransaction.Amount) });
            return JsonHelper.SerializeObject(_history);
            //  return null;
        }
    }
}
