using Microsoft.AspNetCore.Mvc;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;

namespace Vakapay.ApiService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ETHController : Controller
	{
		EthereumRpc _etheRpc = new EthereumRpc("HELOO");
		[HttpGet("Block/{id}")]
		public ActionResult<string> GetBlock(int id)
		{
			var oputput = _etheRpc.FindBlockByNumber(id);
			return JsonHelper.SerializeObject(oputput);
		}

		[HttpGet("Accounts")]
		public ActionResult<string> GetAccount()
		{
			var oputput = _etheRpc.GetAccounts();
			return JsonHelper.SerializeObject(oputput);
		}
	}
}
