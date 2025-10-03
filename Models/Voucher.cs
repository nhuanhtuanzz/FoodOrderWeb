using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? DiscountPercent { get; set; }

    public decimal? DiscountAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Combo> Combos { get; set; } = new List<Combo>();

    public virtual ICollection<MenuItemSize> MenuItemSizes { get; set; } = new List<MenuItemSize>();

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
