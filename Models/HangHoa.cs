using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class HangHoa
{
    public int MaHh { get; set; }

    public string TenHh { get; set; } = null!;

    public string? TenAlias { get; set; }

    public int MaLoai { get; set; }

    public string? MoTaDonVi { get; set; }

    public decimal? DonGia { get; set; }

    public string? Hinh { get; set; }

    public DateOnly NgaySx { get; set; }

    public decimal GiamGia { get; set; }

    public int LuotMua { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual Loai MaLoaiNavigation { get; set; } = null!;
}
