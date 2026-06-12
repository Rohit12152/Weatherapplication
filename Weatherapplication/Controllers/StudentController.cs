using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show Data
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            // var data = _context.StudentDetails.ToList();
            var data = _context.StudentDetails
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .ToList();
            return View(data);
        }

        // Create Page
        public IActionResult Create(int id = 0)
        {
            if (id == 0)
            {
                return View(new StudentDetails());
            }
            else
            {
                var data = _context.StudentDetails.Find(id);

                if (data == null)
                {
                    return RedirectToAction("Index");
                }

                return View(data);
            }
        }

        // Insert & Update
        [HttpPost]
        public IActionResult Create(StudentDetails obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _context.StudentDetails.Add(obj);

                    TempData["success"] = "Student Saved Successfully";
                }
                else
                {
                    _context.StudentDetails.Update(obj);

                    TempData["success"] = "Student Updated Successfully";
                }
                var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
                obj.UserId = userId;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(obj);
        }
        // Delete
        public IActionResult Delete(int id)
        {
            var data = _context.StudentDetails.Find(id);

            if (data != null)
            {
                _context.StudentDetails.Remove(data);

                _context.SaveChanges();

                TempData["success"] = "Student Deleted Successfully";
            }

            return RedirectToAction("Index");
        }
    }
}