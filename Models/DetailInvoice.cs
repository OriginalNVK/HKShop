using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class DetailInvoice
{
    public int DetailInvoiceId { get; set; }

    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public decimal Amount { get; set; }

    public int Quantity { get; set; }

    public decimal Discount { get; set; }

    public virtual Invoice InvoiceIdNavigation { get; set; } = null!;

    public virtual Product ProductIdNavigation { get; set; } = null!;
}
