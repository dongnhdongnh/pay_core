using Microsoft.AspNetCore.Mvc;

namespace Vakapay.ApiService.Controllers
{
    public class TransactionsController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return Ok();
        }
    }
}