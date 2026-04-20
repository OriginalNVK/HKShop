using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HKShop.Helpers;
using HKShop.DTOs;
using HKShop.Services.Interfaces;

namespace HKShop.Controllers
{
    public class CartController : Controller
    {   
        private readonly PaypalClient _paymentClient;
        private readonly ICartService _cartService;

		public CartController(PaypalClient paymentClient, ICartService cartService)
		{
			_paymentClient = paymentClient;
			_cartService = cartService;
		}

        public async Task<IActionResult> Index()
        {
			var customerId = HttpContext.User.Identity?.IsAuthenticated == true
                ? HttpContext.User.FindFirst(Constants.CLAIM_CUSTOMERID)?.Value
                : null;

			var cartItems = await _cartService.GetCartAsync(customerId);
            if(cartItems == null)
            {
                return Redirect("/KhachHang/DangNhap");
            }
            return View(cartItems);
        }

        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var customerId = HttpContext.User.Identity?.IsAuthenticated == true
                ? HttpContext.User.FindFirst(Constants.CLAIM_CUSTOMERID)?.Value
                : null;

            var result = await _cartService.AddToCartAsync(customerId, id, quantity);
            if (!result.Success)
            {
                if (result.Message == "Unauthorized")
                {
                    return Redirect("/KhachHang/DangNhap");
                }

                TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                return Redirect("/404");
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> RemoveCart(int id)
        {
            var customerId = HttpContext.User.FindFirst(Constants.CLAIM_CUSTOMERID)?.Value;
            await _cartService.RemoveCartItemAsync(customerId, id);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var customerId = HttpContext.User.FindFirst(Constants.CLAIM_CUSTOMERID)?.Value;
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return Redirect("/KhachHang/DangNhap");
            }

            var carts = await _cartService.GetCheckoutItemsAsync(customerId);
            if (carts.Count == 0)
            {
                return Redirect("/");
            }
            ViewBag.PaypalClientId = _paymentClient.ClientId;
            return View(carts);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutRequestDto model)
        {
            var customerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == Constants.CLAIM_CUSTOMERID)?.Value;
            if (string.IsNullOrWhiteSpace(customerID))
            {
                return Redirect("/KhachHang/DangNhap");
            }

            if (ModelState.IsValid)
            {
                var result = await _cartService.CheckoutCodAsync(customerID, model);
                if (result.Success)
                {
                    return View("Success");
                }
            }

            var CartItems = await _cartService.GetCheckoutItemsAsync(customerID);
            if (CartItems.Count == 0)
            {
                return Redirect("/");
            }
            
            return View(CartItems);
        }

        #region Payment
        [Authorize]
        [HttpPost("Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var customerId = HttpContext.User.FindFirst(Constants.CLAIM_CUSTOMERID)?.Value;
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new { Message = "Unauthorized" });
            }

            try
            {
                var response = await _cartService.CreatePaypalOrderAsync(customerId, cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new
                {
                    ex.GetBaseException().Message
                };
                return BadRequest(error);
            }
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
		}

		[Authorize]
		[HttpPost("Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder([FromQuery] string orderID, CancellationToken cancellationToken)
        {
            var customerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == Constants.CLAIM_CUSTOMERID)?.Value;
            if (string.IsNullOrWhiteSpace(customerID))
            {
                return BadRequest(new { Message = "Unauthorized" });
            }

            var result = await _cartService.CapturePaypalOrderAsync(customerID, orderID, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(result.Data);
		}
		#endregion
	}
}
