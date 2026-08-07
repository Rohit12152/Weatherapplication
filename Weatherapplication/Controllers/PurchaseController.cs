using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }
        private string GeneratePoNo()
        {
            ViewBag.ItemList = new SelectList(_context.ItemMaster,"Id","ItemName");
            string PoNo = "PO00001";

            var lastSale = _context.PurchaseDetail.OrderByDescending(x => x.Id).FirstOrDefault();

            if (lastSale != null)
            {
                int no = Convert.ToInt32(
                    lastSale.PoNo.Replace("PO", "")
                );

                PoNo = "PO" + (no + 1).ToString("D5");
            }

            return PoNo;
        }
        public IActionResult Create(int? id)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            ViewBag.StudentList = new SelectList(_context.CustomerMaster.Where(x => x.UserId == userId).ToList(), "Id", "CustomerName");
            ViewBag.CategoryList = new SelectList(_context.Categories.Where(x => x.UserId == userId).ToList(), "CategoryId", "CategoryName");
            ViewBag.ItemList = new SelectList(_context.ItemMaster.ToList(), "Id", "ItemName");

            if (id == null)
            {
                var model = new PurchaseDetail
                {
                    PoNo = GeneratePoNo(),
                    PoDate = DateTime.Now
                };

                ViewBag.PoItems = new List<PurchaseItemDetail>();

                return View(model);
            }

            var purchase = _context.PurchaseDetail.FirstOrDefault(x => x.Id == id);

            var items = (from q in _context.PurchaseItemDetail
                         join i in _context.ItemMaster
                         on q.ItemId equals i.Id
                         where q.PoId == id
                         select new PurchaseItemDetail
                         {
                             Id = q.Id,
                             PoId = q.PoId,
                             ItemId = q.ItemId,
                             Qty = q.Qty,
                             Rate = q.Rate,
                             Amount = q.Amount,
                             GST = q.GST,
                             TaxAmount = q.TaxAmount,
                             TotalAmount = q.TotalAmount,
                             categoryid = i.categoryid
                         }).ToList();

            ViewBag.PoItems = items;
            ViewBag.RowCount = items.Count;

            return View(purchase);

        }
        [HttpPost]
        public IActionResult Create(PurchaseDetail poorder, List<PurchaseItemDetail> poitemdetails)
        {
            if (poitemdetails == null || poitemdetails.Count == 0)
            {
                TempData["error"] = "Please add atleast one item.";
                return RedirectToAction("Create");
            }

            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            double gross = poitemdetails.Sum(x => x.Amount ?? 0);
            double tax = poitemdetails.Sum(x => x.TaxAmount ?? 0);

            poorder.TotalAmount = gross;
            poorder.TotalTax = tax;
            poorder.NetAmount = gross + tax;
            poorder.UserId = userId;

            if (poorder.Id == 0)
            {
                // INSERT MASTER

                _context.PurchaseDetail.Add(poorder);
                _context.SaveChanges();

                // INSERT DETAILS

                foreach (var item in poitemdetails)
                {
                    item.PoId = poorder.Id;
                    var stockItem = _context.ItemMaster.FirstOrDefault(x => x.Id == item.ItemId);

                    if (stockItem != null)
                    {
                        stockItem.CurrentStock += Convert.ToDecimal(item.Qty);
                    }
                }

                _context.PurchaseItemDetail.AddRange(poitemdetails);

                TempData["success"] = "Purchase Order Saved Successfully";
            }
            else
            {
                // UPDATE MASTER

                _context.PurchaseDetail.Update(poorder);

                // DELETE OLD DETAILS

                var oldItems = _context.PurchaseItemDetail.Where(x => x.PoId == poorder.Id).ToList();

                _context.PurchaseItemDetail.RemoveRange(oldItems);

                // INSERT NEW DETAILS

                foreach (var item in poitemdetails)
                {
                    item.PoId = poorder.Id;
                    //var stockItem = _context.ItemMaster.FirstOrDefault(x => x.Id == item.ItemId);

                    //if (stockItem != null)
                    //{
                    //    stockItem.CurrentStock += Convert.ToDecimal(item.Qty);
                    //}
                }

                _context.PurchaseItemDetail.AddRange(poitemdetails);

                TempData["success"] = "Purchase Order Updated Successfully";
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var details = _context.PurchaseItemDetail.Where(x => x.PoId == id).ToList();

            _context.PurchaseItemDetail.RemoveRange(details);

            var master = _context.PurchaseDetail
                .FirstOrDefault(x => x.Id == id);

            if (master != null)
            {
                _context.PurchaseDetail.Remove(master);
            }

            _context.SaveChanges();

            TempData["success"] = "Purchase Order Deleted Successfully";

            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult GetItemsByCategory(int categoryId)
        {
            var items = _context.ItemMaster
                .Where(x => x.categoryid == categoryId)
                .Select(x => new
                {
                    id = x.Id,
                    itemName = x.ItemName
                })
                .ToList();

            return Json(items);
        }
        public IActionResult Index()
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            var data = (from s in _context.PurchaseDetail
                        join st in _context.CustomerMaster
                        on s.StudentId equals st.Id into std
                        from st in std.DefaultIfEmpty()
                        where s.UserId == userId
                        orderby s.Id descending
                        select new
                        {
                            Id = s.Id,
                            PoNo = s.PoNo,
                            Reference = s.Reference,
                            PoDate = s.PoDate,
                            NetAmount = s.NetAmount,
                            CustomerName = st == null ? "" : st.CustomerName
                        }).ToList();

            return View(data);
        }
    }
}
