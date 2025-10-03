using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class Combo
{
    public int ComboId { get; set; }

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
