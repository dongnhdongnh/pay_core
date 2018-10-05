using Microsoft.AspNetCore.Mvc;
using Vakapay.EthereumBusiness;

namespace Vakapay.ApiServer.Controllers
{
    
    [Route("v1/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        [HttpGet("GetBalance/{id}")]
        public ActionResult<string> GetBlock(int id)
        {
            
        }

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject("");
        }
    }
}