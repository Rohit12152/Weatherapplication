using Microsoft.AspNetCore.Mvc;

namespace Weatherapplication.Controllers
{
    public class DeliveryOrderController : Controller
    {
        public IActionResult Index(int? soid)
        {
            ViewBag.SOId = soid;
            return View();
        }
    }
}
