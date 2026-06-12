using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class QuotationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuotationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Create(int? id)
        {
            //var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            //ViewBag.StudentList = new SelectList(
            //    _context.StudentDetails.ToList(),
            //    "Id",
            //    "StudentName");
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            ViewBag.StudentList = new SelectList(_context.StudentDetails.Where(x => x.UserId == userId).ToList(), "Id", "StudentName");

            ViewBag.ItemList = new SelectList(
                _context.ItemMaster.ToList(),
                "Id",
                "ItemName");

            if (id == null)
            {
                var model = new QuotationDetail
                {
                    QuotationNo = GenerateQuotationNo(),
                    QuotationDate = DateTime.Now
                };

                ViewBag.QuotationItems = new List<QuotationItemDetail>();

                return View(model);
            }

            var quotation = _context.QuotationDetail
                 .FirstOrDefault(x => x.Id == id);

            var items = _context.QuotationItemDetail
                .Where(x => x.QuotationId == id)
                .ToList();

            ViewBag.QuotationItems = items;
            ViewBag.RowCount = items.Count;

            return View(quotation);
        }

        private string GenerateQuotationNo()
        {
            var lastQuotation = _context.QuotationDetail
     .OrderByDescending(x => x.Id)
     .FirstOrDefault();

            int nextNo = 1;

            if (lastQuotation != null)
            {
                nextNo = lastQuotation.Id + 1;
            }

            return "QTN-" + nextNo.ToString("00000");
        }
        [HttpPost]
        public IActionResult Create(QuotationDetail quotation,List<QuotationItemDetail> quotationDetails)
        {
            if (quotationDetails == null || quotationDetails.Count == 0)
            {
                TempData["error"] = "Please add atleast one item.";
                return RedirectToAction("Create");
            }
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            decimal gross = quotationDetails.Sum(x => x.Amount);
            decimal tax = quotationDetails.Sum(x => x.TaxAmount);

            quotation.TotalAmount = gross;
            quotation.TotalTax = tax;
            quotation.NetAmount = gross + tax;
            quotation.UserId = userId;

            if (quotation.Id == 0)
            {
                // INSERT

                _context.QuotationDetail.Add(quotation);
                _context.SaveChanges();

                foreach (var item in quotationDetails)
                {
                    item.QuotationId = quotation.Id;
                }

                _context.QuotationItemDetail.AddRange(quotationDetails);

                TempData["success"] = "Quotation Saved Successfully";
            }
            else
            {
                // UPDATE MASTER

                _context.QuotationDetail.Update(quotation);

                // DELETE OLD ITEMS

                var oldItems = _context.QuotationItemDetail
                    .Where(x => x.QuotationId == quotation.Id)
                    .ToList();

                _context.QuotationItemDetail.RemoveRange(oldItems);

                // INSERT NEW ITEMS

                foreach (var item in quotationDetails)
                {
                    item.QuotationId = quotation.Id;
                }
                Console.WriteLine("Count = " + quotationDetails.Count);
                _context.QuotationItemDetail.AddRange(quotationDetails);
               
                TempData["success"] = "Quotation Updated Successfully";
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            //var data = _context.QuotationDetail
            //    .Where(x => x.UserId == userId)
            //    .OrderByDescending(x => x.Id)
            //    .ToList();
            var data = _context.QuotationDetail.Include(x => x.Student).Where(x => x.UserId == userId).OrderByDescending(x => x.Id).ToList();
            return View(data);
        }

        public IActionResult Delete(int id)
        {
            var details = _context.QuotationItemDetail
                .Where(x => x.QuotationId == id)
                .ToList();

            _context.QuotationItemDetail.RemoveRange(details);

            var master = _context.QuotationDetail
                .FirstOrDefault(x => x.Id == id);

            if (master != null)
            {
                _context.QuotationDetail.Remove(master);
            }

            _context.SaveChanges();

            TempData["success"] = "Quotation Deleted Successfully";

            return RedirectToAction("Index");
        }
        public JsonResult GetItemRate(int itemId)
        {
            //var rate = _context.ItemMaster
            //                   .Where(x => x.Id == itemId)
            //                   .Select(x => x.PurchaseRate)
            //                   .FirstOrDefault();

            //return Json(new { rate = rate });
            var item = _context.ItemMaster
                      .Where(x => x.Id == itemId)
                      .Select(x => new
                      {
                          rate = x.PurchaseRate,
                          gst = x.GST
                      })
                      .FirstOrDefault();

            return Json(item);
        }
    }
}