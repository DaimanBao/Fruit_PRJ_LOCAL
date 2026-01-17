using System;
using System.Collections.Generic;

namespace Fruit_PRJ.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsMain { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
