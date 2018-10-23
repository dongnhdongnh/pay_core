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

        [HttpGet("eos/{condition}")]
        public ReturnObject EOSPrice(string condition)
        {
            switch (condition)
            {
                    case "day":
                        return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                            DashboardConfig.EOS, DashboardConfig.DAY));
                    case "week":
                        return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                            DashboardConfig.EOS, DashboardConfig.WEEK));
                    case "month":
                        return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                            DashboardConfig.EOS, DashboardConfig.MONTH));
                    case "year":
                        return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                            DashboardConfig.EOS, DashboardConfig.YEAR));
                    case "all":
                        return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                            DashboardConfig.EOS, DashboardConfig.ALL));
                    default:
                        return new ReturnObject
                        {
                            Status = Status.STATUS_ERROR,
                            Message = "Can't find by "+condition
                        };
            }
        }
        
        [HttpGet("bitcoin/{condition}")]
        public ReturnObject BTCPrice(string condition)
        {
            switch (condition)
            {
                case "day":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.BITCOIN, DashboardConfig.DAY));
                case "week":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.BITCOIN, DashboardConfig.WEEK));
                case "month":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.BITCOIN, DashboardConfig.MONTH));
                case "year":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.BITCOIN, DashboardConfig.YEAR));
                case "all":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.BITCOIN, DashboardConfig.ALL));
                default:
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't find by "+condition
                    };
            }
        }
        
        [HttpGet("ethereum/{condition}")]
        public ReturnObject ETHPrice(string condition)
        {
            switch (condition)
            {
                case "day":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.ETHEREUM, DashboardConfig.DAY));
                case "week":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.ETHEREUM, DashboardConfig.WEEK));
                case "month":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.ETHEREUM, DashboardConfig.MONTH));
                case "year":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.ETHEREUM, DashboardConfig.YEAR));
                case "all":
                    return Result(String.Format(DashboardConfig.COINMARKET_PRICE_CACHEKEY,
                        DashboardConfig.ETHEREUM, DashboardConfig.ALL));
                default:
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't find by "+condition
                    };
            }
        }

        private ReturnObject Result(string cacheKey)
        {
            if (!CacheHelper.HaveKey(cacheKey))
                return null;
            
            var result = new ReturnObject
            {
                Status = Status.STATUS_SUCCESS,
                Data = CacheHelper.GetCacheString(cacheKey)
            };
            return result;
        }
    }
}