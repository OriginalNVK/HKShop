using HKShop.DTOs;
using HKShop.Repositories.Interfaces;
using HKShop.Services.Interfaces;

namespace HKShop.Services;

public class AdminOrderService : IAdminOrderService
{
	private readonly IInvoiceRepository _invoiceRepository;
	private readonly IDetailInvoiceRepository _detailInvoiceRepository;

	public AdminOrderService(IInvoiceRepository invoiceRepository, IDetailInvoiceRepository detailInvoiceRepository)
	{
		_invoiceRepository = invoiceRepository;
		_detailInvoiceRepository = detailInvoiceRepository;
	}

	public async Task<List<InvoiceDetailDto>> GetDetailAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		var details = await _detailInvoiceRepository.GetByInvoiceIdAsync(invoiceId, cancellationToken);

		return details
			.Select(d => new InvoiceDetailDto
			{
				DetailInvoiceId = d.DetailInvoiceId,
				InvoiceId = d.InvoiceId,
				ProductId = d.ProductId,
				Price = d.Amount,
				Quantity = d.Quantity,
				Discount = d.Discount,
				ProductName = d.ProductIdNavigation.ProductName,
				ProductImage = d.ProductIdNavigation.Image ?? string.Empty,
				TotalAmount = d.Amount * d.Quantity
			})
			.ToList();
	}

	public async Task<ServiceResult> DeleteAsync(int invoiceId, CancellationToken cancellationToken = default)
	{
		var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
		if (invoice == null)
		{
			return ServiceResult.Fail("Order not found");
		}

		await _invoiceRepository.DeleteAsync(invoiceId, cancellationToken);
		return ServiceResult.Ok("Delete order successfully");
	}

	public async Task<ServiceResult> ConfirmAsync(int invoiceId, DateTime deliveryDate, string adminId, CancellationToken cancellationToken = default)
	{
		if (deliveryDate < DateTime.Now.Date)
		{
			return ServiceResult.Fail("Delivery date cannot be in the past");
		}

		var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
		if (invoice == null)
		{
			return ServiceResult.Fail("Order not found");
		}

		var updatedStatus = await _invoiceRepository.UpdateStatusAsync(invoiceId, 2, DateOnly.FromDateTime(deliveryDate), cancellationToken);
		var assignedAdmin = await _invoiceRepository.AssignAdminAsync(invoiceId, adminId, cancellationToken);

		if (!updatedStatus || !assignedAdmin)
		{
			return ServiceResult.Fail("There was an error confirming the order");
		}

		return ServiceResult.Ok("Order confirmed successfully");
	}
}
