using Microsoft.EntityFrameworkCore;
using transactionService.Application.Queries.Models;

namespace transactionService.Infrastructure.Persistence;

// Read-side DbContext for queries (could be denormalized views or separate read models)
public class TransactionReadDbContext : DbContext
{
    public TransactionReadDbContext(DbContextOptions<TransactionReadDbContext> options) : base(options) { }

    public DbSet<TransactionDetailsDto> TransactionDetails { get; set; }
    public DbSet<TransactionSummaryDto> TransactionSummaries { get; set; }
    public DbSet<UserTransactionStatsDto> UserTransactionStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TransactionDetailsDto configuration (could be mapped to a view)
        modelBuilder.Entity<TransactionDetailsDto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalItemsValue).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            
            // This could map to a database view for optimized queries
            entity.ToTable("TransactionDetails");
            
            entity.OwnsMany(e => e.Items, item =>
            {
                item.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
                item.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                item.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            });
        });

        // TransactionSummaryDto configuration (could be mapped to a view)
        modelBuilder.Entity<TransactionSummaryDto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            
            // This could map to a database view for optimized queries
            entity.ToTable("TransactionSummaries");
        });

        // UserTransactionStatsDto configuration (could be mapped to a view)
        modelBuilder.Entity<UserTransactionStatsDto>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.TotalPurchases).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalSales).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AveragePurchaseAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AverageSaleAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NetBalance).HasColumnType("decimal(18,2)");
            
            // This could map to a database view for optimized queries
            entity.ToTable("UserTransactionStats");
        });

        base.OnModelCreating(modelBuilder);
    }
}
