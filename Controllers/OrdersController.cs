using HKShop.Models;
using HKShop.Helpers;
using HKShop.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace HKShop.Controllers
{
    [Authorize(Roles="1")]
    public class OrdersController : Controller
    {
        private readonly DBContext db;

        public OrdersController(DBContext db)
        {
            this.db = db;
        }

        [Route("Admin/Orders/Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var hoaDons = await db.ChiTietHds
                           .Include(h => h.MaHhNavigation)
                           .Where(h => h.MaHd == id).ToListAsync();

            if (hoaDons == null)
            {
                return NotFound();
            }

            var result = hoaDons.Select(h => new DetailInvoiceVM
            {
                MaCt = h.MaCt,
                MaHd = h.MaHd,
                MaHh = h.MaHh,
                DonGia = h.DonGia,
                SoLuong = h.SoLuong,
                GiamGia = h.GiamGia,
                TenHangHoa = h.MaHhNavigation?.TenHh,
                Hinh = h.MaHhNavigation?.Hinh,
                ThanhTien = h.DonGia * h.SoLuong
            }).ToList();

            return View(result);
        }

        [Route("Admin/Orders/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var donHang = await db.HoaDons.FirstOrDefaultAsync(h => h.MaHd == id);
            var cthds = await db.ChiTietHds.Where(c => c.MaHd == id).ToListAsync();
            db.HoaDons.Remove(donHang);
            db.ChiTietHds.RemoveRange(cthds);
            await db.SaveChangesAsync();
            return Redirect("/Admin/Orders");
        }

        public async Task<IActionResult> Confirm(int id, DateTime deliveryDate)
        {
			if (deliveryDate < DateTime.Now.Date)
			{
				TempData["ErrorMessage"] = "Delivery date cannot be in the past";
                return Redirect("/Admin/Orders/Detail/" + id);
			}
			using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
					var donHang = await db.HoaDons.SingleOrDefaultAsync(h => h.MaHd == id);
					if (donHang == null)
					{
						return NotFound();
					}
					donHang.NgayGiao = DateOnly.Parse(deliveryDate.ToString());
					donHang.MaTrangThai = 2;
					var MaKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(Helpers.Constants.CLAIM_CUSTOMERID)?.Value : "Guest";
					if (MaKH == "Guest")
					{
						return Redirect("/404");
					}
					donHang.MaNv = MaKH;
                    await db.SaveChangesAsync();
					await transaction.CommitAsync();
					TempData["SuccessMessage"] = "Order confirmed successfully";
					return Redirect("/Admin/Orders");
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					// Log lỗi ở đây (sử dụng ILogger)
					TempData["ErrorMessage"] = "There was an error confirming the order";
					return Redirect("/Admin/Orders/Detail/" + id);
				}
			}
				
        }
    }
}
