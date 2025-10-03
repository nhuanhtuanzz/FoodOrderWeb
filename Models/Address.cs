using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int UserId { get; set; }

    public string? Label { get; set; }

    public string Street { get; set; } = null!;

    public string? City { get; set; }

    public string? District { get; set; }

    public string? PostalCode { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
