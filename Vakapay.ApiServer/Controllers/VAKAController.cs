using Microsoft.AspNetCore.Mvc;
using Vakapay.BitcoinBusiness;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VAKAController : Controller
    {
        VakacoinRPC _vakacoinRpc = new VakacoinRPC("HELOO");

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            var oputput = _vakacoinRpc.CreateNewAddress(pass);
            return JsonHelper.SerializeObject(oputput);
        }
    }
}