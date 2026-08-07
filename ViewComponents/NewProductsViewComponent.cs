using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;
using HKShop.Repositories.Interfaces;

namespace HKShop.ViewComponents
{
    public class NewProductsViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public NewProductsViewComponent(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryRepository.GetAllAsync(default);
            var products = await _productRepository.GetAllAsync(default);

            var model = new CategoriesModel()
            {
                Categories = categories
                    .Select(l => new CategoryProducts
                    {
                        MaLoai = l.Id,
                        TenLoai = l.Name,
                        Products = products
                            .Where(h => h.CategoryId == l.Id)
                            .OrderByDescending(h => h.CreatedDate)
                            .Take(5)
                            .Select(h => new ProductResponseDto
                            {
                                ProductId = h.Id,
                                ProductName = h.Name,
                                Price = h.UnitPrice ?? 0,
                                ImageUrl = h.Image ?? "default.png",
                                Discount = h.Discount
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return View(model);
        }
    }
}
