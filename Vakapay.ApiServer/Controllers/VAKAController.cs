using Microsoft.AspNetCore.Mvc;
using Vakapay.Commons.Helpers;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VAKAController : Controller
    {
        VakacoinRpc _vakacoinRpc = new VakacoinRpc(AppSettingHelper.GetVakacoinNode());

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            var oputput = _vakacoinRpc.CreateNewAddress(pass);
            return JsonHelper.SerializeObject(oputput);
        }
    }
}