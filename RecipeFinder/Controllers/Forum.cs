using Microsoft.AspNetCore.Mvc;

namespace RecipeFinder.Controllers
{
    public class ForumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}