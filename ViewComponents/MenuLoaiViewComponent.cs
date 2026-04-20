using HKShop.Models;
using HKShop.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HKShop.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly DBContext db;
        public MenuLoaiViewComponent(DBContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Categories.Select(l => new CategoryMenuDto
            {
                CategoryId = l.CategoryId,
                CategoryName = l.CategoryName,
                ProductCount = l.Products.Count
            }).OrderBy(p => p.CategoryName);

            return View(data);
        }
    }
}
