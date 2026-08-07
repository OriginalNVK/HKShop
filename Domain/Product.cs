using System;
using System.Collections.Generic;

namespace HKShop.Domain;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? UnitDescription { get; set; }

    public decimal? UnitPrice { get; set; }

    public string? Image { get; set; }

    public DateTime CreatedDate { get; set; }

    public decimal Discount { get; set; }

    public int Views { get; set; }

    public string? Description { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<DetailCart> DetailCarts { get; set; } = new List<DetailCart>();

    public virtual ICollection<DetailInvoice> DetailInvoices { get; set; } = new List<DetailInvoice>();
}
