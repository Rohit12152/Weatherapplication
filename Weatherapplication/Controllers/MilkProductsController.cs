using Microsoft.AspNetCore.Mvc;

namespace Weatherapplication.Controllers
{
    public class MilkProductsController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult products() => View();
        public IActionResult about() => View();
        public IActionResult contact() => View();
        public IActionResult Cart()
        {
            return View();
        }
        //public IActionResult products()
        //{
        //    return View();
        //}
        //public IActionResult about()
        //{
        //    return View();
        //}
        //public IActionResult contact()
        //{
        //    return View();
        //}
    }
}
