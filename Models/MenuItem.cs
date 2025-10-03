using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class MenuItem
{
    public int MenuItemId { get; set; }

    public int? CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsCombo { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

    public virtual ICollection<MenuItemSize> MenuItemSizes { get; set; } = new List<MenuItemSize>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
