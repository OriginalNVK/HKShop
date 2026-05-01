using System.Linq;

namespace HKShop.DTOs;

public class HangHoaResponse : ProductCardViewModel
{
    public int MaHh
    {
        get => ProductId;
        set => ProductId = value;
    }

    public string TenHH
    {
        get => ProductName;
        set => ProductName = value;
    }

    public string Hinh
    {
        get => ImageUrl ?? string.Empty;
        set => ImageUrl = value;
    }

    public decimal DonGia
    {
        get => Price ?? 0m;
        set => Price = value;
    }

    public string MoTaNgan { get; set; } = string.Empty;

    public string TenLoai
    {
        get => CategoryName;
        set => CategoryName = value;
    }

    public decimal GiamGia
    {
        get => Discount;
        set => Discount = value;
    }
}

public class ChiTietHangHoaResponse : ProductDetailDto
{
    public int MaHH
    {
        get => ProductId;
        set => ProductId = value;
    }

    public string TenHH
    {
        get => ProductName;
        set => ProductName = value;
    }

    public string Hinh
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }

    public decimal DonGia
    {
        get => Price;
        set => Price = value;
    }

    public string MoTaNgan
    {
        get => ShortDescription;
        set => ShortDescription = value;
    }

    public string TenLoai
    {
        get => CategoryName;
        set => CategoryName = value;
    }

    public string ChiTiet
    {
        get => Description;
        set => Description = value;
    }

    public int DiemDanhGia { get; set; }

    public int SoLuongTon { get; set; }
}

public class ProductsRequest : ProductRequestDto
{
    public int MaHh
    {
        get => ProductId;
        set => ProductId = value;
    }

    public string TenHh
    {
        get => ProductName;
        set => ProductName = value;
    }

    public string? TenAlias
    {
        get => AliasName;
        set => AliasName = value;
    }

    public int? MaLoai
    {
        get => CategoryId;
        set => CategoryId = value;
    }

    public string? MoTaDonVi
    {
        get => UnitDescription;
        set => UnitDescription = value;
    }

    public decimal? DonGia
    {
        get => Price;
        set => Price = value;
    }

    public IFormFile? Hinh
    {
        get => ImageFile;
        set => ImageFile = value;
    }

    public DateTime NgaySx
    {
        get => ManufactureDate;
        set => ManufactureDate = value;
    }

    public decimal? GiamGia
    {
        get => Discount;
        set => Discount = value;
    }

    public int? LuotMua
    {
        get => Views;
        set => Views = value;
    }

    public string? MoTa
    {
        get => Description;
        set => Description = value;
    }

    public object? MaLoaiNavigation
    {
        get => Category;
        set => Category = value;
    }
}

public class ProductsResponse : ProductResponseDto
{
    public int MaHh
    {
        get => ProductId;
        set => ProductId = value;
    }

    public string TenHh
    {
        get => ProductName;
        set => ProductName = value;
    }

    public string? TenAlias
    {
        get => AliasName;
        set => AliasName = value;
    }

    public int MaLoai
    {
        get => CategoryId;
        set => CategoryId = value;
    }

    public string? MoTaDonVi
    {
        get => UnitDescription;
        set => UnitDescription = value;
    }

    public decimal? DonGia
    {
        get => Price;
        set => Price = value;
    }

    public string? Hinh
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }

    public DateOnly NgaySx
    {
        get => ManufactureDate;
        set => ManufactureDate = value;
    }

    public decimal GiamGia
    {
        get => Discount;
        set => Discount = value;
    }

    public int LuotMua
    {
        get => Views;
        set => Views = value;
    }

    public string? MoTa
    {
        get => Description;
        set => Description = value;
    }

    public object? MaLoaiNavigation
    {
        get => Category;
        set => Category = value;
    }
}

public class ClientRequest : CustomerRequestDto
{
    public string MaKH
    {
        get => CustomerId;
        set => CustomerId = value;
    }

    public string? MatKhau
    {
        get => Password;
        set => Password = value;
    }

    public string HoTen
    {
        get => FullName;
        set => FullName = value;
    }

    public bool GioiTinh
    {
        get => Gender;
        set => Gender = value;
    }

    public DateOnly NgaySinh
    {
        get => BirthDate;
        set => BirthDate = value;
    }

    public string? DiaChi
    {
        get => Address;
        set => Address = value;
    }

    public string? DienThoai
    {
        get => PhoneNumber;
        set => PhoneNumber = value;
    }

    public int VaiTro
    {
        get => Role;
        set => Role = value;
    }

    public string? Hinh
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }
}

public class ClientResponse : CustomerResponseDto
{
    public string MaKH
    {
        get => CustomerId;
        set => CustomerId = value;
    }

    public string? MatKhau
    {
        get => Password;
        set => Password = value;
    }

    public string HoTen
    {
        get => FullName;
        set => FullName = value;
    }

    public bool GioiTinh
    {
        get => Gender;
        set => Gender = value;
    }

    public DateOnly NgaySinh
    {
        get => BirthDate;
        set => BirthDate = value;
    }

    public string? DiaChi
    {
        get => Address;
        set => Address = value;
    }

    public string? DienThoai
    {
        get => PhoneNumber;
        set => PhoneNumber = value;
    }

