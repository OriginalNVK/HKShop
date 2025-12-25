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
            var data = db.Loais.Select(l => new MenuLoai
            {
                MaLoai = l.Maloai,
                TenLoai = l.Tenloai,
                SoLuong = l.HangHoas.Count
            }).OrderBy(p => p.TenLoai);

            return View(data);
        }
    }
}
