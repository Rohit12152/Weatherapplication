using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Weatherapplication.Models;

[AllowAnonymous]
public class LoginController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoginController(ApplicationDbContext context)
    {
        _context = context;
    }
    [AllowAnonymous]
    public IActionResult LoginView()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == model.Email &&  x.Password == model.Password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims,  CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,  principal);

                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid Email or Password";
        }

        return View("LoginView", model);
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        Response.Cookies.Delete(".AspNetCore.Cookies");

        return RedirectToAction("LoginView", "Login");
    }
    public IActionResult Dashboard()
    {
        return View();
    }
}