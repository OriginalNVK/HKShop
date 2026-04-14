using HKShop.Models;
using HKShop.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Controllers
{
    [Authorize(Roles = "1")]
    public class AdminController : Controller
    {
        private readonly DBContext db;

        public AdminController(DBContext db)
        {
            this.db = db;
        }

        [Route("/admin")]
        public async Task<IActionResult> Index()
        {
            var slDonHang = await db.HoaDons.CountAsync();
            var slHangHoa = await db.HangHoas.CountAsync();
            var result = new ThongKeVM
            {
                SlDonHang = slDonHang,
                SlHangHoa = slHangHoa
            };
            return View(result);
        }

        [Route("/admin/orders")]
        public async Task<IActionResult> Orders()
        {
            var result = await (
                from i in db.HoaDons.AsNoTracking()
                join k in db.KhachHangs.AsNoTracking() on i.MaKh equals k.MaKh into khachHangGroup
                from kh in khachHangGroup.DefaultIfEmpty()
                orderby i.NgayDat descending
                select new InvoiceResponse
                {
                    MaHd = i.MaHd,
                    HoTen = i.HoTen ?? (kh != null ? kh.HoTen : "Khach le"),
                    NgayDat = i.NgayDat,
                    DiaChi = i.DiaChi,
                    CachThanhToan = i.CachThanhToan,
                    CachVanChuyen = i.CachVanChuyen,
                    TrangThai = i.MaTrangThai == 0 ? "pending"
                        : i.MaTrangThai == 1 ? "paid"
                        : i.MaTrangThai == 2 ? "processing"
                        : i.MaTrangThai == 3 ? "completed"
                        : "cancelled",
                    GhiChu = i.GhiChu ?? "",
                    DienThoai = i.DienThoai
                }
            ).ToListAsync();
            return View(result);
        }

        [Route("/admin/products")]
        public async Task<IActionResult> Products(int pageNumber = 1, int pageSize = 5, int? maLoai = null)
        {
            var HangHoas = db.HangHoas.Include(h => h.MaLoaiNavigation).AsQueryable();
            if (maLoai.HasValue)
            {
                HangHoas = HangHoas.Where(h => h.MaLoai == maLoai.Value);
            }
            var totalProducts = await HangHoas.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var HangHoaDTO = await HangHoas.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(h => new HangHoaResponse
            {
                MaHh = h.MaHh,
                TenHH = h.TenHh,
                MoTaNgan = !string.IsNullOrEmpty(h.MoTa) ? h.MoTa : "Không có mô tả",
                Hinh = !string.IsNullOrEmpty(h.Hinh) ? h.Hinh : null,
                DonGia = h.DonGia ?? 0,
                GiamGia = h.GiamGia,
                TenLoai = h.MaLoaiNavigation.Tenloai,
            }).ToListAsync();
            var Categories = await db.Loais.Select(l => new MenuLoai
            {
                MaLoai = l.Maloai,
                TenLoai = l.Tenloai,
                SoLuong = l.HangHoas.Count
            }).ToListAsync();

            ViewBag.Categories = Categories;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentMaLoai = maLoai;

            return View(HangHoaDTO);
        }

        [Route("/admin/clients")]
        public async Task<IActionResult> Clients(int pageNumber = 1, int pageSize = 5, int? VaiTro = null)
        {
            // Join KhachHang + NguoiDung để lấy thông tin hồ sơ và quyền từ 2 bảng riêng.
            var query = db.KhachHangs.AsNoTracking()
                .Join(
                    db.NguoiDungs.AsNoTracking(),
                    kh => kh.UserId,
                    nd => nd.Id,
                    (kh, nd) => new ClientResponse
                    {
                        MaKH = kh.MaKh,
                        HoTen = kh.HoTen,
                        Hinh = kh.Hinh,
                        GioiTinh = kh.GioiTinh,
                        NgaySinh = kh.NgaySinh,
                        DienThoai = kh.DienThoai,
                        DiaChi = kh.DiaChi,
                        Email = kh.Email,
                        VaiTro = nd.VaiTro,
                        MatKhau = nd.MatKhau
                    });

            if (VaiTro.HasValue)
            {
                query = query.Where(c => c.VaiTro == VaiTro.Value);
            }

            var totalCount = await query.CountAsync();

            // Phân trang
            var pagedList = await query
                .OrderBy(c => c.MaKH)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Lấy danh sách các VaiTro duy nhất
            var uniqueRoles = await db.NguoiDungs
                .Select(c => c.VaiTro)
                .Distinct()
                .ToListAsync();

            ViewBag.Roles = uniqueRoles;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(pagedList);
        }

        [Route("/admin/categories")]
        public async Task<IActionResult> Categories()
        {
            var result = await db.Loais.Select(c => new CategoryResponse
            {
                MaLoai = c.Maloai,
                TenLoai = c.Tenloai,
                TenLoaiAlias = c.Tenloaialias,
                MoTa = c.Mota,
                Hinh = c.Hinh,
            }).ToListAsync();
            return View(result);
        }
    }
}
