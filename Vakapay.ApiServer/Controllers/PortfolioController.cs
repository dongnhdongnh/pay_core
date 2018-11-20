using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class PortfolioController : CustomController
    {
        private readonly UserBusiness.UserBusiness _userBusiness;
        private readonly PortfolioHistoryBusiness.PortfolioHistoryBusiness _portfolioHistoryBusiness;

        public PortfolioController(
            IVakapayRepositoryFactory persistenceFactory,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
        ) : base(persistenceFactory, configuration, hostingEnvironment)
        {
            _userBusiness = new UserBusiness.UserBusiness(persistenceFactory);
            _portfolioHistoryBusiness = new PortfolioHistoryBusiness.PortfolioHistoryBusiness(persistenceFactory);
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
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = "User not exist in DB"
                };
            }

            var userid = userModel.Id;
            var currentTime = CommonHelper.GetUnixTimestamp();

            if (!Time.SECOND_COUNT_IN_PERIOD.ContainsKey(condition))
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = "Data not found with condition " + condition
                };
            }

            return Result(userid, currentTime - Time.SECOND_COUNT_IN_PERIOD[condition], currentTime,
                condition == DashboardConfig.CURRENT ? condition : null);
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

                if (time != null && time.Equals(DashboardConfig.CURRENT))
                {
                    var returnData = JsonHelper.SerializeObject(data[data.Count - 1]);

                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = returnData
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
                        Data = JsonHelper.SerializeObject(sortData)
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