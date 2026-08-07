using Microsoft.AspNetCore.Mvc;

namespace Weatherapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DairyProductsapiController : ControllerBase
    {
        public class Product
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Cat { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string Unit { get; set; } = string.Empty;
            public string Desc { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string Tag { get; set; } = string.Empty;
        }

        private static readonly List<Product> ProductsList = new List<Product>
        {
            new Product { Id = "milk-toned", Name = "Toned Milk", Cat = "milk", Price = 32, Unit = "500 ml pouch", Desc = "Pasteurised farm milk, delivered chilled every morning.", Icon = "bottle", Tag = "Daily" },
            new Product { Id = "milk-full", Name = "Full Cream Milk", Cat = "milk", Price = 38, Unit = "500 ml pouch", Desc = "Rich and creamy, straight from grass-fed herds.", Icon = "bottle", Tag = "Best seller" },
            new Product { Id = "milk-a2", Name = "A2 Gir Cow Milk", Cat = "milk", Price = 68, Unit = "1 litre bottle", Desc = "Single-breed A2 milk from our Gir cow herd.", Icon = "bottle", Tag = "Premium" },
            new Product { Id = "dahi-classic", Name = "Classic Dahi", Cat = "dahi", Price = 40, Unit = "400 g tub", Desc = "Set the traditional way — thick, tangy, probiotic-rich.", Icon = "bowl", Tag = "Daily" },
            new Product { Id = "dahi-mishti", Name = "Mishti Doi", Cat = "dahi", Price = 55, Unit = "250 g tub", Desc = "Slow-caramelised jaggery dahi, Bengal style.", Icon = "bowl", Tag = "New" },
            new Product { Id = "dahi-greek", Name = "Hung Curd", Cat = "dahi", Price = 85, Unit = "200 g tub", Desc = "Strained thick for dips, shrikhand and desserts.", Icon = "bowl", Tag = "Premium" },
            new Product { Id = "ghee-desi", Name = "Desi Cow Ghee", Cat = "ghee", Price = 620, Unit = "500 ml jar", Desc = "Bilona-churned, slow-cooked in small batches.", Icon = "jar", Tag = "Best seller" },
            new Product { Id = "ghee-a2", Name = "A2 Bilona Ghee", Cat = "ghee", Price = 780, Unit = "500 ml jar", Desc = "Traditional hand-churned ghee from A2 milk.", Icon = "jar", Tag = "Premium" },
            new Product { Id = "chach-masala", Name = "Masala Chaas", Cat = "chach", Price = 25, Unit = "300 ml bottle", Desc = "Spiced buttermilk with roasted cumin and curry leaf.", Icon = "glass", Tag = "Daily" },
            new Product { Id = "chach-plain", Name = "Plain Chaas", Cat = "chach", Price = 20, Unit = "300 ml bottle", Desc = "Light and cooling, churned fresh every day.", Icon = "glass", Tag = "Daily" },
            new Product { Id = "cream-fresh", Name = "Fresh Malai", Cat = "cream", Price = 60, Unit = "200 g tub", Desc = "Hand-collected malai, perfect for kulfi and sweets.", Icon = "tub", Tag = "New" },
            new Product { Id = "cream-whip", Name = "Whipping Cream", Cat = "cream", Price = 150, Unit = "500 ml pack", Desc = "35% fat cream that whips to soft, stable peaks.", Icon = "tub", Tag = "Bakers' pick" }
        };

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(ProductsList);
        }
    }
}