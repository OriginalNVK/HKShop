using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;
using HKShop.Services.Interfaces;

namespace HKShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) => _productService = productService;
        public async Task<IActionResult> Index(int? MaLoai, string? keyword)
        {
            var result = await _productService.GetProductsAsync(MaLoai, keyword);
            return View(result);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var result = await _productService.GetDetailAsync(id);
            if(result == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }
            return View(result);
        }
    }
}
