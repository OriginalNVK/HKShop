using HKShop.Models;
using System.ComponentModel.DataAnnotations;
namespace HKShop.DTOs
{
    public class ProductsRequest
    {
        public int MaHh { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        public string TenHh { get; set; } = string.Empty;

        public string? TenAlias { get; set; }

        [Required(ErrorMessage = "Please select a product category")]
        public int? MaLoai { get; set; }

        public string? MoTaDonVi { get; set; }

        public decimal? DonGia { get; set; }

        public IFormFile? Hinh { get; set; }

        [Required(ErrorMessage = "Production Date is required")]
        public DateTime NgaySx { get; set; }

        [Range(0, 100)]
        public decimal? GiamGia { get; set; }

        public int? LuotMua { get; set; }

        public string? MoTa { get; set; }
        public virtual Loai? MaLoaiNavigation { get; set; }
    }
}
