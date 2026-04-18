using HKShop.Models;
using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HKShop.ViewComponents
{
    public class LoaiViewComponent : ViewComponent
    {
        private readonly DBContext db;
        public LoaiViewComponent(DBContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Categories.Select(l => new MenuLoai
            {
                MaLoai = l.CategoryId,
                TenLoai = l.CategoryName,
                SoLuong = l.Products.Count
            }).OrderBy(p => p.TenLoai);

            return View(data);
        }
    }
}
