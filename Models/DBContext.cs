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

    public virtual DbSet<ChiTietHd> ChiTietHds { get; set; }

    public virtual DbSet<HangHoa> HangHoas { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=HKShop");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.MaCart).HasName("PK__Cart__20E715D500E97680");

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.MaKh, e.MaHh }, "UQ_Cart_KhachHang_HangHoa").IsUnique();

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaHh).HasColumnName("MaHH");
            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .HasColumnName("MaKH");
            entity.Property(e => e.NgayThem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaHhNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.MaHh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_HangHoa");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK_Cart_KhachHang");
        });

        modelBuilder.Entity<ChiTietHd>(entity =>
        {
            entity.HasKey(e => e.MaCt).HasName("PK__ChiTietH__27258E749CB16809");

            entity.ToTable("ChiTietHD");

            entity.Property(e => e.MaCt).HasColumnName("MaCT");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiamGia).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaHh).HasColumnName("MaHH");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHds)
                .HasForeignKey(d => d.MaHd)
                .HasConstraintName("FK_ChiTietHD_HoaDon");

            entity.HasOne(d => d.MaHhNavigation).WithMany(p => p.ChiTietHds)
                .HasForeignKey(d => d.MaHh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietHD_HangHoa");
        });

        modelBuilder.Entity<HangHoa>(entity =>
        {
            entity.HasKey(e => e.MaHh).HasName("PK__HangHoa__2725A6E4A8F62B3E");

            entity.ToTable("HangHoa");

            entity.Property(e => e.MaHh).HasColumnName("MaHH");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiamGia).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Hinh).HasMaxLength(255);
            entity.Property(e => e.MoTaDonVi).HasMaxLength(50);
            entity.Property(e => e.NgaySx).HasColumnName("NgaySX");
            entity.Property(e => e.TenAlias).HasMaxLength(100);
            entity.Property(e => e.TenHh)
                .HasMaxLength(100)
                .HasColumnName("TenHH");

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.HangHoas)
                .HasForeignKey(d => d.MaLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HangHoa_Loai");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E024B9D85B");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.CachThanhToan).HasMaxLength(50);
            entity.Property(e => e.CachVanChuyen).HasMaxLength(50);
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .HasColumnName("MaKH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(50)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_KhachHang");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK_HoaDon_NhanVien");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1E959A51CB");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.UserId, "UQ__KhachHan__1788CC4D903C0EE7").IsUnique();

            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Hinh).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.KhachHang)
                .HasForeignKey<KhachHang>(d => d.UserId)
                .HasConstraintName("FK_KhachHang_NguoiDung");
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.Maloai).HasName("PK__LOAI__2F633F2365B28AFA");

            entity.ToTable("LOAI");

            entity.Property(e => e.Maloai).HasColumnName("MALOAI");
            entity.Property(e => e.Hinh).HasColumnName("HINH");
            entity.Property(e => e.Mota).HasColumnName("MOTA");
            entity.Property(e => e.Tenloai)
                .HasMaxLength(50)
                .HasColumnName("TENLOAI");
            entity.Property(e => e.Tenloaialias)
                .HasMaxLength(50)
                .HasColumnName("TENLOAIALIAS");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC07D5C16B17");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenDangNhap, "UQ__NguoiDun__55F68FC0A38F42FB").IsUnique();

            entity.Property(e => e.HieuLuc).HasDefaultValue(true);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RandomKey)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70A03F8EECD");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.UserId, "UQ__NhanVien__1788CC4DE2AFACA7").IsUnique();

            entity.Property(e => e.MaNv)
                .HasMaxLength(50)
                .HasColumnName("MaNV");
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.NhanVien)
                .HasForeignKey<NhanVien>(d => d.UserId)
                .HasConstraintName("FK_NhanVien_NguoiDung");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__USERS__2725CF1E0AB39FDC");

            entity.ToTable("USERS");

            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
                .HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HieuLuc).HasDefaultValue(true);
            entity.Property(e => e.Hinh).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.RandomKey)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
