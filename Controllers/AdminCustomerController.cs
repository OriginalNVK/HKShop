using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;
using HKShop.Services.Interfaces;

namespace HKShop.Controllers
{
    public class ClientsController : Controller
    {
        private const string ToastMessage = "ToastMessage";
        private const string ToastType = "ToastType";
        private const string ToastTypeSuccess = "success";
        private const string ToastTypeError = "error";
        private readonly IAdminCustomerService _customerService;

        public ClientsController(IAdminCustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Route("/admin/clients/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CustomerRequestDto client, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                TempData[ToastMessage] = "Invalid data";
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }
            var result = await _customerService.CreateUserAsync(client, image);
            TempData[ToastMessage] = result.Message;
            TempData[ToastType] = result.Success ? ToastTypeSuccess : ToastTypeError;

            if (result.Success)
            {
                return Redirect("/admin/clients");
            }

            return RedirectToAction("Create");

        }

        [HttpGet]
        [Route("/admin/client/update/{id}")]
        public async Task<IActionResult> Update(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var result = await _customerService.GetByIdAsync(id);
            if (result == null)
            {
                TempData[ToastMessage] = "User not found";
                TempData[ToastType] = ToastTypeError;
                return NotFound();
            }

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
                public async Task<IActionResult> Edit(CustomerRequestDto client, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
				var result = await _customerService.UpdateAsync(client, Hinh);
                TempData[ToastMessage] = result.Message;
                TempData[ToastType] = result.Success ? ToastTypeSuccess : ToastTypeError;

                if (result.Success)
                {
                    return Redirect("/admin/clients");
                }

                return NotFound();
            }
            TempData[ToastMessage] = "Failed to update user information";
            TempData[ToastType] = ToastTypeError;
			return Redirect("/admin/clients/update/" + client.CustomerId);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var result = await _customerService.DeleteAsync(id);
            TempData[ToastMessage] = result.Message;
            TempData[ToastType] = result.Success ? ToastTypeSuccess : ToastTypeError;

            if (result.Success)
            {
                return Redirect("/admin/clients");
            }

            return NotFound();

        }
    }
}
