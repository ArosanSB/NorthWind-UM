﻿using System;
using System.Collections.Generic;

namespace NorthWind_UM.Models;

public partial class ProductsAboveAveragePrice
{
    public string ProductName { get; set; } = null!;

    public decimal? UnitPrice { get; set; }
}