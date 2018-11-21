using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NLog;
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
    public class CurrencyController : Controller
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        
        //symbol = VND, EUR ...
        [HttpGet("{symbol}")]
        public ReturnObject GetCurrencyConverter(string symbol)
        {
            try
            {
                var cacheKey = "USD_" + symbol;
                if (CacheHelper.HaveKey(cacheKey))
                {
                    Console.WriteLine("get cu "+cacheKey);
                    var result = CacheHelper.GetCacheString(cacheKey);
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Data = result
                    };
                }
                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Message = "Don't have exchange rate USD_"+symbol
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR"+e.ToString());
                _logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.ToString()
                };
            }
        }
    }
}