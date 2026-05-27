using Microsoft.AspNetCore.Mvc;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.StudentDetails.ToList();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentDetails obj)
        {
            if (ModelState.IsValid)
            {
                _context.StudentDetails.Add(obj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }
    }
}
