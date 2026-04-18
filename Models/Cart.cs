using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public string CustomerId { get; set; } = null!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Amount { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Product ProductIdNavigation { get; set; } = null!;

    public virtual Customer CustomerIdNavigation { get; set; } = null!;
}
