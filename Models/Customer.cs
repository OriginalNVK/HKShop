using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class Customer
{
    public string CustomerId { get; set; } = null!;

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public bool Sex { get; set; }

    public DateOnly BirthDate { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public string? Image { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual User User { get; set; } = null!;
}
