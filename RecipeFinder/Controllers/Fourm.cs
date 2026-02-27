using Microsoft.AspNetCore.Mvc;

namespace RecipeFinder.Controllers
{
    public class FourmController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}