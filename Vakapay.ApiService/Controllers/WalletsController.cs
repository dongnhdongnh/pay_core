using Microsoft.AspNetCore.Mvc;

namespace Vakapay.ApiService.Controllers
{
    public class WalletsController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return Ok();
        }
    }
}