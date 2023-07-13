using Microsoft.AspNetCore.Mvc;

namespace EmpireApp.Areas.Customer.Controllers
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
