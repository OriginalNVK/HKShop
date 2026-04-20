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
                return View(new CartSummaryDto
                {
                    TotalQuantity = 0,
                    Total = 0,
                    CartItems = new List<CartItemDto>()
                });
            }

            var gioHangItems = context.Carts.Where(c => c.CustomerId == maKH).Select(c => new CartItemDto
            {
                ProductId = c.ProductId,
                ProductName = c.ProductIdNavigation.ProductName,
                Price = c.Amount,
                Quantity = c.Quantity,
                ImageUrl = c.ProductIdNavigation.Image ?? string.Empty
            }).ToList();
            return View(new CartSummaryDto()
            {
                TotalQuantity = gioHangItems.Sum(p => p.Quantity),
                Total = gioHangItems.Sum(p => p.LineTotal),
                CartItems = gioHangItems
            });
        }
    }
}
