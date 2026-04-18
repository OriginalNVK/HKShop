using HKShop.DTOs;

namespace HKShop.Services.Interfaces;

public interface IAdminOrderService
{
	Task<List<DetailInvoiceVM>> GetDetailAsync(int invoiceId, CancellationToken cancellationToken = default);
	Task<ServiceResult> DeleteAsync(int invoiceId, CancellationToken cancellationToken = default);
	Task<ServiceResult> ConfirmAsync(int invoiceId, DateTime deliveryDate, string adminId, CancellationToken cancellationToken = default);
}
