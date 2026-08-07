using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Weatherapplication.Models;

namespace Weatherapplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            var data = (from s in _context.SalesDetail
                        join st in _context.StudentDetails
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
                            StudentName = st == null ? "" : st.StudentName
                        }).Take(3).ToList();

            return View(data);
        }
        [HttpGet]
        public IActionResult GetDashboardData(string filter)
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            var today = DateTime.Today;

            DateTime fromDate;
            DateTime toDate;

            switch (filter)
            {
                case "today":
                    fromDate = today;
                    toDate = today.AddDays(1);
                    break;

                case "weekly":
                    fromDate = today.AddDays(-(int)today.DayOfWeek);
                    toDate = fromDate.AddDays(7);
                    break;

                default: // monthly
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1);
                    break;
            }

            var sales = _context.SalesDetail.Where(x => x.UserId == userId && x.SalesDate >= fromDate && x.SalesDate < toDate).Sum(x => (decimal?)x.NetAmount) ?? 0;

            var purchase = _context.PurchaseDetail.Where(x => x.UserId == userId && x.PoDate >= fromDate && x.PoDate < toDate) .Sum(x => (decimal?)x.NetAmount) ?? 0;

            var stock = _context.ItemMaster.Where(x => x.CreatedDate >= fromDate && x.CreatedDate < toDate).Sum(x => (decimal?)x.CurrentStock) ?? 0;
            
            return Json(new
            {
                totalSales = sales,
                totalPurchase = purchase,
                totalStock = stock,
                profit = sales - purchase,

                 trendLabels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri" },
                trendData = new[] { 2000, 3500, 4200, 2800, 5000 },

                productLabels = new[] { "Shirt", "Jeans", "Shoes", "Watch" },
                productData = new[] { 50, 40, 30, 20 }
            });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult MarkNotificationsRead()
        {
            var unreadUsers = _context.Users
                .Where(x => !x.IsRead)
                .ToList();

            foreach (var user in unreadUsers)
            {
                user.IsRead = true;
            }

            _context.SaveChanges();

            return Ok();
        }
    }
}
