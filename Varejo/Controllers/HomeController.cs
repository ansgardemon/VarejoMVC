using Microsoft.AspNetCore.Mvc;

namespace Varejo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
