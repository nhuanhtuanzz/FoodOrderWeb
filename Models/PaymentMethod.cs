using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
