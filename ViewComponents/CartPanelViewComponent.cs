using HKShop.Helpers;
using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;
using HKShop.Repositories.Interfaces;

namespace HKShop.ViewComponents
{
    public class CartPanelViewComponent : ViewComponent
    {
        private readonly ICartRepository _cartRepository;

        public CartPanelViewComponent(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
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

            var carts = await _cartRepository.GetByCustomerIdAsync(maKH);
            var gioHangItems = carts.Select(c => new GioHangItem
            {
                MaHH = c.ProductId,
                TenHH = c.ProductIdNavigation.ProductName,
                DonGia = c.Amount,
                SoLuong = c.Quantity,
                Hinh = c.ProductIdNavigation.Image
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
