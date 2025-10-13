using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderWeb.Models;
using System.Linq;

namespace FoodOrderWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly FoodorderwebContext _context;

        public OrdersController(FoodorderwebContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? searchString)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.User.FullName.Contains(searchString) ||
                    o.OrderId.ToString().Contains(searchString));
            }

            ViewData["SearchString"] = searchString;
            return View(orders.ToList());
        }

        public async Task<IActionResult> Details(int id)
{
    var order = await _context.Orders
        .Include(o => o.User)
        .Include(o => o.OrderStatus)
        .Include(o => o.OrderItems)
        .FirstOrDefaultAsync(o => o.OrderId == id);

    if (order == null)
        return NotFound();

    return View(order);
}
        public async Task<IActionResult> EditStatus(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            ViewBag.StatusList = await _context.OrderStatuses.ToListAsync();
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, int OrderStatusId)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.OrderStatusId = OrderStatusId;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
