using System;
using System.Collections.Generic;

namespace HKShop.Domain;

public partial class Invoice
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateOnly? ShipmentDate { get; set; }

    public string? ReceiverName { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int StatusId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string ShippingMethod { get; set; } = null!;

    public decimal ShippingFee { get; set; }

    public int? EmployeeId { get; set; }

    public string? Note { get; set; }

    public decimal Discount { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<DetailInvoice> DetailInvoices { get; set; } = new List<DetailInvoice>();

    public virtual Employee? Employee { get; set; }
}
