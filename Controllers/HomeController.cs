using FoodOrderWeb.Models;
using FoodOrderWeb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FoodOrderWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FoodorderwebContext _context = new FoodorderwebContext();

        public HomeController(ILogger<HomeController> logger, FoodorderwebContext context)
        {
            _logger = logger;
            _context = context;
        }
        //home view all
        public IActionResult Index()
        {
            var featuredProducts = _context.MenuItems
                .OrderBy(r => Guid.NewGuid())
                .Take(5)
                .ToList();

            var categories = _context.Categories
                .Include(c => c.MenuItems)
                .ToList();
                
            var vouchers = _context.Vouchers
                .Include(v => v.MenuItems)
                    .ThenInclude(mi => mi.Category)
                .Include(v => v.Combos)
                .Where(v => v.IsActive && v.EndDate >= DateTime.Now)
                .ToList();

                    // phân lo?i theo category name (Starter, Main, Dessert, Drink)
                    var comboVouchers = vouchers.Where(v => v.Combos.Any()).ToList();
                    var starterVouchers = vouchers.Where(v => v.MenuItems.Any(mi => mi.Category.Name == "Starter")).ToList();
                    var mainVouchers = vouchers.Where(v => v.MenuItems.Any(mi => mi.Category.Name == "Main")).ToList();
                    var dessertVouchers = vouchers.Where(v => v.MenuItems.Any(mi => mi.Category.Name == "Dessert")).ToList();
                    var drinkVouchers = vouchers.Where(v => v.MenuItems.Any(mi => mi.Category.Name == "Drink")).ToList();

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                MenuItems = featuredProducts,
                Vouchers = vouchers,

                ComboVouchers = comboVouchers,
                StarterVouchers = starterVouchers,
                MainVouchers = mainVouchers,
                DessertVouchers = dessertVouchers,
                DrinkVouchers = drinkVouchers
            };
            return View(viewModel);
        }


        public IActionResult Search(string query, string? category, string? sortBy, decimal? minPrice, decimal? maxPrice)
        {
            var viewModel = new ViewModels.SearchViewModel
            {
                Query = query,
                SelectedCategory = category,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                AllCategories = _context.Categories.ToList()
            };

            // --- L?c MenuItems ---
            var menuItemsQuery = _context.MenuItems.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                menuItemsQuery = menuItemsQuery.Where(mi =>
                    mi.Name.Contains(query) ||
                    (mi.Description != null && mi.Description.Contains(query)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                menuItemsQuery = menuItemsQuery.Where(mi =>
                    mi.Category != null && mi.Category.Name == category);
            }

            if (minPrice.HasValue)
                menuItemsQuery = menuItemsQuery.Where(mi => mi.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                menuItemsQuery = menuItemsQuery.Where(mi => mi.Price <= maxPrice.Value);

            switch (sortBy)
            {
                case "price_asc":
                    menuItemsQuery = menuItemsQuery.OrderBy(mi => mi.Price);
                    break;
                case "price_desc":
                    menuItemsQuery = menuItemsQuery.OrderByDescending(mi => mi.Price);
                    break;
                case "newest":
                    menuItemsQuery = menuItemsQuery.OrderByDescending(mi => mi.CreatedAt);
                    break;
                default:
                    menuItemsQuery = menuItemsQuery.OrderBy(mi => mi.Name);
                    break;
            }

            viewModel.MenuItems = menuItemsQuery.ToList();

            // --- L?c Combos ---
            var combosQuery = _context.Combos.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                combosQuery = combosQuery.Where(c =>
                    c.Name.Contains(query) ||
                    (c.Description != null && c.Description.Contains(query)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                combosQuery = combosQuery.Where(c =>
                    c.ComboItems.Any(ci =>
                        ci.MenuItem.Category != null &&
                        ci.MenuItem.Category.Name == category));
            }

            if (minPrice.HasValue)
                combosQuery = combosQuery.Where(c => c.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                combosQuery = combosQuery.Where(c => c.Price <= maxPrice.Value);

            switch (sortBy)
            {
                case "price_asc":
                    combosQuery = combosQuery.OrderBy(c => c.Price);
                    break;
                case "price_desc":
                    combosQuery = combosQuery.OrderByDescending(c => c.Price);
                    break;
                case "newest":
                    combosQuery = combosQuery.OrderByDescending(c => c.CreatedAt);
                    break;
                default:
                    combosQuery = combosQuery.OrderBy(c => c.Name);
                    break;
            }

            viewModel.Combos = combosQuery.ToList();

            return View(viewModel);
        }


        public IActionResult Starter()
        {
            return View();
        }

        public IActionResult Main()
        {
            return View();
        }

        public IActionResult Dessert()
        {
            return View();
        }

        public IActionResult Drink()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
