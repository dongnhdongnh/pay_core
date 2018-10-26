using Microsoft.AspNetCore.Mvc;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;

namespace Vakapay.ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ETHController : Controller
    {
        private readonly EthereumRpc _ethRpc = new EthereumRpc(AppSettingHelper.GetEthereumNode());

        [HttpGet("Block/{id}")]
        public ActionResult<string> GetBlock(int id)
        {
            return JsonHelper.SerializeObject(_ethRpc.FindBlockByNumber(id));
        }

        [HttpGet("Accounts")]
        public ActionResult<string> GetAccount()
        {
            return JsonHelper.SerializeObject(_ethRpc.GetAccounts());
        }

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject(_ethRpc.CreateNewAddress(pass));
        }
    }
}