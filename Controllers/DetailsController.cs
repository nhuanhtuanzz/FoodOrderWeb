using FoodOrderWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class DetailsController : Controller
{
    private readonly FoodorderwebContext _context;

    public DetailsController(FoodorderwebContext context)
    {
        _context = context;
    }

    // Chi tiết Combo
    public IActionResult ComboDetails(int id)
    {
        var combo = _context.Combos
            .Include(c => c.ComboItems)
                .ThenInclude(ci => ci.MenuItem)
            .FirstOrDefault(c => c.ComboId == id);

        if (combo == null) return NotFound();

        // Lấy ngẫu nhiên 3 combo khác
        var otherCombos = _context.Combos
            .Where(c => c.ComboId != id)
            .OrderBy(r => Guid.NewGuid())
            .Take(3)
            .ToList();

        ViewBag.OtherCombos = otherCombos;
        return View(combo);
    }

    // Chi tiết món ăn
    public IActionResult FoodDetails(int id)
    {
        var food = _context.MenuItems
            .Include(mi => mi.Category)
            .Include(mi => mi.MenuItemSizes)
            .FirstOrDefault(mi => mi.MenuItemId == id);

        if (food == null) return NotFound();

        // Lấy ngẫu nhiên 3 món khác cùng Category
        var relatedFoods = _context.MenuItems
            .Where(mi => mi.CategoryId == food.CategoryId && mi.MenuItemId != id)
            .OrderBy(r => Guid.NewGuid())
            .Take(3)
            .ToList();

        ViewBag.RelatedFoods = relatedFoods;
        return View(food);
    }

    // Chi tiết danh mục
    public IActionResult CategoryDetails(int id)
    {
        var category = _context.Categories
            .Include(c => c.MenuItems)
            .FirstOrDefault(c => c.CategoryId == id);

        if (category == null) return NotFound();

        // Lấy ngẫu nhiên 3 category khác
        var otherCategories = _context.Categories
            .Where(c => c.CategoryId != id)
            .OrderBy(r => Guid.NewGuid())
            .Take(3)
            .ToList();

        ViewBag.OtherCategories = otherCategories;
        return View(category);
    }
}
