using Microsoft.EntityFrameworkCore;
using transactionService.Domain.Entities;
using transactionService.Domain.Enums;

namespace transactionService.Infrastructure.Persistence;

// Write-side DbContext for commands
public class TransactionDbContext : DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionItem> TransactionItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Type).IsRequired().HasConversion<int>();
            entity.Property(e => e.Status).IsRequired().HasConversion<int>();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Currency).HasMaxLength(10).IsRequired();
            entity.Property(e => e.PaymentMethod).IsRequired().HasConversion<int>();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.CompletedAt);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            
            // Computed properties - ignore in database
            entity.Ignore(e => e.TotalItemsValue);
            entity.Ignore(e => e.TotalItemsCount);
            
            // Indexes for performance
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.ReferenceNumber).IsUnique();
        });

        // TransactionItem configuration
        modelBuilder.Entity<TransactionItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // Computed property - ignore in database
            entity.Ignore(e => e.TotalPrice);

            entity.HasOne(e => e.Transaction)
                .WithMany(t => t.Items)
                .HasForeignKey(e => e.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Indexes for performance
            entity.HasIndex(e => e.TransactionId);
            entity.HasIndex(e => e.ProductId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
