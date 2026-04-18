using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class Admin
{
    public string AdminId { get; set; } = null!;

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual User User { get; set; } = null!;
}
