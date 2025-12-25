using AutoMapper;
using HKShop.Models;
using HKShop.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HKShop.DTOs;

namespace HKShop.Controllers
{
    public class KhachHangController : Controller
    {
        // private readonly IMapper _mapper;
        private readonly DBContext db;
        private readonly IGenerateToken _generateToken;

        public KhachHangController(DBContext context, IGenerateToken generateToken)
        {
            // _mapper = mapper;
            db = context;
            _generateToken = generateToken;
        }

        #region Đăng ký 
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKy(DangKyRequest model, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // var khachHang = _mapper.Map<KhachHang>(model);
                    var KhachHang = new KhachHang
                    {
                        MaKh = model.TenDangNhap,
                        HoTen = model.HoTen,
                        GioiTinh = model.GioiTinh,
                        NgaySinh = model.NgaySinh.HasValue ? model.NgaySinh.Value : DateOnly.MinValue,
                        DiaChi = model.DiaChi,
                        DienThoai = model.DienThoai,
                        Email = model.Email
                    };
                    var NguoiDung = new NguoiDung();
                    NguoiDung.TenDangNhap = model.TenDangNhap;
                    NguoiDung.NgayTao = DateTime.Now;
                    NguoiDung.RandomKey = Utils.GenerateRandomKey();
                    NguoiDung.MatKhau = model.MatKhau.ToMd5Hash(NguoiDung.RandomKey);
                    NguoiDung.HieuLuc = true; // sẽ xử lý khi dùng mail để active

                    if (Hinh != null)
                    {
                        KhachHang.Hinh = Utils.UpLoadHinh(Hinh, "KhachHang");
                    }
                    await db.NguoiDungs.AddAsync(NguoiDung);
                    await db.SaveChangesAsync();

                    // Gán UserId cho KhachHang sau khi NguoiDung đã được lưu
                    KhachHang.UserId = NguoiDung.Id;

                    await db.KhachHangs.AddAsync(KhachHang);
                    await db.SaveChangesAsync();
                    return Redirect("/");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi đăng ký tài khoản: {ex.Message}");
                }
            }
            return View(model);
        }
        #endregion

        #region Đăng nhập
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapRequest model, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var NguoiDung = db.NguoiDungs.SingleOrDefault(kh => kh.TenDangNhap == model.Username);
                if (NguoiDung == null)
                {
                    ModelState.AddModelError("Lỗi", "Sai thông tin đăng nhập");
                }
                else
                {
                    if (!NguoiDung.HieuLuc)
                    {
                        ModelState.AddModelError("Lỗi", "Tài khoản của bạn đã bị khóa vui lòng liên hệ Admin");
                    }
                    else
                    {
                        if (NguoiDung.MatKhau != model.Password.ToMd5Hash(NguoiDung.RandomKey))
                        {
                            ModelState.AddModelError("Lỗi", "Sai mật khẩu đăng nhập");
                        }
                        else
                        {
                            var khachHang = await db.KhachHangs.FirstOrDefaultAsync(kh => kh.MaKh == NguoiDung.TenDangNhap);
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim(Constants.CLAIM_CUSTOMERID, khachHang.MaKh),
                                new Claim(ClaimTypes.Role, NguoiDung.VaiTro.ToString()),
                                new Claim("Avatar", khachHang.Hinh),
                                ////claim role - động
                                //new Claim(ClaimTypes.Role, "Customer")
                            };

                            var jwtToken = _generateToken.GenerateJwtToken(claims);
                            Response.Cookies.Append("JwtToken", jwtToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTimeOffset.Now.AddMinutes(double.Parse(
                                    HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:ExpireTimespan"] ?? "60")),
                                Path = "/"
                            });
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return NguoiDung.VaiTro switch
                                {
                                    0 => Redirect("/"),
                                    1 => Redirect("/admin"),
                                    _ => Redirect("/")
                                };
                            }
                        }
                    }
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
