using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;
using HKShop.Repositories.Interfaces;

namespace HKShop.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public MenuLoaiViewComponent(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();

            var productCounts = products
                .GroupBy(p => p.CategoryId)
                .ToDictionary(g => g.Key, g => g.Count());

            var data = categories
                .Select(l => new CategoryMenuDto
                {
                    CategoryId = l.CategoryId,
                    CategoryName = l.CategoryName,
                    ProductCount = productCounts.GetValueOrDefault(l.CategoryId, 0)
                })
                .OrderBy(p => p.CategoryName)
                .ToList();

            return View(data);
        }
    }
}
