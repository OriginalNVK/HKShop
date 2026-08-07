using System;
using System.Collections.Generic;

namespace HKShop.Domain;

public partial class Cart
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<DetailCart> DetailCarts { get; set; } = new List<DetailCart>();
}
