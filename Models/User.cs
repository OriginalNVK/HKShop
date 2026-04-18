using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? RandomKey { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Admin? Admin { get; set; }
}
