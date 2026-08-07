using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Weatherapplication.Models;
using Weatherapplication.Services;

namespace Weatherapplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AccountController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(UserRegistration model)
        {
            if (ModelState.IsValid)
            {
                bool emailExists = _context.Users.Any(x => x.Email.ToLower() == model.Email.ToLower());

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email is already registered."); //"Email", means model property name  public string Email { get; set; }
                    return View(model);
                }

                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                string body = $@"
            <h2>Welcome {model.Name}</h2>

            <p>Your account has been created successfully.</p>

            <table border='1' cellpadding='5'>
                <tr>
                    <td><b>Username</b></td>
                    <td>{model.Email}</td>
                </tr>

                <tr>
                    <td><b>Password</b></td>
                    <td>{model.Password}</td>
                </tr>
            </table>

            <br/>
            <p>Thank You.</p>";

                await _emailService.SendEmailAsync(
                    model.Email,
                    "Welcome To Our Website",
                    body);

                TempData["Success"] =
                    "Registration completed successfully.";

//                var notifications = _context.Users
//.OrderByDescending(x => x.CreatedDate)
//.Take(10)
//.ToList();

//                ViewBag.Notifications = notifications;
//                ViewBag.NotificationCount = notifications.Count();
                return RedirectToAction("Signup");
            }
           
            return View(model);
        }
    }
}
