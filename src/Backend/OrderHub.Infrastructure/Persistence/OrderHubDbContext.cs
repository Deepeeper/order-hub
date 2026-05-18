using Microsoft.EntityFrameworkCore;
using OrderHub.Domain.Customers;
using OrderHub.Domain.Orders;
using OrderHub.Domain.Products;

namespace OrderHub.Infrastructure.Persistence;

public class OrderHubDbContext : DbContext
{
    public OrderHubDbContext(DbContextOptions<OrderHubDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.HasKey(o => o.Id);
            e.Property(o => o.OrderNumber).HasMaxLength(32).IsRequired();
            e.Property(o => o.CustomerName).HasMaxLength(128);
            e.Property(o => o.Notes).HasMaxLength(500);
            e.HasMany(o => o.Lines)
                .WithOne()
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<OrderLine>(e =>
        {
            e.ToTable("OrderLines");
            e.HasKey(l => l.Id);
            e.Property(l => l.Quantity).HasPrecision(18, 3);
            e.Property(l => l.UnitPrice).HasPrecision(18, 2);
            e.Property(l => l.ProductName).HasMaxLength(128);
            e.Property(l => l.SteelGrade).HasMaxLength(32);
        });

        b.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(128).IsRequired();
            e.Property(c => c.Country).HasMaxLength(64);
            e.Property(c => c.ContactEmail).HasMaxLength(128);
        });

        b.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(128).IsRequired();
            e.Property(p => p.SteelGrade).HasMaxLength(32);
            e.Property(p => p.DimensionMm).HasMaxLength(64);
            e.Property(p => p.PricePerKg).HasPrecision(18, 2);
            e.Property(p => p.LeadTimeDays).HasPrecision(8, 2);
        });
    }
}
