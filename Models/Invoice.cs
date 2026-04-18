using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public string CustomerId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public DateOnly? DateNeeded { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public string? CustomerName { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int StatusCode { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string ShippingMethod { get; set; } = null!;

    public decimal ShippingFee { get; set; }

    public string? AdminId { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<DetailInvoice> DetailInvoices { get; set; } = new List<DetailInvoice>();

    public virtual Customer CustomerIdNavigation { get; set; } = null!;

    public virtual Admin? AdminIdNavigation { get; set; }
}
