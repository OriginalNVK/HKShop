using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HKShop.DTOs;
using Microsoft.AspNetCore.Authorization;
using HKShop.Services.Interfaces;

namespace HKShop.Controllers
{

    [Authorize(Roles = "1")]
    public class ProductsController : Controller
    {
        private readonly IAdminProductService _adminProductService;

        public ProductsController(IAdminProductService adminProductService)
        {
            _adminProductService = adminProductService;
        }

        // GET: HangHoas/Details/5
        [Route("/admin/products/detail/{id}")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _adminProductService.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: HangHoas/Create

        [HttpGet]
        [Route("/admin/products/create")]
        public async Task<IActionResult> Create()
        {
            var categories = await _adminProductService.GetCategoriesAsync();
            ViewData["Maloai"] = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: HangHoas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([Bind("ProductId,ProductName,AliasName,CategoryId,UnitDescription,Price,ManufactureDate,Discount,Views,Description")] ProductsRequest hangHoa,
            IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminProductService.CreateAsync(hangHoa, Hinh);
                if (result.Success)
                {
                    return Redirect("/admin/products");
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }

            var categories = await _adminProductService.GetCategoriesAsync();
            ViewData["Maloai"] = new SelectList(categories, "Id", "Name", hangHoa.CategoryId);
            return View("Create", hangHoa);
        }

        // GET: HangHoas/Edit/5
        [Route("admin/products/update/{id}")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _adminProductService.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _adminProductService.GetCategoriesAsync();
            ViewData["MaLoai"] = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: HangHoas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,AliasName,CategoryId,UnitDescription,Price,ImageFile,ManufactureDate,Discount,Views,Description")] ProductsRequest product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }
            var result = await _adminProductService.UpdateAsync(id, product);
            if (result.Success)
            {
                return Redirect("/admin/products");
            }

            TempData["Message"] = result.Message;
            return Redirect("/admin");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminProductService.DeleteAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            return Redirect("/admin/products");
        }
    }
}
