using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? AliasName { get; set; }

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? Image { get; set; }

    public DateOnly CreatedAt { get; set; }

    public decimal Discount { get; set; }

    public int Views { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<DetailInvoice> DetailInvoices { get; set; } = new List<DetailInvoice>();

    public virtual Category Category { get; set; } = null!;
}
