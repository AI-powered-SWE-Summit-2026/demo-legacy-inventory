using LegacyInventory.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyInventory.Core
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<StockLevel> StockLevels { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }

        public DbSet<Shipment> Shipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasOne(x => x.ParentCategory)
                      .WithMany(x => x.ChildCategories)
                      .HasForeignKey(x => x.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                entity.HasIndex(x => x.SKU).IsUnique();
                entity.HasOne(x => x.Category)
                      .WithMany(x => x.Products)
                      .HasForeignKey(x => x.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Supplier)
                      .WithMany(x => x.Products)
                      .HasForeignKey(x => x.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.ToTable("Warehouses");
            });

            modelBuilder.Entity<StockLevel>(entity =>
            {
                entity.ToTable("StockLevels");
                entity.HasOne(x => x.Product)
                      .WithMany(x => x.StockLevels)
                      .HasForeignKey(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Warehouse)
                      .WithMany(x => x.StockLevels)
                      .HasForeignKey(x => x.WarehouseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.ToTable("PurchaseOrders");
                entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
                entity.HasIndex(x => x.OrderNumber).IsUnique();
                entity.HasOne(x => x.Supplier)
                      .WithMany(x => x.PurchaseOrders)
                      .HasForeignKey(x => x.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseOrderLine>(entity =>
            {
                entity.ToTable("PurchaseOrderLines");
                entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
                entity.HasOne(x => x.PurchaseOrder)
                      .WithMany(x => x.Lines)
                      .HasForeignKey(x => x.PurchaseOrderId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Product)
                      .WithMany(x => x.PurchaseOrderLines)
                      .HasForeignKey(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.ToTable("Shipments");
                entity.HasOne(x => x.PurchaseOrder)
                      .WithMany(x => x.Shipments)
                      .HasForeignKey(x => x.PurchaseOrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
