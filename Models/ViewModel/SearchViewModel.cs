using FoodOrderWeb.Models;
using System.Collections.Generic;

namespace FoodOrderWeb.ViewModels
{
    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;

        // Danh sách kết quả
        public List<Combo> Combos { get; set; } = new();
        public List<MenuItem> MenuItems { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        // Bộ lọc
        public string? SelectedCategory { get; set; }
        public string? SortBy { get; set; }   // "price_asc", "price_desc", "newest", "popular"
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Danh sách cho dropdown lọc
        public List<Category> AllCategories { get; set; } = new();
    }
}

