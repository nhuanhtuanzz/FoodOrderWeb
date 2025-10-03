namespace FoodOrderWeb.Models.ViewModel
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public List<Combo> Combos { get; set; }
        public List<Order> Orders { get; set; }
        public List<Voucher> Vouchers { get; set; }

        public List<Voucher> ComboVouchers { get; set; }
        public List<Voucher> StarterVouchers { get; set; }
        public List<Voucher> MainVouchers { get; set; }
        public List<Voucher> DessertVouchers { get; set; }
        public List<Voucher> DrinkVouchers { get; set; }
        public HomeViewModel()
        {
            Categories = new List<Category>();
            MenuItems = new List<MenuItem>();
            Combos = new List<Combo>();
            Orders = new List<Order>();

        }
    }
}
