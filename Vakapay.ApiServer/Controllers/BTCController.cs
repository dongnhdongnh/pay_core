using Microsoft.AspNetCore.Mvc;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;

namespace Vakapay.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BTCController : Controller
    {
        private readonly BitcoinRpc _bitcoinRpc = new BitcoinRpc(AppSettingHelper.GetBitcoinNode(),
            AppSettingHelper.GetBitcoinRpcAuthentication());

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject(_bitcoinRpc.CreateNewAddress(pass));
        }
    }
}