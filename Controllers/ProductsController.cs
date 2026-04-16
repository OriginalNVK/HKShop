using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HKShop.Models;
using HKShop.DTOs;
using Microsoft.AspNetCore.Authorization;
using HKShop.Helpers;

namespace HKShop.Controllers
{

    [Authorize(Roles = "1")]
    public class ProductsController : Controller
    {
        private readonly DBContext db;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(DBContext db, ICloudinaryService cloudinaryService)
        {
            this.db = db;
            _cloudinaryService = cloudinaryService;
        }

        // GET: HangHoas/Details/5
        [Route("/admin/products/detail/{id}")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await db.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);

            if (product == null)
            {
                return NotFound();
            }

            var ProductDTO = new ProductsResponse
            {
                MaHh = product.MaHh,
                TenHh = product.TenHh,
                TenAlias = product.TenAlias,
                MaLoai = product.MaLoai,
                MoTaDonVi = product.MoTaDonVi,
                DonGia = product.DonGia,
                Hinh = product.Hinh,
                NgaySx = product.NgaySx,
                GiamGia = product.GiamGia,
                LuotMua = product.LuotMua,
                MoTa = product.MoTa,
                MaLoaiNavigation = product.MaLoaiNavigation,
            };

            return View(ProductDTO);
        }

        // GET: HangHoas/Create

        [HttpGet]
        [Route("/admin/products/create")]
        public IActionResult Create()
        {
            ViewData["Maloai"] = new SelectList(db.Loais, "Maloai", "Tenloai");
            return View();
        }

        // POST: HangHoas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([Bind("MaHh,TenHh,TenAlias,MaLoai,MoTaDonVi,DonGia,NgaySx,GiamGia,LuotMua,MoTa")] ProductsRequest hangHoa,
            IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                var product = new HangHoa
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    TenAlias = hangHoa.TenAlias,
                    MaLoai = hangHoa.MaLoai ?? 0,
                    MoTaDonVi = hangHoa.MoTaDonVi,
                    DonGia = hangHoa.DonGia,
                    NgaySx = DateOnly.FromDateTime(hangHoa.NgaySx),
                    GiamGia = hangHoa.GiamGia ?? 0,
                    LuotMua = hangHoa.LuotMua ?? 0,
                    MoTa = hangHoa.MoTa,
                };
                // Xử lý lưu ảnh
                if (Hinh != null && Hinh.Length > 0)
                {
                    try
                    {
                        product.Hinh = await _cloudinaryService.UploadImageAsync(Hinh, Constants.FOLDER_CLOUDINARY_PRODUCT);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "Lỗi khi tải ảnh lên: " + ex.Message);
                        ViewData["Maloai"] = new SelectList(db.Loais, "Maloai", "Tenloai", hangHoa.MaLoai);
                        return View("Create", hangHoa);
                    }
                }
                

                await db.HangHoas.AddAsync(product);
                await db.SaveChangesAsync();
                return Redirect("/admin/products");
            }

            // Nếu có lỗi, trả về View với dữ liệu cũ
            ViewData["Maloai"] = new SelectList(db.Loais, "Maloai", "Tenloai", hangHoa.MaLoai);
            return Redirect("/admin/products/create");
        }

        // GET: HangHoas/Edit/5
        [Route("admin/products/update/{id}")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await db.HangHoas.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var ProductDTO = new ProductsResponse
            {
                MaHh = product.MaHh,
                TenHh = product.TenHh,
                TenAlias = product.TenAlias,
                MaLoai = product.MaLoai,
                MoTaDonVi = product.MoTaDonVi,
                DonGia = product.DonGia,
                Hinh = product.Hinh,
                NgaySx = product.NgaySx,
                GiamGia = product.GiamGia,
                LuotMua = product.LuotMua,
                MoTa = product.MoTa,
                MaLoaiNavigation = product.MaLoaiNavigation,
            };
            ViewData["MaLoai"] = new SelectList(db.Loais, "Maloai", "Tenloai", product.MaLoai);
            return View(ProductDTO);
        }

        // POST: HangHoas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHh,TenHh,TenAlias,MaLoai,MoTaDonVi,DonGia,Hinh,NgaySx,GiamGia,LuotMua,MoTa")] ProductsRequest product)
        {
            if (id != product.MaHh)
            {
                return NotFound();
            }
            try
            {
                var existingProduct = await db.HangHoas.FirstOrDefaultAsync(h => h.MaHh == id);
                // Gán các giá trị từ ViewModel sang Entity
                existingProduct.TenHh = product.TenHh;
                existingProduct.TenAlias = product.TenAlias;
                existingProduct.MaLoai = product.MaLoai??0;
                existingProduct.MoTaDonVi = product.MoTaDonVi;
                existingProduct.DonGia = product.DonGia;
                if (product.Hinh != null)
                {
                    existingProduct.Hinh = await _cloudinaryService.UploadImageAsync(product.Hinh, Constants.FOLDER_CLOUDINARY_PRODUCT);
                }
                existingProduct.NgaySx = DateOnly.FromDateTime(product.NgaySx);
                existingProduct.GiamGia = product.GiamGia ?? 0;
                existingProduct.LuotMua = product.LuotMua ?? 0;
                existingProduct.MoTa = product.MoTa;

                // Cập nhật và lưu thay đổi
                db.HangHoas.Update(existingProduct);
                await db.SaveChangesAsync();

                return Redirect("/admin/products");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HangHoaExists(product.MaHh))
                {
                    return NotFound();
                }
                else
                {
                    TempData["Message"] = "Model state is invalid";
                    return Redirect("/admin");
                }
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cthhs = await db.ChiTietHds.FirstOrDefaultAsync(c => c.MaHh == id);
            if (cthhs == null)
            {
                var hangHoa = await db.HangHoas.FindAsync(id);
                if (hangHoa != null)
                {
                    db.HangHoas.Remove(hangHoa);
                }

                await db.SaveChangesAsync();
                return Redirect("/admin/products");
            }
            TempData["ErrorMessage"] = "Không thể xóa hàng hóa vì đang được sử dụng trong hóa đơn!";
            return Redirect("/admin/products");
        }

        private bool HangHoaExists(int id)
        {
            return db.HangHoas.Any(e => e.MaHh == id);
        }
    }
}
