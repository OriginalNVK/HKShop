using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public int UserId { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DienThoai { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual NguoiDung User { get; set; } = null!;
}
