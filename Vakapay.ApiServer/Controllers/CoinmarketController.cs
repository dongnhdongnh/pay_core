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
            switch (condition)
            {
                    case "day":
                        return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                            RedisCacheKey.EOS, RedisCacheKey.DAY));
                    case "week":
                        return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                            RedisCacheKey.EOS, RedisCacheKey.WEEK));
                    case "month":
                        return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                            RedisCacheKey.EOS, RedisCacheKey.MONTH));
                    case "year":
                        return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                            RedisCacheKey.EOS, RedisCacheKey.YEAR));
                    case "all":
                        return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                            RedisCacheKey.EOS, RedisCacheKey.ALL));
                    default:
                        return new ReturnObject
                        {
                            Status = Status.STATUS_ERROR,
                            Message = "Can't find by "+condition
                        };
            }
        }
        
        [HttpGet("eos/{condition}")]
        public ReturnObject EOSPrice(string condition)
        {
            switch (condition)
            {
                case "day":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.EOS, RedisCacheKey.DAY));
                case "week":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.EOS, RedisCacheKey.WEEK));
                case "month":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.EOS, RedisCacheKey.MONTH));
                case "year":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.EOS, RedisCacheKey.YEAR));
                case "all":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.EOS, RedisCacheKey.ALL));
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
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.BITCOIN, RedisCacheKey.DAY));
                case "week":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.BITCOIN, RedisCacheKey.WEEK));
                case "month":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.BITCOIN, RedisCacheKey.MONTH));
                case "year":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.BITCOIN, RedisCacheKey.YEAR));
                case "all":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.BITCOIN, RedisCacheKey.ALL));
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
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.ETHEREUM, RedisCacheKey.DAY));
                case "week":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.ETHEREUM, RedisCacheKey.WEEK));
                case "month":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.ETHEREUM, RedisCacheKey.MONTH));
                case "year":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.ETHEREUM, RedisCacheKey.YEAR));
                case "all":
                    return Result(String.Format(RedisCacheKey.COINMARKET_PRICE_CACHEKEY,
                        RedisCacheKey.ETHEREUM, RedisCacheKey.ALL));
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