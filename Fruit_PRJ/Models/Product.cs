using System;
using System.Collections.Generic;

namespace Fruit_PRJ.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int OriginId { get; set; }

    public int CategoryId { get; set; }

    public int UnitId { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int Status { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Origin Origin { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual Unit Unit { get; set; } = null!;
}
