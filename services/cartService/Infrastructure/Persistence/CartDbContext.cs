using Microsoft.EntityFrameworkCore;
using cartService.Domain.Entities;

namespace cartService.Infrastructure.Persistence;

// Write-side DbContext for commands
public class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cart configuration
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            
            // Computed properties - ignore in database
            entity.Ignore(e => e.TotalAmount);
            entity.Ignore(e => e.TotalItems);
            
            // Index for performance
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.IsActive);
        });

        // CartItem configuration
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.AddedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // Computed property - ignore in database
            entity.Ignore(e => e.TotalPrice);

            entity.HasOne(e => e.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CartId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Indexes for performance
            entity.HasIndex(e => e.CartId);
            entity.HasIndex(e => e.ProductId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
