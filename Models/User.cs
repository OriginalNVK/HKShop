using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class User
{
    public string MaKh { get; set; } = null!;

    public string? MatKhau { get; set; }

    public string HoTen { get; set; } = null!;

    public bool GioiTinh { get; set; }

    public DateOnly NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? DienThoai { get; set; }

    public string Email { get; set; } = null!;

    public string? Hinh { get; set; }

    public bool HieuLuc { get; set; }

    public int VaiTro { get; set; }

    public string? RandomKey { get; set; }
}
