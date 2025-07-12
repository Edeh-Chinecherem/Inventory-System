using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using InventorySystem.Models;


namespace InventorySystem.Data
{
    public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relationships
        modelBuilder.Entity<Sale>()
            .HasMany(s => s.SaleItems)
            .WithOne(si => si.Sale)
            .HasForeignKey(si => si.SaleId);

        modelBuilder.Entity<SaleItem>()
            .HasOne(si => si.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(si => si.ProductId);

        // Indexes
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.ProductName)
            .HasDatabaseName("Index_ProductName");

        modelBuilder.Entity<Sale>()
            .HasIndex(s => s.Timestamp)
            .HasDatabaseName("Index_SaleTimestamp");

        // Seed Data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, ProductName = "Pampers Diapers", Price = 19.99M, Quantity = 100 },
            new Product { Id = 2, ProductName = "Pears Lotion", Price = 49.99M, Quantity = 50 }
        );

        // Entity configurations
        modelBuilder.Entity<Product>()
            .Property(p => p.ProductName)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.IsDeleted);


        modelBuilder.Entity<SaleItem>()
            .Property(si => si.Quantity)
            .HasDefaultValue(1);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Product && e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var entity = entry.Entity as Product;
            if (entity != null)
            {
                entity.IsDeleted = true;
                entry.State = EntityState.Modified;
            }
        }

        return base.SaveChanges();
    }
}

}
