using Microsoft.AspNetCore.Mvc;
using Vakapay.BitcoinBusiness;

namespace Vakapay.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BTCController : Controller
    {
        BitcoinRpc _bitcoinRpcRpc = new BitcoinRpc("HELOO");

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            var oputput = _bitcoinRpcRpc.CreateNewAddress(pass);
            return JsonHelper.SerializeObject(oputput);
        }
    }
}