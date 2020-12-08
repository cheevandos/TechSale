using Microsoft.AspNetCore.Mvc;

namespace WebApplicationTechSale.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
