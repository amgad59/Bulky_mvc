using Microsoft.AspNetCore.Mvc;

namespace BulkyApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MakeAGameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
