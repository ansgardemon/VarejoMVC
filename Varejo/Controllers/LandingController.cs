using Microsoft.AspNetCore.Mvc;

namespace Varejo.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        // Página de produtos
        public IActionResult Produtos()
        {
            return View();
        }
    }


}
