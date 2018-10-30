using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    public class RecentActivityController : Controller
    {
        private VakapayRepositoryMysqlPersistenceFactory _vakapayRepository { get; }
        private readonly UserBusiness.UserBusiness _userBusiness;
        public RecentActivityController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            _vakapayRepository = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _userBusiness = new UserBusiness.UserBusiness(_vakapayRepository);
        }
        
        [HttpGet("transactions/{limit:int?}")]
        public ReturnObject GetTransactions(int? limit = null)
        {
            try
            {
                var email = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                var query = new Dictionary<string, string> {{"Email", email}};


                var userModel = _userBusiness.GetUserInfo(query);

                if (userModel == null)
                {
                    //return error
                    return new ReturnObject{
                        Status = Status.STATUS_ERROR,
                        Message = "User not exist in DB"
                    };
                }

                var userId = userModel.Id;
                
                var bitcoinDepositTrxRepo =
                    new BitcoinDepositTransactionRepository(_vakapayRepository.GetOldConnection());
                var bitcoinWithdrawTrxRepo =
                    new BitcoinWithdrawTransactionRepository(_vakapayRepository.GetOldConnection());
                var ethereumDepositTrxRepo =
                    new EthereumDepositTransactionRepository(_vakapayRepository.GetOldConnection());
                var ethereumWithdrawTrxRepo =
                    new EthereumWithdrawnTransactionRepository(_vakapayRepository.GetOldConnection());
                var vakacoinDepositTrxRepo =
                    new VakacoinDepositTransactionRepository(_vakapayRepository.GetOldConnection());
                var vakacoinWithdrawTrxRepo =
                    new VakacoinWithdrawTransactionRepository(_vakapayRepository.GetOldConnection());

                var activities = new List<RecentActivity>();

                activities.AddRange(ProcessTransactions(bitcoinDepositTrxRepo.FindTransactionsByUserId(userId),
                    DashboardConfig.BITCOIN, false));
                activities.AddRange(ProcessTransactions(bitcoinWithdrawTrxRepo.FindTransactionsByUserId(userId),
                    DashboardConfig.BITCOIN, true));
                
                activities.AddRange(ProcessTransactions(ethereumDepositTrxRepo.FindTransactionsByUserId(userId),
                    DashboardConfig.ETHEREUM, false));
                activities.AddRange(ProcessTransactions(ethereumWithdrawTrxRepo.FindTransactionsByUserId(userId),
                    DashboardConfig.ETHEREUM, true));
                
                activities.AddRange(ProcessTransactions(vakacoinDepositTrxRepo.FindTransactionsByUserId(userId),
                    DashboardConfig.VAKACOIN, false));
                activities.AddRange(ProcessTransactions(vakacoinWithdrawTrxRepo.FindTransactionsByUserId(userId),
                    DashboardConfig.VAKACOIN, true));
                
                var sortedActivities = activities.OrderByDescending(o=>o.TimeStamp).ToList();

                if ( limit != null && limit > 0 && limit < sortedActivities.Count )
                {
                    sortedActivities = sortedActivities.GetRange(0, (int) limit);
                }

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS, 
                    Data = JsonHelper.SerializeObject(sortedActivities)
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR, 
                    Message = e.Message
                };
            }
        }

        private List<RecentActivity> ProcessTransactions(List<BlockchainTransaction> transactions, string networkName,
            bool isSend)
        {
            var activities = new List<RecentActivity>();
            var price = Decimal.Parse(CacheHelper.GetCacheString(String.Format(
                RedisCacheKey.COINMARKET_PRICE_CACHEKEY, networkName,
                DashboardConfig.CURRENT)));
            
            foreach (var transaction in transactions)
            {
                var activity = new RecentActivity
                {
                    TimeStamp = transaction.UpdatedAt,
                    Amount = transaction.Amount,
                    IsSend = isSend,
                    NetworkName = networkName,
                    BlockNumber = transaction.BlockNumber,
                    FromAddress = transaction.FromAddress,
                    ToAddress = transaction.ToAddress,
                    Hash = transaction.Hash,
                    Price = price,
                    Value = transaction.Amount * price,
                    Status = transaction.Status
                };
                activities.Add(activity);
            }
            return activities;
        }
    }
}