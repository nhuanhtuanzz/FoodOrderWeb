using FoodOrderWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodOrderWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly FoodorderwebContext _context;

        public LoginController(FoodorderwebContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Email, string PasswordHash)
        {
            var user = _context.Users
                .Include(u => u.Address)
                .FirstOrDefault(u => u.Email == Email && u.IsActive);

            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, PasswordHash);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString())
            };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    if (user.Role == "Admin")
                        return RedirectToAction("Index", "AdminDashboard");
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string FullName, string Email, string PasswordHash, string Phone)
        {
            if (_context.Users.Any(u => u.Email == Email))
            {
                ViewBag.ErrorMessage = "Email already exists.";
                return View();
            }

            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PasswordHash = passwordHasher.HashPassword(null, PasswordHash),
                Phone = Phone,
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
