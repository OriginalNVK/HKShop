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
            var model = new CategoriesModel()
            {
                Categories = db.Categories
                .Select(l => new CategoryProducts
                {
                    MaLoai = l.CategoryId,
                    TenLoai = l.CategoryName,
                    Products = db.Products
                        .Where(h => h.CategoryId == l.CategoryId)
                        .OrderByDescending(h => h.CreatedAt)
                        .Take(5) // Lấy 5 sản phẩm
                        .Select(h => new HangHoaResponse
                        {
                            MaHh = h.ProductId,
                            TenHH = h.ProductName,
                            DonGia = h.Price ?? 0,
                            Hinh = h.Image ?? "default.png",
                            GiamGia = h.Discount
                        })
                        .ToList()
                })
                .ToList()
            };

            return View(model);
        }
    }
}
