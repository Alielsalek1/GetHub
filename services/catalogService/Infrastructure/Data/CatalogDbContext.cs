using CatalogService.Domain.models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Data;

/// <summary>
/// Database context for the Catalog Service containing product and category data
/// </summary>
/// <remarks>
/// Initializes a new instance of the CatalogDbContext
/// </remarks>
/// <param name="options">Database context options</param>
public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Categories table
    /// </summary>
    public DbSet<Category> Categories { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Products table
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Product Variants table
    /// </summary>
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;

    /// <summary>
    /// Configures the model and relationships for the database entities
    /// </summary>
    /// <param name="modelBuilder">Model builder instance</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Name)
                .IsUnique();

            // Self-referencing relationship for parent/child categories
            entity.HasOne(e => e.ParentCategory)
                .WithMany(e => e.Subcategories)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Brand)
                .HasMaxLength(100);

            entity.HasMany(e => e.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Name);

            entity.HasIndex(e => e.Brand);
        });

        // Configure ProductVariant entity
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SKU)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Price)
                .HasPrecision(18, 2);

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            // Configure Attributes as JSON column
            entity.Property(e => e.Attributes)
                .HasColumnType("jsonb");

            entity.HasIndex(e => e.SKU)
                .IsUnique();
        });
    }
}
