using System;
using System.Collections.Generic;

namespace Fruit_PRJ.Models;

public partial class Order
{
    public int Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int TotalQuantity { get; set; }

    public int PaymentMethod { get; set; }

    public int Status { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
