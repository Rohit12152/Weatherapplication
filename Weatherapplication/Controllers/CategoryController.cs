using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Categories.ToList();
            return View(data);
        }

        public IActionResult Create(int id = 0)
        {
            if (id == 0)
                return View(new Category());

            var data = _context.Categories.Find(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost]
        public IActionResult Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            if (model.CategoryId == 0)
            {
                model.UserId = userId;
                _context.Categories.Add(model);
                TempData["success"] = "Category Saved Successfully";
            }
            else
            {
                model.UserId = userId;
                _context.Categories.Update(model);
                TempData["success"] = "Category Update Successfully";
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var data = _context.Categories.Find(id);

            if (data != null)
            {
                _context.Categories.Remove(data);
                _context.SaveChanges();
            }
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
