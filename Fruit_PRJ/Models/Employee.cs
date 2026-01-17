using System;
using System.Collections.Generic;

namespace Fruit_PRJ.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string Position { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
