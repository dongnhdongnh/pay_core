using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Configuration;
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
                        return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                            CoinmarketConfiguration.EOS, CoinmarketConfiguration.DAY));
                    case "week":
                        return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                            CoinmarketConfiguration.EOS, CoinmarketConfiguration.WEEK));
                    case "month":
                        return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                            CoinmarketConfiguration.EOS, CoinmarketConfiguration.MONTH));
                    case "year":
                        return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                            CoinmarketConfiguration.EOS, CoinmarketConfiguration.YEAR));
                    case "all":
                        return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                            CoinmarketConfiguration.EOS, CoinmarketConfiguration.ALL));
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
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.BITCOIN, CoinmarketConfiguration.DAY));
                case "week":
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.BITCOIN, CoinmarketConfiguration.WEEK));
                case "month":
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.BITCOIN, CoinmarketConfiguration.MONTH));
                case "year":
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.BITCOIN, CoinmarketConfiguration.YEAR));
                case "all":
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.BITCOIN, CoinmarketConfiguration.ALL));
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
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.ETHEREUM, CoinmarketConfiguration.DAY));
                case "week":
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.ETHEREUM, CoinmarketConfiguration.WEEK));
                case "month":
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.ETHEREUM, CoinmarketConfiguration.MONTH));
                case "year":
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.ETHEREUM, CoinmarketConfiguration.YEAR));
                case "all":
                    return Result(String.Format(CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY,
                        CoinmarketConfiguration.ETHEREUM, CoinmarketConfiguration.ALL));
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