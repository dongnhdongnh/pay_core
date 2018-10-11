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
using Vakapay.UserBusiness;
namespace Vakapay.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : Controller
    {

        WalletBusiness.WalletBusiness _walletBusiness;
        UserBusiness.UserBusiness _userBusiness;
        public WalletController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = VakapayConfig.ConnectionString
            };
            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _walletBusiness =
                    new Vakapay.WalletBusiness.WalletBusiness(PersistenceFactory);
            _userBusiness
            = new Vakapay.UserBusiness.UserBusiness(PersistenceFactory);
        }
        [HttpGet("test")]
        public ActionResult<string> GetTest()
        {
            return "hello";
            //  return null;
        }
        [HttpGet("all")]
        public ActionResult<ReturnObject> GetWalletsByUser([FromQuery]string userID)
        {
            try
            {
                var user = _userBusiness.getUserByID(userID);
                if (user == null)
                    return new ReturnObject()
                    {
                        Status = Status.StatusError,
                        Message = "No user"
                    };
                var wallets = _walletBusiness.LoadAllWalletByUser(user);
                return new ReturnObject()
                {
                    Status = Status.StatusCompleted,
                    Message = JsonHelper.SerializeObject(wallets)
                };
            }
            catch (Exception e)
            {

                return new ReturnObject()
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }
        //[HttpPost("all")]
        //public ActionResult<string> GetWalletsByUser([FromBody]User user)
        //{
        //    var wallets = _walletBusiness.LoadAllWalletByUser(user);
        //    return JsonHelper.SerializeObject(wallets);
        //    //  return null;
        //}
        //  WalletBusiness.WalletBusiness walletBusiness = new WalletBusiness.WalletBusiness();
        [HttpGet("Infor")]
        public ActionResult<string> GetWalletInfor([FromQuery]string walletID)
        {
            var wallet = _walletBusiness.GetWalletByID(walletID);
            return JsonHelper.SerializeObject(wallet);
            //  return null;
        }

        [HttpPost("History")]
        public ActionResult<ReturnObject> GetWalletHistory([FromBody]Wallet walletID)
        {
            try
            {
                var _history = _walletBusiness.GetHistory(walletID, 1, 3, new string[] { nameof(BlockchainTransaction.Amount) });
                //  return 
                return new ReturnObject()
                {
                    Status = Status.StatusCompleted,
                    Message = JsonHelper.SerializeObject(_history)
                };
            }
            catch (Exception e)
            {


                return new ReturnObject()
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }

            //  return null;
        }
    }
}
