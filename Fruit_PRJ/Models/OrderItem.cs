using System;
using System.Collections.Generic;

namespace Fruit_PRJ.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string UnitName { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }

    public virtual Order Order { get; set; } = null!;
}
