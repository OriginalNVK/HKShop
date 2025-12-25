using HKShop.Models;
using HKShop.Helpers;
using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HKShop.ViewComponents
{
    public class CartPanelViewComponent : ViewComponent
    {
        private readonly DBContext context;

        public CartPanelViewComponent(DBContext context)
        {
            this.context = context;
        }
        public IViewComponentResult Invoke()
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(Constants.CLAIM_CUSTOMERID)?.Value : null;
            if (maKH == null)
            {
                return View(new GioHangModel
                {
                    Quantity = 0,
                    Total = 0,
                    Items = new List<GioHangItem>()
                });
            }

            var gioHangItems = context.Carts.Where(c => c.MaKh == maKH).Select(c => new GioHangItem
            {
                MaHH = c.MaHh,
                TenHH = c.MaHhNavigation.TenHh,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong,
                Hinh = c.MaHhNavigation.Hinh
            }).ToList();
            return View(new GioHangModel()
            {
                Quantity = gioHangItems.Sum(p => p.SoLuong),
                Total = (decimal)gioHangItems.Sum(p => p.ThanhTien),
                Items = gioHangItems
            });
        }
    }
}
