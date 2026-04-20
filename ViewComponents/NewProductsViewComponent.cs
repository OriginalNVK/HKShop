using HKShop.Models;
using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;
namespace HKShop.ViewComponents
{
    public class NewProductsViewComponent : ViewComponent
    {
        private readonly DBContext db;

        public NewProductsViewComponent(DBContext db)
        {
            this.db = db;
        }

        public IViewComponentResult Invoke()
        {
            var model = new CategoryCollectionDto()
            {
                CategoryGroups = db.Categories
                .Select(l => new CategoryProducts
                {
                    CategoryId = l.CategoryId,
                    CategoryName = l.CategoryName,
                    ProductItems = db.Products
                        .Where(h => h.CategoryId == l.CategoryId)
                        .OrderByDescending(h => h.CreatedAt)
                        .Take(5) // Lấy 5 sản phẩm
                        .Select(h => new ProductDto
                        {
                            ProductId = h.ProductId,
                            ProductName = h.ProductName,
                            Price = h.Price ?? 0,
                            ImageUrl = h.Image ?? "default.png",
                            Discount = h.Discount,
                            CategoryId = h.CategoryId,
                            Category = h.Category,
                            ManufactureDate = h.CreatedAt,
                            Views = h.Views,
                            Description = h.Description,
                            UnitDescription = h.Description
                        })
                        .ToList()
                })
                .ToList()
            };

            return View(model);
        }
    }
}
