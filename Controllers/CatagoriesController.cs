using FoodOrderWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly FoodorderwebContext _context;

        public CategoriesController(FoodorderwebContext context)
        {
            _context = context;
        }

        // ========== INDEX + SEARCH ==========
        public async Task<IActionResult> Index(string? search)
        {
            var categories = from c in _context.Categories
                             select c;

            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(c => c.Name.Contains(search));
            }

            return View(await categories.OrderBy(c => c.SortOrder).ToListAsync());
        }

        // ========== CREATE ==========
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // ========== EDIT ==========
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.CategoryId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // ========== DELETE ==========
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
