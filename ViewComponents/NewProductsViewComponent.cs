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
                Categories = db.Loais
                .Select(l => new CategoryProducts
                {
                    MaLoai = l.Maloai,
                    TenLoai = l.Tenloai,
                    Products = db.HangHoas
                        .Where(h => h.MaLoai == l.Maloai)
                        .OrderByDescending(h => h.NgaySx) // Sắp xếp theo sản phẩm mới nhất
                        .Take(5) // Lấy 5 sản phẩm
                        .Select(h => new HangHoaResponse
                        {
                            MaHh = h.MaHh,
                            TenHH = h.TenHh,
                            DonGia = h.DonGia ?? 0,
                            Hinh = h.Hinh ?? "default.png",
                            GiamGia = h.GiamGia
                        })
                        .ToList()
                })
                .ToList()
            };

            return View(model);
        }
    }
}
