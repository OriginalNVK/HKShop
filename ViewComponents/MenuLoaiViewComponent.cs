using HKShop.Models;
using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HShop.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly DBContext db;
        public MenuLoaiViewComponent(DBContext context) => db = context;

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
