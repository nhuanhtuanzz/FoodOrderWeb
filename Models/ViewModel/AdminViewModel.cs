namespace FoodOrderWeb.Models.ViewModel
{
    public class AdminViewModel
    {
        public int? TotalUsers { get; set; }

        public int? TotalOrders { get; set; }

        public decimal? TotalRevenue { get; set; }

        public int? TotalProducts { get; set; }

        public int? TotalCategories { get; set; }

        public List<User> Users { get; set; }
        public List<Category> Categories { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public List<Combo> Combos { get; set; }
        public List<Order> Orders { get; set; }
        public List<Voucher> Vouchers { get; set; }
        public AdminViewModel()
        {
            Users = new List<User>();
            Categories = new List<Category>();
            MenuItems = new List<MenuItem>();
            Combos = new List<Combo>();
            Orders = new List<Order>();
            Vouchers = new List<Voucher>();
        }

    }
}
