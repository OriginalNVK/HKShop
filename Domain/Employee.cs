using System;
using System.Collections.Generic;

namespace HKShop.Domain;

public partial class Employee
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual AppUser User { get; set; } = null!;
}
