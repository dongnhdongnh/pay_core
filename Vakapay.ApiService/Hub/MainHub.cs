using Microsoft.AspNetCore.Mvc;

namespace Vakapay.ApiService.Hub
{
    public class MainHub : Controller
    {
        // GET
        public IActionResult Index()
        {
            return Ok();
        }
    }
}