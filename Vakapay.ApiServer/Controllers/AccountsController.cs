using Microsoft.AspNetCore.Mvc;

namespace Vakapay.ApiServer.Controllers
{
    
    [Route("v1/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        [HttpGet("GetBalance/{id}")]
        public ActionResult<string> GetBlock(int id)
        {
            return JsonHelper.SerializeObject("");
        }

        [HttpGet("Test/{pass}")]
        public ActionResult<string> Test(string pass)
        {
            return JsonHelper.SerializeObject("");
        }
    }
}