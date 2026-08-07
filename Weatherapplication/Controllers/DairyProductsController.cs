using Microsoft.AspNetCore.Mvc;

public class DairyProductsController : Controller
{
    public IActionResult Index()
    {
        return View(); // Views/DairyProducts/Index.cshtml load hoga
    }

    public IActionResult Products()
    {
        return View(); // Views/DairyProducts/Products.cshtml load hoga
    }

    public IActionResult About()
    {
        return View(); // Views/DairyProducts/About.cshtml load hoga
    }

    public IActionResult Contact()
    {
        return View(); // Views/DairyProducts/Contact.cshtml load hoga
    }
}