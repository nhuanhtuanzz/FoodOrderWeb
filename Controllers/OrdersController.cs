using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderWeb.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FoodOrderWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly FoodorderwebContext _context;

        public OrdersController(FoodorderwebContext context)
        {
            _context = context;
        }

        // --------------------- INDEX ---------------------
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

        // --------------------- DETAILS ---------------------
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItemSize)
                        .ThenInclude(ms => ms.MenuItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Combo)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            ViewData["Statuses"] = await _context.OrderStatuses.ToListAsync();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int statusId)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.OrderStatusId = statusId;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
        public IActionResult History(string? searchString)
        {
            var completedOrders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Where(o => o.OrderStatus.Name == "Completed")
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                completedOrders = completedOrders.Where(o =>
                    o.User.FullName.Contains(searchString) ||
                    o.OrderId.ToString().Contains(searchString));
            }

            ViewData["SearchString"] = searchString;
            return View(completedOrders.ToList());
        }

    }
}
