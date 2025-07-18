using catalogService.Application.DTOs;
using catalogService.Application.Services;
using SharedKernel.Extensions;

namespace catalogService.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var catalogGroup = app.MapGroup("/catalog")
            .WithTags("Catalog");

        // Product endpoints
        catalogGroup.MapPost("/products", async (CreateProductRequest request, IProductService productService) =>
        {
            var result = await productService.CreateProductAsync(request);
            return result.ToApiResult(201, "Product created successfully");
        }).RequireAuthorization("Internal");

        catalogGroup.MapGet("/products/{id:guid}", async (Guid id, IProductService productService) =>
        {
            var result = await productService.GetProductByIdAsync(id);
            return result.ToApiResult(200, "Product retrieved successfully");
        });

        catalogGroup.MapGet("/products", async (IProductService productService) =>
        {
            var result = await productService.GetAllProductsAsync();
            return result.ToApiResult(200, "Products retrieved successfully");
        });

        catalogGroup.MapPut("/products/{id:guid}", async (Guid id, UpdateProductRequest request, IProductService productService) =>
        {
            var result = await productService.UpdateProductAsync(id, request);
            return result.ToApiResult(200, "Product updated successfully");
        }).RequireAuthorization("Internal");

        catalogGroup.MapDelete("/products/{id:guid}", async (Guid id, IProductService productService) =>
        {
            var result = await productService.DeleteProductAsync(id);
            return result.ToApiResult(204, "Product deleted successfully");
        }).RequireAuthorization("Internal");

        catalogGroup.MapGet("/products/search", async (string attributeName, string value, IProductService productService) =>
        {
            var result = await productService.SearchProductsByAttributeAsync(attributeName, value);
            return result.ToApiResult(200, "Products found");
        });

        // Attribute endpoints
        catalogGroup.MapPost("/attributes", async (CreateAttributeRequest request, IProductAttributeService attributeService) =>
        {
            var result = await attributeService.CreateAttributeAsync(request);
            return result.ToApiResult(201, "Attribute created successfully");
        }).RequireAuthorization("Internal");

        catalogGroup.MapGet("/attributes/{id:int}", async (int id, IProductAttributeService attributeService) =>
        {
            var result = await attributeService.GetAttributeByIdAsync(id);
            return result.ToApiResult(200, "Attribute retrieved successfully");
        });

        catalogGroup.MapGet("/attributes", async (IProductAttributeService attributeService) =>
        {
            var result = await attributeService.GetAllAttributesAsync();
            return result.ToApiResult(200, "Attributes retrieved successfully");
        });

        catalogGroup.MapDelete("/attributes/{id:int}", async (int id, IProductAttributeService attributeService) =>
        {
            var result = await attributeService.DeleteAttributeAsync(id);
            return result.ToApiResult(204, "Attribute deleted successfully");
        }).RequireAuthorization("Internal");
    }
}
