using Microsoft.EntityFrameworkCore;
using orderService.Application.Queries.Models;

namespace orderService.Infrastructure.Persistence;

// Read-side DbContext for queries (could be denormalized views or separate read models)
public class OrderReadDbContext : DbContext
{
    public OrderReadDbContext(DbContextOptions<OrderReadDbContext> options) : base(options) { }

    public DbSet<OrderSummary> OrderSummaries { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // OrderSummary configuration (could be mapped to a view)
        modelBuilder.Entity<OrderSummary>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            
            // This could map to a database view for optimized queries
            entity.ToTable("OrderSummaries");
        });

        // OrderDetails configuration (could be mapped to a view with joined data)
        modelBuilder.Entity<OrderDetails>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            entity.Property(e => e.BillingAddress).HasMaxLength(500);
            
            // This could map to a database view for optimized queries
            entity.ToTable("OrderDetails");
            
            entity.OwnsMany(e => e.Items, item =>
            {
                item.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
                item.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                item.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}
