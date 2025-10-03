﻿using System;
using System.Collections.Generic;

namespace FoodOrderWeb.Models;

public partial class ComboItem
{
    public int ComboItemId { get; set; }

    public int ComboId { get; set; }

    public int MenuItemId { get; set; }

    public int? MenuItemSizeId { get; set; }

    public int Quantity { get; set; }

    public virtual Combo Combo { get; set; } = null!;

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual MenuItemSize? MenuItemSize { get; set; }
}
