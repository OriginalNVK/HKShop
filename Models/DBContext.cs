using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HKShop.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<DetailInvoice> DetailInvoices { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Admin> Admin { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Keep DI configuration from Program.cs as the source of truth.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:HKShop");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__MaCart");

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.CustomerId, e.ProductId }, "UQ_Cart_KhachHang_HangHoa").IsUnique();

            entity.Property(e => e.CartId).HasColumnName("MaCart");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("MaKH");
            entity.Property(e => e.ProductId).HasColumnName("MaHH");
            entity.Property(e => e.Quantity).HasColumnName("SoLuong");
            entity.Property(e => e.Amount)
                .HasColumnName("DonGia")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AddedAt)
                .HasColumnName("NgayThem")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ProductIdNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_HangHoa");

            entity.HasOne(d => d.CustomerIdNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Cart_KhachHang");
        });

        modelBuilder.Entity<DetailInvoice>(entity =>
        {
            entity.HasKey(e => e.DetailInvoiceId).HasName("PK__ChiTietHD__MaCT");

            entity.ToTable("ChiTietHD");

            entity.Property(e => e.DetailInvoiceId).HasColumnName("MaCT");
            entity.Property(e => e.InvoiceId).HasColumnName("MaHD");
            entity.Property(e => e.ProductId).HasColumnName("MaHH");
            entity.Property(e => e.Amount)
                .HasColumnName("DonGia")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnName("SoLuong");
            entity.Property(e => e.Discount)
                .HasColumnName("GiamGia")
                .HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.InvoiceIdNavigation).WithMany(p => p.DetailInvoices)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_ChiTietHD_HoaDon");

            entity.HasOne(d => d.ProductIdNavigation).WithMany(p => p.DetailInvoices)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHD_HangHoa");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__HangHoa__MaHH");

            entity.ToTable("HangHoa");

            entity.Property(e => e.ProductId).HasColumnName("MaHH");
            entity.Property(e => e.ProductName)
                .HasColumnName("TenHH")
                .HasMaxLength(100);
            entity.Property(e => e.AliasName)
                .HasColumnName("TenAlias")
                .HasMaxLength(100);
            entity.Property(e => e.CategoryId).HasColumnName("MaLoai");
            entity.Property(e => e.Description)
                .HasColumnName("MoTa");
            entity.Property(e => e.Price)
                .HasColumnName("DonGia")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Image)
                .HasColumnName("Hinh")
                .HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnName("NgaySX");
            entity.Property(e => e.Discount)
                .HasColumnName("GiamGia")
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Views).HasColumnName("LuotMua");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HangHoa_Loai");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__HoaDon__MaHD");

            entity.ToTable("HoaDon");

            entity.Property(e => e.InvoiceId).HasColumnName("MaHD");
            entity.Property(e => e.CustomerId)
                .HasColumnName("MaKH")
                .HasMaxLength(20);
            entity.Property(e => e.OrderDate)
                .HasColumnName("NgayDat")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateNeeded).HasColumnName("NgayCan");
            entity.Property(e => e.DeliveryDate).HasColumnName("NgayGiao");
            entity.Property(e => e.CustomerName)
                .HasColumnName("HoTen")
                .HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasColumnName("DienThoai")
                .HasMaxLength(20);
            entity.Property(e => e.Address)
                .HasColumnName("DiaChi")
                .HasMaxLength(200);
            entity.Property(e => e.StatusCode).HasColumnName("MaTrangThai");
            entity.Property(e => e.PaymentMethod)
                .HasColumnName("CachThanhToan")
                .HasMaxLength(50);
            entity.Property(e => e.ShippingMethod)
                .HasColumnName("CachVanChuyen")
                .HasMaxLength(50);
            entity.Property(e => e.ShippingFee)
                .HasColumnName("PhiVanChuyen")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AdminId)
                .HasColumnName("MaNV")
                .HasMaxLength(50);
            entity.Property(e => e.Notes)
                .HasColumnName("GhiChu")
                .HasMaxLength(500);

            entity.HasOne(d => d.CustomerIdNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_KhachHang");

            entity.HasOne(d => d.AdminIdNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_HoaDon_NhanVien");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__KhachHang__MaKH");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.UserId, "UQ__KhachHang__UserId").IsUnique();

            entity.Property(e => e.CustomerId)
                .HasColumnName("MaKH")
                .HasMaxLength(20);
            entity.Property(e => e.UserId).HasColumnName("UserId");
            entity.Property(e => e.FullName)
                .HasColumnName("HoTen")
                .HasMaxLength(100);
            entity.Property(e => e.Sex).HasColumnName("GioiTinh");
            entity.Property(e => e.BirthDate).HasColumnName("NgaySinh");
            entity.Property(e => e.Address)
                .HasColumnName("DiaChi")
                .HasMaxLength(200);
            entity.Property(e => e.PhoneNumber)
                .HasColumnName("DienThoai")
                .HasMaxLength(20);
            entity.Property(e => e.Email)
                .HasColumnName("Email")
                .HasMaxLength(100);
            entity.Property(e => e.Image)
                .HasColumnName("Hinh")
                .HasMaxLength(255);

            entity.HasOne(d => d.User).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.UserId)
                .HasConstraintName("FK_KhachHang_NguoiDung");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Loai__MaLoai");

            entity.ToTable("Loai");

            entity.Property(e => e.CategoryId).HasColumnName("MaLoai");
            entity.Property(e => e.CategoryName)
                .HasColumnName("TenLoai")
                .HasMaxLength(50);
            entity.Property(e => e.CategoryAlias)
                .HasColumnName("TenLoaiAlias")
                .HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnName("MoTa");
            entity.Property(e => e.Image).HasColumnName("Hinh");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDung__Id");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.Username, "UQ__NguoiDung__TenDangNhap").IsUnique();

            entity.Property(e => e.Username)
                .HasColumnName("TenDangNhap")
                .HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasColumnName("MatKhau")
                .HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasColumnName("VaiTro");
            entity.Property(e => e.IsActive)
                .HasColumnName("HieuLuc")
                .HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasColumnName("NgayTao")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RandomKey)
                .HasColumnName("RandomKey")
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__NhanVien__MaNV");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.UserId, "UQ__NhanVien__UserId").IsUnique();

            entity.Property(e => e.AdminId)
                .HasColumnName("MaNV")
                .HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserId");
            entity.Property(e => e.FullName)
                .HasColumnName("HoTen")
                .HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasColumnName("Email")
                .HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasColumnName("DienThoai")
                .HasMaxLength(20);

            entity.HasOne(d => d.User).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.UserId)
                .HasConstraintName("FK_NhanVien_NguoiDung");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
