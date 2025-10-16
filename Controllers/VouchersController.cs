using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderWeb.Controllers
{
    public class VouchersController : Controller
    {
        private readonly FoodorderwebContext _context;

        public VouchersController(FoodorderwebContext context)
        {
            _context = context;
        }

        // GET: Vouchers
        public async Task<IActionResult> Index(string? searchString)
        {
            var vouchers = _context.Vouchers
                                   .OrderByDescending(v => v.CreatedAt)
                                   .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                vouchers = vouchers.Where(v => v.Code.Contains(searchString));
            }

            ViewData["SearchString"] = searchString;
            return View(await vouchers.ToListAsync());
        }

        // GET: Vouchers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // GET: Vouchers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vouchers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                voucher.CreatedAt = DateTime.Now;
                _context.Add(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: Vouchers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // POST: Vouchers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Voucher voucher)
        {
            if (id != voucher.VoucherId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voucher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Vouchers.Any(e => e.VoucherId == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: Vouchers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // POST: Vouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
