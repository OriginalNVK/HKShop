using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class NguoiDung
{
    public int Id { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int VaiTro { get; set; }

    public bool HieuLuc { get; set; }

    public DateTime NgayTao { get; set; }

    public string? RandomKey { get; set; }

    public virtual KhachHang? KhachHang { get; set; }

    public virtual NhanVien? NhanVien { get; set; }
}
