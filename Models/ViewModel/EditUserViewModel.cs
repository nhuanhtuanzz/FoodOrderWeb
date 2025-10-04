namespace FoodOrderWeb.Models.ViewModel {
    public class EditUserViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; }
    }

}