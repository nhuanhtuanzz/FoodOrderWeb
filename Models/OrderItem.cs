using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int? MenuItemSizeId { get; set; }

    public int? ComboId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual Combo? Combo { get; set; }

    public virtual MenuItemSize? MenuItemSize { get; set; }

    public virtual Order Order { get; set; } = null!;
}
