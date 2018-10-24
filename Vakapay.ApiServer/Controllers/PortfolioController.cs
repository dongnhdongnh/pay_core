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
    public class PortfolioController : ControllerBase
    {
        private readonly VakapayRepositoryMysqlPersistenceFactory _vakapayRepository;
        private readonly UserBusiness.UserBusiness _userBusiness;
        private readonly PortfolioHistoryBusiness.PortfolioHistoryBusiness _portfolioHistoryBusiness;

        public PortfolioController()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            _vakapayRepository = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _userBusiness = new UserBusiness.UserBusiness(_vakapayRepository);
            _portfolioHistoryBusiness = new PortfolioHistoryBusiness.PortfolioHistoryBusiness(_vakapayRepository);
        }
        
        [HttpGet("value/{condition}")]
        public ReturnObject PortfolioValueHistory(string condition)
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

            var userid = userModel.Id;
            
            switch (condition)
            {
                    case "hour":
                        return Result(userid, CommonHelper.GetUnixTimestamp() - 60 * 60, CommonHelper.GetUnixTimestamp());
                    
                    case "day":
                        return Result(userid, CommonHelper.GetUnixTimestamp() - 24 * 60 * 60, CommonHelper.GetUnixTimestamp());
                    
                    case "week":
                        return Result(userid, CommonHelper.GetUnixTimestamp() - 7 * 24 * 60 * 60, CommonHelper.GetUnixTimestamp());
                    
                    case "month":
                        return Result(userid, CommonHelper.GetUnixTimestamp() - 30 * 24 * 60 * 60, CommonHelper.GetUnixTimestamp());
                    
                    case "year":
                        return Result(userid, CommonHelper.GetUnixTimestamp() - 365 * 24 * 60 * 60, CommonHelper.GetUnixTimestamp());
                    
                    case "all":
                        return Result(userid, CommonHelper.GetUnixTimestamp() - 5 * 365 * 24 * 60 * 60, CommonHelper.GetUnixTimestamp());
                    
                    case "current":
                        return Result(userid, CommonHelper.GetUnixTimestamp() - 60 * 60, CommonHelper.GetUnixTimestamp(), condition);
                    
                    default:
                        return new ReturnObject
                        {
                            Status = Status.STATUS_ERROR,
                            Message = "Data not found with condition " + condition
                        };
            }
        }

        private ReturnObject Result(string userId, long from, long to, string time = null)
        {
            try
            {
                var data = _portfolioHistoryBusiness.FindByUserId(userId, from, to);
                if (data == null)
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Data not found!"
                    };

                if (time != null && time.Equals("current"))
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonHelper.SerializeObject(data[data.Count - 1])
                    };
                }
                
                // if data is too many
                if (data.Count >= 600)
                {
                    var sortData = new List<PortfolioHistory>();

                    var chooseEvery = data.Count / 300;
                    //choose 1 point in every chooseEvery point
                    foreach (var portfolio in data)
                    {
                        if (data.IndexOf(portfolio) % chooseEvery == 0)
                            sortData.Add(portfolio);
                    }

                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = JsonHelper.SerializeObject(data)
                    };
                }
                
                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(data)
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = "Error!"
                };
            }
        }
    }
}