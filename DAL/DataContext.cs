using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BeerShopPOS.Models;

namespace BeerShopPOS.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Receipt> Receipts { get; set; } = null!;
        public DbSet<ReceiptItem> ReceiptItems { get; set; } = null!;

        // This is only used as a fallback for design-time tools
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=beershop.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Barcode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.EGAISCode).HasMaxLength(50);
                entity.Property(e => e.EGAISVolume).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AmountPaid).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Created).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.FiscalNumber).HasMaxLength(50);
                entity.Property(e => e.EGAISCheckNumber).HasMaxLength(50);
                entity.Property(e => e.VoidReason).HasMaxLength(500);
                entity.Property(e => e.IsPrinted).IsRequired();
                entity.Property(e => e.CashierName).HasMaxLength(100);

                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Receipt)
                    .HasForeignKey(e => e.ReceiptId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ReceiptItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.VoidedQuantity);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .IsRequired();
            });
        }
    }
}