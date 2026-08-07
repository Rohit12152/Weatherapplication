using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class PurchaseInvoiceController : Controller
    {
        public IActionResult Index(int? poid)
        {
            ViewBag.POId = poid;
            return View();
        }
    }
}
