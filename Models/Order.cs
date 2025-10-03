using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int? AddressId { get; set; }

    public DateTime OrderDate { get; set; }

    public int OrderStatusId { get; set; }

    public int? PaymentMethodId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal ShippingFee { get; set; }

    public string? VoucherCode { get; set; }

    public string? Note { get; set; }

    public bool Paid { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual User User { get; set; } = null!;
}
