using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class MenuItemSize
{
    public int MenuItemSizeId { get; set; }

    public int MenuItemId { get; set; }

    public string SizeName { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
