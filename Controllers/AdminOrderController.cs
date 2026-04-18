using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HKShop.Helpers;
using HKShop.Services.Interfaces;

namespace HKShop.Controllers
{
    [Authorize(Roles="1")]
    public class OrdersController : Controller
    {
        private readonly IAdminOrderService _adminOrderService;

        public OrdersController(IAdminOrderService adminOrderService)
        {
            _adminOrderService = adminOrderService;
        }

        [Route("Admin/Orders/Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var result = await _adminOrderService.GetDetailAsync(id);
            if (result.Count == 0)
            {
                return NotFound();
            }

            return View(result);
        }

        [Route("Admin/Orders/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _adminOrderService.DeleteAsync(id);
            return Redirect("/Admin/Orders");
        }

        public async Task<IActionResult> Confirm(int id, DateTime deliveryDate)
        {
            var adminId = HttpContext.User.Identity?.IsAuthenticated == true
                ? HttpContext.User.FindFirst(Constants.CLAIM_CUSTOMERID)?.Value
                : null;

            if (string.IsNullOrWhiteSpace(adminId))
            {
                return Redirect("/404");
            }

            var result = await _adminOrderService.ConfirmAsync(id, deliveryDate, adminId);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return result.Success ? Redirect("/Admin/Orders") : Redirect("/Admin/Orders/Detail/" + id);
        }
    }
}
