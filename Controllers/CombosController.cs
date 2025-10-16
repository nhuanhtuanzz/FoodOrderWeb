using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderWeb.Models;

namespace FoodOrderWeb.Controllers
{
    public class CombosController : Controller
    {
        private readonly FoodorderwebContext _context;

        public CombosController(FoodorderwebContext context)
        {
            _context = context;
        }

        // ================== READ + SEARCH ==================
        public async Task<IActionResult> Index(string? search)
        {
            var combos = from c in _context.Combos
                         select c; 

            if (!string.IsNullOrEmpty(search))
            {
                combos = combos.Where(c =>
                    (c.Name != null && c.Name.Contains(search)) ||
                    (c.Code != null && c.Code.Contains(search))
                );
            }

            combos = combos.OrderByDescending(c => c.CreatedAt);

            return View(await combos.ToListAsync());
        }


        // ================== CREATE ==================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Combo combo)
        {
            if (ModelState.IsValid)
            {
                combo.CreatedAt = DateTime.Now;
                _context.Combos.Add(combo);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "✅ Thêm combo thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(combo);
        }

        // ================== EDIT ==================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var combo = _context.Combos.Find(id);
            if (combo == null)
                return NotFound();

            return View(combo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Combo combo)
        {
            if (id != combo.ComboId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(combo);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "✏️ Cập nhật combo thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }

            return View(combo);
        }

        // ================== DELETE ==================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var combo = _context.Combos.Find(id);
            if (combo == null)
                return NotFound();

            return View(combo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var combo = _context.Combos.Find(id);
            if (combo != null)
            {
                _context.Combos.Remove(combo);
                _context.SaveChanges();

                TempData["SuccessMessage"] = " Đã xóa combo!";
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combo = await _context.Combos
                .Include(c => c.ComboItems)
                .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(m => m.ComboId == id);

            if (combo == null)
            {
                return NotFound();
            }

            return View(combo);
        }
    }
}
