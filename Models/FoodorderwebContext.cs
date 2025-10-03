using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderWeb.Models;

public partial class FoodorderwebContext : DbContext
{
    public FoodorderwebContext()
    {
    }

    public FoodorderwebContext(DbContextOptions<FoodorderwebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<ComboItem> ComboItems { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<MenuItemSize> MenuItemSizes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\VUDOMINHTHANH;Database=foodorderweb;uid=sa;pwd=1234;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Addresse__091C2AFB472F1AEC");

            entity.HasIndex(e => e.UserId, "UX_Addresses_User_Default")
                .IsUnique()
                .HasFilter("([IsDefault]=(1))");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Label).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Street).HasMaxLength(300);

            entity.HasOne(d => d.User).WithOne(p => p.Address)
                .HasForeignKey<Address>(d => d.UserId)
                .HasConstraintName("FK_Addresses_Users");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BFA0C71AA");

            entity.HasIndex(e => e.Name, "UQ_Categories_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("PK__Combos__DD42582EFB1C6C24");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ComboItem>(entity =>
        {
            entity.HasKey(e => e.ComboItemId).HasName("PK__ComboIte__EE32F8058900BC7D");

            entity.HasOne(d => d.Combo).WithMany(p => p.ComboItems)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK_ComboItems_Combos");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.ComboItems)
                .HasForeignKey(d => d.MenuItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComboItems_MenuItems");

            entity.HasOne(d => d.MenuItemSize).WithMany(p => p.ComboItems)
                .HasForeignKey(d => d.MenuItemSizeId)
                .HasConstraintName("FK_ComboItems_MenuItemSizes");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuItemId).HasName("PK__MenuItem__8943F722456A034A");

            entity.HasIndex(e => new { e.Name, e.IsCombo }, "UQ_MenuItems_Name").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_MenuItems_Categories");
        });

        modelBuilder.Entity<MenuItemSize>(entity =>
        {
            entity.HasKey(e => e.MenuItemSizeId).HasName("PK__MenuItem__AFADF8BE460B5FB4");

            entity.HasIndex(e => e.MenuItemId, "IX_MenuItemSizes_MenuItem");

            entity.HasIndex(e => new { e.MenuItemId, e.SizeName }, "UQ_MenuItemSizes").IsUnique();

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SizeName).HasMaxLength(50);

            entity.HasOne(d => d.MenuItem).WithMany(p => p.MenuItemSizes)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("FK_MenuItemSizes_MenuItems");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF5E5335DF");

            entity.HasIndex(e => new { e.UserId, e.OrderDate }, "IX_Orders_User_OrderDate");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ShippingFee).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);

            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Orders_Address");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Status");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Orders_Payment");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06816B7BD6B8");

            entity.HasIndex(e => e.OrderId, "IX_OrderItems_OrderId");

            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Combo).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK_OrderItems_Combo");

            entity.HasOne(d => d.MenuItemSize).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.MenuItemSizeId)
                .HasConstraintName("FK_OrderItems_MenuItemSize");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderItems_Order");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PK__OrderSta__BC674CA1307EBDE8");

            entity.ToTable("OrderStatus");

            entity.HasIndex(e => e.Name, "UQ__OrderSta__737584F6D65861A7").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1D3103FAE30");

            entity.ToTable("PaymentMethod");

            entity.HasIndex(e => e.Name, "UQ__PaymentM__737584F6875202B4").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CFEC9B03E");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasDefaultValue("Customer");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Vouchers__3AEE792148D79EA9");

            entity.HasIndex(e => e.Code, "UQ__Vouchers__A25C5AA70CACE85F").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasMany(d => d.Combos).WithMany(p => p.Vouchers)
                .UsingEntity<Dictionary<string, object>>(
                    "VoucherCombo",
                    r => r.HasOne<Combo>().WithMany()
                        .HasForeignKey("ComboId")
                        .HasConstraintName("FK_VoucherCombos_Combo"),
                    l => l.HasOne<Voucher>().WithMany()
                        .HasForeignKey("VoucherId")
                        .HasConstraintName("FK_VoucherCombos_Voucher"),
                    j =>
                    {
                        j.HasKey("VoucherId", "ComboId");
                        j.ToTable("VoucherCombos");
                    });

            entity.HasMany(d => d.MenuItemSizes).WithMany(p => p.Vouchers)
                .UsingEntity<Dictionary<string, object>>(
                    "VoucherMenuItemSize",
                    r => r.HasOne<MenuItemSize>().WithMany()
                        .HasForeignKey("MenuItemSizeId")
                        .HasConstraintName("FK_VoucherMenuItemSizes_Size"),
                    l => l.HasOne<Voucher>().WithMany()
                        .HasForeignKey("VoucherId")
                        .HasConstraintName("FK_VoucherMenuItemSizes_Voucher"),
                    j =>
                    {
                        j.HasKey("VoucherId", "MenuItemSizeId");
                        j.ToTable("VoucherMenuItemSizes");
                    });

            entity.HasMany(d => d.MenuItems).WithMany(p => p.Vouchers)
                .UsingEntity<Dictionary<string, object>>(
                    "VoucherMenuItem",
                    r => r.HasOne<MenuItem>().WithMany()
                        .HasForeignKey("MenuItemId")
                        .HasConstraintName("FK_VoucherMenuItems_MenuItem"),
                    l => l.HasOne<Voucher>().WithMany()
                        .HasForeignKey("VoucherId")
                        .HasConstraintName("FK_VoucherMenuItems_Voucher"),
                    j =>
                    {
                        j.HasKey("VoucherId", "MenuItemId");
                        j.ToTable("VoucherMenuItems");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
