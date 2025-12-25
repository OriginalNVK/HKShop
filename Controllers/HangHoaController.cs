using HKShop.Models;
using HKShop.DTOs;
using HKShop.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly DBContext db;

        public HangHoaController(DBContext context) => db = context;
        public async Task<IActionResult> Index(int? MaLoai, string? keyword)
        {
            var HangHoas = db.HangHoas.AsQueryable();
            var result = new List<HangHoaResponse>();

            if (MaLoai.HasValue && MaLoai.Value != 0)
            {
                HangHoas = HangHoas.Where(p => p.MaLoai == MaLoai.Value);
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                HangHoas = HangHoas.Where(p => p.TenHh.Contains(keyword));
            }

            result = await HangHoas.Select(p => new HangHoaResponse
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.Tenloai
            }).ToListAsync();
            return View(result);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var data = await db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefaultAsync(p => p.MaHh == id);
            if(data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            var result = new ChiTietHangHoaResponse()
            {
                MaHH = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                DiemDanhGia = 5,// check sau
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.Tenloai,
                SoLuongTon = 10 // check sau
            };
            return View(result);
        }
    }
}
