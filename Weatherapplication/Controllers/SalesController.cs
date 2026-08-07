using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GenerateSalesNo()
        {
            ViewBag.ItemList = new SelectList(
    _context.ItemMaster,
    "Id",
    "ItemName");
            string salesNo = "SAL00001";

            var lastSale = _context.SalesDetail
                                   .OrderByDescending(x => x.Id)
                                   .FirstOrDefault();

            if (lastSale != null)
            {
                int no = Convert.ToInt32(
                    lastSale.SalesNo.Replace("SAL", "")
                );

                salesNo = "SAL" + (no + 1).ToString("D5");
            }

            return salesNo;
        }

        public IActionResult ConvertToSales(int quotationId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
          
            var quotation = _context.QuotationDetail
                                    .FirstOrDefault(x => x.Id == quotationId);

            if (quotation == null)
                return NotFound();

            var quotationItems = _context.QuotationItemDetail
                                         .Where(x => x.QuotationId == quotationId)
                                         .ToList();

            SalesDetail model = new SalesDetail();
            try
            {
                model.SalesNo = GenerateSalesNo();
                model.QuotationId = quotation.Id;
                model.ReferenceQuotationNo = quotation.QuotationNo;
                model.StudentId = quotation.StudentId;
                model.SalesDate = DateTime.Now;
                model.TotalAmount = quotation.TotalAmount;
                model.TotalTax = quotation.TotalTax;
                model.NetAmount = quotation.NetAmount;
                model.UserId = quotation.UserId;


                //model.SalesItems = quotationItems.Select(x => new SalesItemDetail
                //    {
                //        ItemId = x.ItemId,
                //        Qty = x.Qty,
                //        Rate = x.Rate,
                //        Amount = x.Amount,
                //        GST = x.GST,
                //        //  TaxPercent = x.TaxPercent,
                //        TaxAmount = x.TaxAmount,
                //        TotalAmount = x.TotalAmount
                //    }).ToList();
                model.SalesItems = (from q in quotationItems
                                    join i in _context.ItemMaster
                                        on q.ItemId equals i.Id
                                    select new SalesItemDetail
                                    {
                                        ItemId = q.ItemId,
                                        categoryid = i.categoryid,
                                        Qty = q.Qty,
                                        Rate = q.Rate,
                                        Amount = q.Amount,
                                        GST = q.GST,
                                        // TaxPercent = q.TaxPercent,
                                        TaxAmount = q.TaxAmount,
                                        TotalAmount = q.TotalAmount
                                    }).ToList();
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }


            ViewBag.StudentList = new SelectList(_context.CustomerMaster.Where(x => x.UserId == userId && x.partytype == 1 && x.IsActive == true).ToList(), "Id", "CustomerName");


            ViewBag.ItemList = new SelectList(_context.ItemMaster,"Id","ItemName");

            ViewBag.CategoryList = new SelectList(_context.Categories.ToList(), "CategoryId", "CategoryName");

           
            return View(model);
            // return View(model);
        }
        [HttpPost]
        public IActionResult SaveSales(SalesDetail model)
        {
            SalesDetail sales = new SalesDetail
            {
                SalesNo = model.SalesNo,
                QuotationId = model.QuotationId,
                ReferenceQuotationNo = model.ReferenceQuotationNo,
                StudentId = model.StudentId,
                SalesDate = model.SalesDate,
                TotalAmount = model.TotalAmount,
                TotalTax = model.TotalTax,
                NetAmount = model.NetAmount,
                UserId = model.UserId
            };

            _context.SalesDetail.Add(sales);
            _context.SaveChanges();

            foreach (var item in model.SalesItems)
            {
                item.SalesId = sales.Id;
                _context.SalesItemDetail.Add(item);

                // Stock Minus
                var stockItem = _context.ItemMaster.FirstOrDefault(x => x.Id == item.ItemId);

                if (stockItem != null)
                {
                    stockItem.CurrentStock -= Convert.ToDecimal(item.Qty);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            var data = (from s in _context.SalesDetail
                        join st in _context.CustomerMaster
                        on s.StudentId equals st.Id into std
                        from st in std.DefaultIfEmpty()
                        where s.UserId == userId
                        orderby s.Id descending
                        select new
                        {
                            Id = s.Id,
                            SalesNo = s.SalesNo,
                            ReferenceQuotationNo = s.ReferenceQuotationNo,
                            SalesDate = s.SalesDate,
                            NetAmount = s.NetAmount,
                            CustomerName = st == null ? "" : st.CustomerName
                        }).ToList();

            return View(data);


            //return Content("Record Count : " + data.Count);
        }
        public IActionResult Delete(int id)
        {
            var details = _context.SalesItemDetail
                .Where(x => x.SalesId == id)
                .ToList();

            _context.SalesItemDetail.RemoveRange(details);

            var master = _context.SalesDetail
                .FirstOrDefault(x => x.Id == id);

            if (master != null)
            {
                _context.SalesDetail.Remove(master);
            }

            _context.SaveChanges();

            TempData["success"] = "Sales Order Deleted Successfully";

            return RedirectToAction("Index");
        }
        public IActionResult Create(int? id)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            ViewBag.StudentList = new SelectList(_context.CustomerMaster.Where(x => x.UserId == userId && x.partytype == 1 && x.IsActive == true).ToList(), "Id", "CustomerName");
            ViewBag.CategoryList = new SelectList(_context.Categories.Where(x => x.UserId == userId).ToList(), "CategoryId", "CategoryName");
            ViewBag.ItemList = new SelectList(_context.ItemMaster.ToList(),"Id","ItemName");

            if (id == null)
            {
                var model = new SalesDetail
                {
                    SalesNo = GenerateSalesNo(),
                    SalesDate = DateTime.Now
                };

                ViewBag.SalesItems = new List<SalesItemDetail>();

                return View(model);
            }

            var sales = _context.SalesDetail
                 .FirstOrDefault(x => x.Id == id);

            //var items = _context.SalesItemDetail
            //    .Where(x => x.SalesId == id)
            //    .ToList();

            var items = (from q in _context.SalesItemDetail
                         join i in _context.ItemMaster
                         on q.ItemId equals i.Id
                         where q.SalesId == id
                         select new SalesItemDetail
                         {
                             Id = q.Id,
                             SalesId = q.SalesId,
                             ItemId = q.ItemId,
                             Qty = q.Qty,
                             Rate = q.Rate,
                             Amount = q.Amount,
                             GST = q.GST,
                             TaxAmount = q.TaxAmount,
                             TotalAmount = q.TotalAmount,
                             categoryid = i.categoryid
                         }).ToList();

            ViewBag.SalesItems = items;
            ViewBag.RowCount = items.Count;

            return View(sales);

        }
        [HttpPost]
        public IActionResult Create(SalesDetail salesorder, List<SalesItemDetail> salesitemdetails)
        {
            if (salesitemdetails == null || salesitemdetails.Count == 0)
            {
                TempData["error"] = "Please add atleast one item.";
                return RedirectToAction("Create");
            }

            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            double gross = salesitemdetails.Sum(x => x.Amount ?? 0);
            double tax = salesitemdetails.Sum(x => x.TaxAmount ?? 0);

            salesorder.TotalAmount = gross;
            salesorder.TotalTax = tax;
            salesorder.NetAmount = gross + tax;
            salesorder.UserId = userId;

            if (salesorder.Id == 0)
            {
                // INSERT MASTER

                _context.SalesDetail.Add(salesorder);
                _context.SaveChanges();

                // INSERT DETAILS

                foreach (var item in salesitemdetails)
                {
                    item.SalesId = salesorder.Id;
                    var stockItem = _context.ItemMaster.FirstOrDefault(x => x.Id == item.ItemId);

                    if (stockItem != null)
                    {
                        stockItem.CurrentStock -= Convert.ToDecimal(item.Qty);
                    }
                }

                _context.SalesItemDetail.AddRange(salesitemdetails);

                TempData["success"] = "Sales Order Saved Successfully";
            }
            else
            {
                // UPDATE MASTER

                _context.SalesDetail.Update(salesorder);

                // DELETE OLD DETAILS

                var oldItems = _context.SalesItemDetail
                    .Where(x => x.SalesId == salesorder.Id)
                    .ToList();

                _context.SalesItemDetail.RemoveRange(oldItems);

                // INSERT NEW DETAILS

                foreach (var item in salesitemdetails)
                {
                    item.SalesId = salesorder.Id;
                    //var stockItem = _context.ItemMaster.FirstOrDefault(x => x.Id == item.ItemId);

                    //if (stockItem != null)
                    //{
                    //    stockItem.CurrentStock -= Convert.ToDecimal(item.Qty);
                    //}
                }

                _context.SalesItemDetail.AddRange(salesitemdetails);

                TempData["success"] = "Sales Order Updated Successfully";
            }

            _context.SaveChanges();

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
    }
}
