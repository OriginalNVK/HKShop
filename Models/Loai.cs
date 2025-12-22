using System;
using System.Collections.Generic;

namespace HKShop.Models;

public partial class Loai
{
    public int Maloai { get; set; }

    public string Tenloai { get; set; } = null!;

    public string? Tenloaialias { get; set; }

    public string? Mota { get; set; }

    public string? Hinh { get; set; }

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
}
