using AutoMapper;
using HKShop.Models;
using HKShop.Helpers;
using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Reflection.Metadata;

namespace HKShop.Controllers
{
    public class ClientsController : Controller
    {
        private const string ToastMessage = "ToastMessage";
        private const string ToastType = "ToastType";
        private const string ToastTypeSuccess = "success";
        private const string ToastTypeError = "error";
        private readonly DBContext db;
        private readonly IMapper _mapper;

        private readonly ICloudinaryService _cloudinaryService;

        public ClientsController(DBContext db, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            this.db = db;
            this._mapper = mapper;
            this._cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        [Route("/admin/clients/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(ClientRequest client, IFormFile? image)
        {
            if (string.IsNullOrWhiteSpace(client.MatKhau))
            {
                TempData[ToastMessage] = "Password is required";
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }

            var clientExist = await db.KhachHangs.FirstOrDefaultAsync(k => k.MaKh == client.MaKH);
            if (clientExist != null)
            {
                TempData[ToastMessage] = "User already exists";
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }

            var accountExists = await db.NguoiDungs.AnyAsync(u => u.TenDangNhap == client.MaKH);
            if (accountExists)
            {
                TempData[ToastMessage] = "Username already exists";
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }

            if (!ModelState.IsValid)
            {
                TempData[ToastMessage] = "Invalid data";
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }
            try
            {
                var userInfo = _mapper.Map<NguoiDung>(client);
                var clientInfo = _mapper.Map<KhachHang>(client);

                userInfo.TenDangNhap = client.MaKH;
                userInfo.NgayTao = DateTime.Now;
                userInfo.RandomKey = Utils.GenerateRandomKey();
                userInfo.MatKhau = client.MatKhau.ToMd5Hash(userInfo.RandomKey);
                userInfo.HieuLuc = true; // sẽ xử lý khi dùng mail để active
                userInfo.VaiTro = client.VaiTro;

                if (image != null)
                {
                    clientInfo.Hinh = await _cloudinaryService.UploadImageAsync(image, Constants.FOLDER_CLOUDINARY_CUSTOMER);
                }

                await using var transaction = await db.Database.BeginTransactionAsync();

                await db.NguoiDungs.AddAsync(userInfo);
                await db.SaveChangesAsync();

                clientInfo.UserId = userInfo.Id;

                await db.KhachHangs.AddAsync(clientInfo);
                await db.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData[ToastMessage] = "Create user successfully";
                TempData[ToastType] = ToastTypeSuccess;
                return Redirect("/admin/clients");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData[ToastMessage] = "System error: " + ex.Message;
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }

        }

        [HttpGet]
        [Route("/admin/client/update/{id}")]
        public async Task<IActionResult> Update(string? id)
        {
            var Client = await db.KhachHangs.FirstOrDefaultAsync(c => c.MaKh == id);
            // DTO
            var result = new ClientResponse
            {
                MaKH = Client.MaKh,
                HoTen = Client.HoTen,
                GioiTinh = Client.GioiTinh,
                NgaySinh = Client.NgaySinh,
                DienThoai = Client.DienThoai,
                DiaChi = Client.DiaChi,
                Email = Client.Email,
                Hinh = Client.Hinh,
            };
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MaKH, HoTen, GioiTinh, NgaySinh, DiaChi, DienThoai, Email")] ClientRequest Client,
            IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                // Xử lý lưu ảnh
                if (Hinh != null && Hinh.Length > 0)
                {
                    Client.Hinh = await _cloudinaryService.UploadImageAsync(Hinh, Constants.FOLDER_CLOUDINARY_CUSTOMER);
                }

                var ExistsClient = await db.KhachHangs.FirstOrDefaultAsync(c => c.MaKh == Client.MaKH);
                if (ExistsClient != null)
                {
                    ExistsClient.HoTen = Client.HoTen;
                    ExistsClient.GioiTinh = Client.GioiTinh;
                    ExistsClient.NgaySinh = DateOnly.Parse(Client.NgaySinh.ToString());
                    ExistsClient.DienThoai = Client.DienThoai;
                    ExistsClient.DiaChi = Client.DiaChi;
                    ExistsClient.Email = Client.Email;
                    if (Client.Hinh != null)
                    {
                        ExistsClient.Hinh = Client.Hinh;
                    }
                    db.KhachHangs.Update(ExistsClient);
                    await db.SaveChangesAsync();
                    TempData[ToastMessage] = "Update user information successfully";
                    TempData[ToastType] = ToastTypeSuccess;
                    return Redirect("/admin/clients");
                }
                TempData[ToastMessage] = "User not found";
                TempData[ToastType] = ToastTypeError;
                return NotFound();
            }
            TempData[ToastMessage] = "Failed to update user information";
            TempData[ToastType] = ToastTypeError;
            return Redirect("/admin/clients/update/" + Client.MaKH);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var Client = await db.KhachHangs.SingleOrDefaultAsync(k => k.MaKh == id);
            if (Client != null)
            {
                var HoaDons = await db.HoaDons.Where(h => h.MaKh == id).ToListAsync();
                foreach (var h in HoaDons)
                {
                    var Cthhs = await db.ChiTietHds.Where(h => h.MaHd == h.MaHd).ToListAsync();
                    db.ChiTietHds.RemoveRange(Cthhs);
                    db.Remove(h);
                    await db.SaveChangesAsync();
                }
                db.KhachHangs.Remove(Client);
                await db.SaveChangesAsync();
                TempData[ToastMessage] = "Delete user successfully";
                TempData[ToastType] = ToastTypeSuccess;
                return Redirect("/admin/clients");
            }
            TempData[ToastMessage] = "User not found";
            TempData[ToastType] = ToastTypeError;
            return NotFound();

        }
    }
}
