using FoodOrderWeb.Models;
using FoodOrderWeb.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly FoodorderwebContext _context;

        public AdminDashboardController(FoodorderwebContext context)
        {
            _context = context;
        }

        // ===== DASHBOARD =====
        public IActionResult Index()
        {
            var viewModel = new AdminViewModel
            {
                TotalUsers = _context.Users.Count(),
                TotalOrders = _context.Orders.Count(),
                TotalRevenue = _context.Orders
                    .Include(o => o.OrderStatus)
                    .Where(o => o.OrderStatus.Name == "Completed")
                    .Sum(o => o.TotalAmount),
                TotalProducts = _context.MenuItems.Count(),
                TotalCategories = _context.Categories.Count()
            };

            return View(viewModel);
        }

        // ===== USERS CRUD =====

        // READ + SEARCH
        public async Task<IActionResult> Users(string? searchString)
        {
            var users = from u in _context.Users select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.FullName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    (u.Phone != null && u.Phone.Contains(searchString))
                );
            }

            return View("~/Views/Users/Index.cshtml", await users.ToListAsync());
        }

        // ===== CREATE =====
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View("~/Views/Users/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    user.CreatedAt = DateTime.Now;
                    if (string.IsNullOrEmpty(user.Role))
                        user.Role = "Customer";

                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Users));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu: " + (ex.InnerException?.Message ?? ex.Message));
            }

            return View("~/Views/Users/Create.cshtml", user);
        }

        // ===== EDIT =====
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            var vm = new EditUserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role
            };

            return View("~/Views/Users/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(EditUserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(vm.UserId);
                if (user == null) return NotFound();

                // Cập nhật từng thuộc tính
                user.FullName = vm.FullName;
                user.Email = vm.Email;
                user.Phone = vm.Phone;
                user.Role = vm.Role;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật user thành công!";
                return RedirectToAction(nameof(Users));
            }

            return View("~/Views/Users/Edit.cshtml", vm);
        }



        // ===== DELETE =====
        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            return View("~/Views/Users/Delete.cshtml", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUserConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Users));
        }
    }
}