    public int VaiTro
    {
        get => Role;
        set => Role = value;
    }

    public string? Hinh
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }
}

public class DangKyRequest : RegisterRequestDto
{
    public string TenDangNhap
    {
        get => Username;
        set => Username = value;
    }

    public string? MatKhau
    {
        get => Password;
        set => Password = value;
    }

    public string HoTen
    {
        get => FullName;
        set => FullName = value;
    }

    public bool GioiTinh
    {
        get => Gender;
        set => Gender = value;
    }

    public DateOnly? NgaySinh
    {
        get => BirthDate;
        set => BirthDate = value;
    }

    public string DiaChi
    {
        get => Address;
        set => Address = value;
    }

    public string DienThoai
    {
        get => PhoneNumber;
        set => PhoneNumber = value;
    }

    public string? Hinh
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }
}

public class DangNhapRequest : LoginRequestDto
{
}

public class CheckoutVM : CheckoutRequestDto
{
    public bool GiongKhachHang
    {
        get => UseCustomerProfile;
        set => UseCustomerProfile = value;
    }

    public string? HoTen
    {
        get => FullName;
        set => FullName = value;
    }

    public string? DiaChi
    {
        get => Address;
        set => Address = value;
    }

    public string? DienThoai
    {
        get => PhoneNumber;
        set => PhoneNumber = value;
    }

    public string? GhiChu
    {
        get => Notes;
        set => Notes = value;
    }
}

public class DetailInvoiceVM : InvoiceDetailDto
{
    public int MaCt
    {
        get => DetailInvoiceId;
        set => DetailInvoiceId = value;
    }

    public int MaHd
    {
        get => InvoiceId;
        set => InvoiceId = value;
    }

    public int MaHh
    {
        get => ProductId;
        set => ProductId = value;
    }

    public decimal DonGia
    {
        get => Price;
        set => Price = value;
    }

    public int SoLuong
    {
        get => Quantity;
        set => Quantity = value;
    }

    public decimal GiamGia
    {
        get => Discount;
        set => Discount = value;
    }

    public string Hinh
    {
        get => ProductImage;
        set => ProductImage = value;
    }

    public string TenHangHoa
    {
        get => ProductName;
        set => ProductName = value;
    }

    public decimal ThanhTien
    {
        get => TotalAmount;
        set => TotalAmount = value;
    }
}

public class CategoryResponse : CategoryDto
{
    public int MaLoai
    {
        get => CategoryId;
        set => CategoryId = value;
    }

    public string TenLoai
    {
        get => CategoryName;
        set => CategoryName = value;
    }

    public string? TenLoaiAlias
    {
        get => CategoryAlias;
        set => CategoryAlias = value;
    }

    public string? MoTa
    {
        get => Description;
        set => Description = value;
    }

    public string? Hinh
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }
}

public class CategoriesModel : CategoryCollectionDto
{
    public List<CategoryProducts> Categories
    {
        get => CategoryGroups;
        set => CategoryGroups = value;
    }
}

public class MenuLoai : CategoryMenuDto
{
    public int MaLoai
    {
        get => CategoryId;
        set => CategoryId = value;
    }

    public string? TenLoai
    {
        get => CategoryName;
        set => CategoryName = value;
    }

    public int SoLuong
    {
        get => ProductCount;
        set => ProductCount = value;
    }
}

public class GioHangItem : CartItemDto
{
    public int MaHH
    {
        get => ProductId;
        set => ProductId = value;
    }

    public string Hinh
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }

    public string TenHH
    {
        get => ProductName;
        set => ProductName = value;
    }

    public decimal DonGia
    {
        get => Price;
        set => Price = value;
    }

    public int SoLuong
    {
        get => Quantity;
        set => Quantity = value;
    }

    public decimal ThanhTien => LineTotal;
}

public class GioHangModel : CartSummaryDto
{
    public int Quantity
    {
        get => TotalQuantity;
        set => TotalQuantity = value;
    }

    public List<GioHangItem> Items
    {
        get => CartItems.Cast<GioHangItem>().ToList();
        set => CartItems = value.Cast<CartItemDto>().ToList();
    }
}

public class InvoiceResponse : InvoiceDto
{
    public int MaHd
    {
        get => InvoiceId;
        set => InvoiceId = value;
    }

    public string HoTen
    {
        get => CustomerName;
        set => CustomerName = value;
    }

    public DateTime NgayDat
    {
        get => OrderDate;
        set => OrderDate = value;
    }

    public string DiaChi
    {
        get => Address;
        set => Address = value;
    }

    public string CachThanhToan
    {
        get => PaymentMethod;
        set => PaymentMethod = value;
    }

    public string CachVanChuyen
    {
        get => ShippingMethod;
        set => ShippingMethod = value;
    }

    public string TrangThai
    {
        get => Status;
        set => Status = value;
    }

    public string GhiChu
    {
        get => Notes;
        set => Notes = value;
    }

    public string DienThoai
    {
        get => PhoneNumber;
        set => PhoneNumber = value;
    }
}

public class OverviewDTO : DashboardOverviewDto
{
}

public class ThongKeVM : DashboardOverviewDto
{
}
