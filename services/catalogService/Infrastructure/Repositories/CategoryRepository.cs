using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;

namespace CatalogService.Infrastructure.Repositories;

public class CategoryRepository(CatalogDbContext context) : ICategoryRepository
{
    public async Task AddAsync(Category category)
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await context.Categories.FindAsync(id);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await context.Categories.FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task UpdateAsync(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
    }

    public async Task<List<Category>> GetBaseCategoriesAsync()
    {
        return await context.Categories
            .Where(c => c.ParentId == null)
            .ToListAsync();
    }

    public async Task<List<Category>> GetFirstLevelCategoriesAsync(int id)
    {
        return await context.Categories
            .Where(c => c.ParentId == id)
            .ToListAsync();
    }

    public async Task<List<Category>> GetCategoryWithAncestorsAsync(int id)
    {
        var sql = @"
            WITH RECURSIVE CategoryHierarchy AS (
                -- Base case: Start with the specified category
                SELECT ""Id"", ""Name"", ""ParentId"", 0 as Level
                FROM ""Categories"" 
                WHERE ""Id"" = @CategoryId
                
                UNION ALL
                
                -- Recursive case: Get parent categories
                SELECT c.""Id"", c.""Name"", c.""ParentId"", ch.Level + 1
                FROM ""Categories"" c
                INNER JOIN CategoryHierarchy ch ON c.""Id"" = ch.""ParentId""
            )
            SELECT ""Id"", ""Name"", ""ParentId"" FROM CategoryHierarchy
            ORDER BY Level";

        using var connection = context.Database.GetDbConnection();
        var categories = await connection.QueryAsync<Category>(sql, new { CategoryId = id });
        return [.. categories];
    }
}