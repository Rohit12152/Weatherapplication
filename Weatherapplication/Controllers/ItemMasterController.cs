using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Linq;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class ItemMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public IActionResult Index()
        {
            var data = _context.ItemMaster.ToList();

            return View(data);
        }
        //public IActionResult CreateItem(int items)
        //{
        //    ItemMaster model = new ItemMaster();
        //    ViewBag.CategoriesList = new SelectList(_context.Categories.Where(x => x.IsActive == true).ToList(), "Id", "CategoryName");
        //    return View(model);
        //}
        // CREATE / EDIT
        public IActionResult Create(int id = 0)
        {
            ViewBag.CategoriesList = new SelectList(_context.Categories.Where(x => x.IsActive).ToList(),"CategoryId","CategoryName");
            if (id == 0)
            {
                return View(new ItemMaster());
            }
            else
            {
                var data = _context.ItemMaster.Find(id);

                return View(data);
            }
        }

        // SAVE
        [HttpPost]
        public IActionResult Create(ItemMaster obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoriesList = new SelectList(_context.Categories.Where(x => x.IsActive).ToList(),"CategoryId","CategoryName");

                return View(obj);
            }
            if (obj.Id == 0)
            {
                obj.CreatedDate = DateTime.Now;
                obj.CurrentStock = obj.OpeningStock;
                _context.ItemMaster.Add(obj);

                TempData["success"] = "Item Saved Successfully";
            }
            else
            {
                var item = _context.ItemMaster.FirstOrDefault(x => x.Id == obj.Id);

                if (item != null)
                {
                    item.ItemCode = obj.ItemCode;
                    item.ItemName = obj.ItemName;
                    item.Category = obj.Category;
                    item.Unit = obj.Unit;
                    item.PurchaseRate = obj.PurchaseRate;
                    item.SaleRate = obj.SaleRate;
                    item.GST = obj.GST;
                    item.OpeningStock = obj.OpeningStock;
                    item.MinStock = obj.MinStock;
                    item.Brand = obj.Brand;
                    item.HSNCode = obj.HSNCode;
                    item.ItemDescription = obj.ItemDescription;
                    item.categoryid = obj.categoryid;

                    // item.CurrentStock ko update mat karo
                }

               // _context.ItemMaster.Update(obj);

                TempData["success"] = "Item Updated Successfully";
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var data = _context.ItemMaster.Find(id);

            if (data != null)
            {
                _context.ItemMaster.Remove(data);

                _context.SaveChanges();

                TempData["success"] = "Item Deleted Successfully";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);

                    ExcelPackage.LicenseContext =
                        LicenseContext.NonCommercial;

                    using (var package =
                           new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet =
                            package.Workbook.Worksheets[0];

                        int rowCount =
                            worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            ItemMaster item =
                                new ItemMaster();

                            item.ItemCode =
                                worksheet.Cells[row, 1].Text;

                            item.ItemName =
                                worksheet.Cells[row, 2].Text;

                            item.Category =
                                worksheet.Cells[row, 3].Text;

                            item.Unit =
                                worksheet.Cells[row, 4].Text;

                            item.PurchaseRate =
                                Convert.ToDecimal(
                                    worksheet.Cells[row, 5].Text);

                            item.SaleRate =
                                Convert.ToDecimal(
                                    worksheet.Cells[row, 6].Text);

                            item.GST =
                                Convert.ToDecimal(
                                    worksheet.Cells[row, 7].Text);

                            item.OpeningStock =
                                Convert.ToInt32(
                                    worksheet.Cells[row, 8].Text);

                            item.MinStock =
                                Convert.ToInt32(
                                    worksheet.Cells[row, 9].Text);

                            item.Brand =
                                worksheet.Cells[row, 10].Text;

                            item.HSNCode =
                                worksheet.Cells[row, 11].Text;

                            item.ItemDescription =
                                worksheet.Cells[row, 12].Text;

                            item.CreatedDate =
                                DateTime.Now;

                            _context.ItemMaster.Add(item);
                        }

                        _context.SaveChanges();
                    }
                }

                TempData["success"] =
                    "Excel Imported Successfully";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult GetItems()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());

            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var query =
                from i in _context.ItemMaster.AsNoTracking()
                join c in _context.Categories
                    on i.categoryid equals c.CategoryId into cat
                from c in cat.DefaultIfEmpty()
                select new
                {
                    i.Id,
                    i.ItemCode,
                    i.ItemName,
                    CategoryName = c != null ? c.CategoryName : "",
                    i.Unit,
                    i.PurchaseRate,
                    i.SaleRate,
                    i.GST,
                    i.OpeningStock,
                    i.Brand
                };

            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x =>
                    x.ItemCode.Contains(searchValue) ||
                    x.ItemName.Contains(searchValue) ||
                    x.CategoryName.Contains(searchValue) ||
                    x.Brand.Contains(searchValue));
            }

            int recordsTotal = query.Count();

            var data = query
                .OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(length)
                .ToList();

            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }
        //public IActionResult GetItems()
        //{
        //    var draw = Request.Form["draw"].FirstOrDefault();
        //    var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
        //    var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());

        //    var searchValue = Request.Form["search[value]"].FirstOrDefault();

        //    IQueryable<ItemMaster> query = _context.ItemMaster.AsNoTracking();

        //    if (!string.IsNullOrEmpty(searchValue))
        //    {
        //        query = query.Where(x =>
        //            x.ItemCode.Contains(searchValue) ||
        //            x.ItemName.Contains(searchValue) ||
        //            x.Category.Contains(searchValue) ||
        //            x.Brand.Contains(searchValue));
        //    }

        //    int recordsTotal = query.Count();

        //    var data = query
        //        .OrderByDescending(x => x.Id)
        //        .Skip(start)
        //        .Take(length)
        //        .Select(x => new
        //        {
        //            id = x.Id,
        //            itemCode = x.ItemCode,
        //            itemName = x.ItemName,
        //            category = x.Category,
        //            unit = x.Unit,
        //            purchaseRate = x.PurchaseRate,
        //            saleRate = x.SaleRate,
        //            gst = x.GST,
        //            openingStock = x.OpeningStock,
        //            brand = x.Brand
        //        })
        //        .ToList();

        //    return Json(new
        //    {
        //        draw = draw,
        //        recordsFiltered = recordsTotal,
        //        recordsTotal = recordsTotal,
        //        data = data
        //    });
        //}
    }
}