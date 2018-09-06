using Microsoft.AspNetCore.Mvc;

namespace Vakapay.ApiService.Controllers
{
    public class UsersController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return Ok();
        }
    }
}