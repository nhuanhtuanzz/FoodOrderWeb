using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using FoodOrderWeb.Models;

namespace FoodOrderWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly FoodorderwebContext _context = new FoodorderwebContext();
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginController(SignInManager<IdentityUser> signInManager, FoodorderwebContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string Email, string Passwordhash)
        {
            var user = _context.Users
                .Include(u => u.Address)
                .FirstOrDefault(u => u.Email == Email && u.IsActive);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Passwordhash);
                if (verificationResult == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("UserId", user.UserId.ToString())
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };
                    HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties).Wait();
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "AdminDashboard");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
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
            //var address = new Address
            //{
            //    UserId = user.UserId,
            //    Street = street,
            //    City = city,
            //    State = state,
            //    ZipCode = zipCode
            //};
            //_context.Addresses.Add(address);
            //_context.SaveChanges();
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme).Wait();
            return RedirectToAction("Index", "Home");
        }

    }
}
