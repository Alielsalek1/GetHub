using Microsoft.EntityFrameworkCore;
using cartService.Application.Queries.Models;

namespace cartService.Infrastructure.Persistence;

// Read-side DbContext for queries (could be denormalized views or separate read models)
public class CartReadDbContext : DbContext
{
    public CartReadDbContext(DbContextOptions<CartReadDbContext> options) : base(options) { }

    public DbSet<CartDetailsDto> CartDetails { get; set; }
    public DbSet<CartSummaryDto> CartSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CartDetailsDto configuration (could be mapped to a view)
        modelBuilder.Entity<CartDetailsDto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            
            // This could map to a database view for optimized queries
            entity.ToTable("CartDetails");
            
            entity.OwnsMany(e => e.Items, item =>
            {
                item.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
                item.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                item.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            });
        });

        // CartSummaryDto configuration (could be mapped to a view)
        modelBuilder.Entity<CartSummaryDto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            
            // This could map to a database view for optimized queries
            entity.ToTable("CartSummaries");
        });

        base.OnModelCreating(modelBuilder);
    }
}
