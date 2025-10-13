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

        // ================== DASHBOARD ==================
        public IActionResult Index()
        {
            var viewModel = new AdminViewModel
            {
                TotalUsers = _context.Users.Count(),
                TotalOrders = _context.Orders.Count(),

                TotalRevenue = _context.Orders
                    .Include(o => o.OrderStatus)
                    .Where(o => o.OrderStatus.Name == "Completed")
                    .Sum(o => (decimal?)o.TotalAmount) ?? 0,

                TotalProducts = _context.MenuItems.Count(),
                TotalCategories = _context.Categories.Count(),
                TotalVouchers = _context.Vouchers.Count(),
                TotalCombos = _context.ComboItems
                                .Select(c => c.ComboId)
                                .Distinct()
                                .Count(),
                TotalSizes = _context.MenuItemSizes
                                .Select(s => s.SizeName)
                                .Distinct()
                                .Count()

            };

            ViewData["ActivePage"] = "Dashboard";
            return View(viewModel);
        }

        // ================== USERS CRUD ==================
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

        // ================== PRODUCTS CRUD ==================

        // READ + SEARCH
        public async Task<IActionResult> Products(string? search)
        {
            var products = from p in _context.MenuItems.Include(p => p.Category)
                           select p;

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search)
                );
            }

            return View("~/Views/Products/Index.cshtml", await products.ToListAsync());
        }

        // CREATE
        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View("~/Views/Products/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(MenuItem product, IFormFile? imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Xử lý upload ảnh
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                        if (!Directory.Exists(uploadDir))
                            Directory.CreateDirectory(uploadDir);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }

                        product.ImageUrl = "/images/products/" + fileName;
                    }

                    product.CreatedAt = DateTime.Now;
                    _context.MenuItems.Add(product);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction(nameof(Products));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm sản phẩm: " + ex.Message);
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View("~/Views/Products/Create.cshtml", product);
        }

        // EDIT
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _context.MenuItems.Find(id);
            if (product == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View("~/Views/Products/Edit.cshtml", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(int id, MenuItem product, IFormFile? imageFile)
        {
            if (id != product.MenuItemId)
                return NotFound();

            var existingProduct = _context.MenuItems.AsNoTracking().FirstOrDefault(p => p.MenuItemId == id);
            if (existingProduct == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu upload ảnh mới → ghi đè ảnh cũ
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                        if (!Directory.Exists(uploadDir))
                            Directory.CreateDirectory(uploadDir);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }

                        product.ImageUrl = "/images/products/" + fileName;
                    }
                    else
                    {
                        // Nếu không chọn ảnh mới → giữ lại ảnh cũ
                        product.ImageUrl = existingProduct.ImageUrl;
                    }

                    _context.Update(product);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Products));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View("~/Views/Products/Edit.cshtml", product);
        }

        // DELETE
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.MenuItems.Find(id);
            if (product == null)
                return NotFound();

            return View("~/Views/Products/Delete.cshtml", product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProductConfirmed(int id)
        {
            var product = _context.MenuItems.Find(id);
            if (product != null)
            {
                _context.MenuItems.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Products));
        }
    }
}
