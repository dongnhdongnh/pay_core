using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;

namespace Vakapay.ApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    public class CoinmarketController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return null;
        }

        [HttpGet("vakacoin/{condition}")]
        public ReturnObject VKCPrice(string condition)
        {
            return Result(DashboardConfig.BITCOIN, condition);
        }
        
        [HttpGet("bitcoin/{condition}")]
        public ReturnObject BTCPrice(string condition)
        {
            return Result(DashboardConfig.BITCOIN, condition);
        }
        
        [HttpGet("ethereum/{condition}")]
        public ReturnObject ETHPrice(string condition)
        {
            return Result(DashboardConfig.ETHEREUM, condition);
        }

        private ReturnObject Result(string networkName, string condition)
        {
            var cacheKey = String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                networkName, condition);
            
            if (!CacheHelper.HaveKey(cacheKey))
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = "Can't find by "+condition
                };
            
            var result = new ReturnObject
            {
                Status = Status.STATUS_SUCCESS,
                Data = CacheHelper.GetCacheString(cacheKey)
            };
            return result;
        }
    }
}