using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Create(int? id)
        //{
        //    ViewBag.Id = id;
        //    return View();
        //}
    }
}