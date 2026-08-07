using HKShop.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HKShop.Services.Interfaces;

namespace HKShop.Controllers
{
    [Authorize(Roles = "1")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Route("/admin")]
        public async Task<IActionResult> Index()
        {
            var result = await _adminService.GetOverviewAsync();
            return View(result);
        }

        [Route("/admin/orders")]
        public async Task<IActionResult> Orders()
        {
            var result = await _adminService.GetOrdersAsync();
            return View(result);
        }

        [Route("/admin/products")]
        public async Task<IActionResult> Products(int pageNumber = 1, int pageSize = 5, int? maLoai = null)
        {
            var result = await _adminService.GetProductsAsync(pageNumber, pageSize, maLoai);

            ViewBag.Categories = result.Categories;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentMaLoai = maLoai;

            return View(result.Products);
        }

        [Route("/admin/clients")]
        public async Task<IActionResult> Clients(int pageNumber = 1, int pageSize = 5, int? VaiTro = null)
        {
            var result = await _adminService.GetClientsAsync(pageNumber, pageSize, VaiTro);

            ViewBag.Roles = result.Roles;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;

            return View(result.Clients);
        }

        [Route("/admin/categories")]
        public async Task<IActionResult> Categories()
        {
            var result = await _adminService.GetCategoriesAsync();
            return View(result);
        }
    }
}
