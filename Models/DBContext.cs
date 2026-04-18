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
            entity.HasKey(e => e.CartId).HasName("PK__Cart__20E715D500E97680");

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.CustomerId, e.ProductId }, "UQ_Cart_Customer_Product").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductId");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerId");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ProductIdNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_Product");

            entity.HasOne(d => d.CustomerIdNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Cart_Customer");
        });

        modelBuilder.Entity<DetailInvoice>(entity =>
        {
            entity.HasKey(e => e.DetailInvoiceId).HasName("PK__DetailInvoice__27258E749CB16809");

            entity.ToTable("DetailInvoice");

            entity.Property(e => e.DetailInvoiceId).HasColumnName("DetailInvoiceID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceId");
            entity.Property(e => e.ProductId).HasColumnName("ProductId");

            entity.HasOne(d => d.InvoiceIdNavigation).WithMany(p => p.DetailInvoices)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_DetailInvoice_Invoice");

            entity.HasOne(d => d.ProductIdNavigation).WithMany(p => p.DetailInvoices)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailInvoice_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__2725A6E4A8F62B3E");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.AliasName).HasMaxLength(100);
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("ProductName");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__2725A6E024B9D85B");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceId");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.ShippingMethod).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerId");
            entity.Property(e => e.AdminId)
                .HasMaxLength(50)
                .HasColumnName("AdminId");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ShippingFee).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CustomerIdNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Customer");

            entity.HasOne(d => d.AdminIdNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_Invoice_Admin");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__2725CF1E959A51CB");

            entity.ToTable("Customer");

            entity.HasIndex(e => e.UserId, "UQ__Customer__1788CC4D903C0EE7").IsUnique();

            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerId");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.UserId)
                .HasConstraintName("FK_Customer_User");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__2F633F2365B28AFA");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId);
            entity.Property(e => e.Image);
            entity.Property(e => e.Description);
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50);
            entity.Property(e => e.CategoryAlias)
                .HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07D5C16B17");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__55F68FC0A38F42FB").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RandomKey)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__2725D70A03F8EECD");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.UserId, "UQ__Admin__1788CC4DE2AFACA7").IsUnique();

            entity.Property(e => e.AdminId)
                .HasMaxLength(50)
                .HasColumnName("AdminId");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.UserId)
                .HasConstraintName("FK_Admin_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
