using HKShop.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HKShop.DTOs;
using HKShop.Services.Interfaces;

namespace HKShop.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(DangKyRequest model, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.RegisterAsync(model, Hinh);
                if (result.Success)
                {
                    return Redirect("/");
                }

                ModelState.AddModelError("Lỗi", result.Message);
            }
            return View(model);
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(DangNhapRequest model, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _customerService.LoginAsync(model, returnUrl);
                if (!result.Success)
                {
                    ModelState.AddModelError("Lỗi", result.Message);
                }
                else
                {
                    Response.Cookies.Append("JwtToken", result.Token ?? string.Empty, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.Now.AddMinutes(double.Parse(
                            HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:ExpireTimespan"] ?? "60")),
                        Path = "/"
                    });
                    return Redirect(result.RedirectUrl ?? "/");
                }
            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            Response.Cookies.Delete("JwtToken");
            return Redirect("/");
        }
    }
}
