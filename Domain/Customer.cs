using System;
using System.Collections.Generic;

namespace HKShop.Domain;

public partial class Customer
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Fullname { get; set; } = null!;

    public bool Gender { get; set; }

    public DateOnly Birthday { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? Avatar { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual AppUser User { get; set; } = null!;
}
