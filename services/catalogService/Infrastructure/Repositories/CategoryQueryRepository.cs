using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using System.Linq;

namespace CatalogService.Infrastructure.Repositories;

public class CategoryQueryRepository(CatalogDbContext context) : ICategoryQueryRepository
{
    public async Task<Category?> GetByIdAsync(int id)
    {
        return await context.Categories.FindAsync(id);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await context.Categories.FirstOrDefaultAsync(u => u.Name == name);
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
                SELECT ""Id"", ""Name"", ""ParentId"", 0 as Level
                FROM ""Categories"" 
                WHERE ""Id"" = @CategoryId
                
                UNION ALL
                
                SELECT c.""Id"", c.""Name"", c.""ParentId"", ch.Level + 1
                FROM ""Categories"" c
                INNER JOIN CategoryHierarchy ch ON c.""Id"" = ch.""ParentId""
            )
            SELECT ""Id"", ""Name"", ""ParentId"" FROM CategoryHierarchy
            ORDER BY Level";

        using var connection = context.Database.GetDbConnection();
        var categories = await connection.QueryAsync<Category>(sql, new { CategoryId = id });
        return categories.ToList();
    }

    public async Task<bool> HasChildCategoriesAsync(int id)
    {
        return await context.Categories.AnyAsync(c => c.ParentId == id);
    }
}
