using FluentResults;
using catalogService.Application.DTOs;
using catalogService.Domain.Entities;
using catalogService.Domain.Interfaces;
using SharedKernel.ResultPattern;

namespace catalogService.Application.Services;

public class ProductAttributeService : IProductAttributeService
{
    private readonly IProductAttributeRepository _attributeRepository;

    public ProductAttributeService(IProductAttributeRepository attributeRepository)
    {
        _attributeRepository = attributeRepository;
    }

    public async Task<Result<AttributeResponse>> CreateAttributeAsync(CreateAttributeRequest request)
    {
        var existingAttribute = await _attributeRepository.GetByNameAsync(request.Name);
        if (existingAttribute != null)
            return Result.Fail(new ConflictError($"Attribute with name '{request.Name}' already exists"));

        var attribute = new ProductAttribute
        {
            Name = request.Name,
            DataType = request.DataType,
            IsRequired = request.IsRequired
        };

        var createdAttribute = await _attributeRepository.CreateAsync(attribute);
        return Result.Ok(MapToResponse(createdAttribute));
    }

    public async Task<Result<AttributeResponse>> GetAttributeByIdAsync(int id)
    {
        var attribute = await _attributeRepository.GetByIdAsync(id);
        if (attribute == null)
            return Result.Fail(new NotFoundError($"Attribute with ID {id} not found"));

        return Result.Ok(MapToResponse(attribute));
    }

    public async Task<Result<IEnumerable<AttributeResponse>>> GetAllAttributesAsync()
    {
        var attributes = await _attributeRepository.GetAllAsync();
        return Result.Ok(attributes.Select(MapToResponse));
    }

    public async Task<Result> DeleteAttributeAsync(int id)
    {
        var attribute = await _attributeRepository.GetByIdAsync(id);
        if (attribute == null)
            return Result.Fail(new NotFoundError($"Attribute with ID {id} not found"));

        await _attributeRepository.DeleteAsync(id);
        return Result.Ok();
    }

    private static AttributeResponse MapToResponse(ProductAttribute attribute)
    {
        return new AttributeResponse
        {
            Id = attribute.Id,
            Name = attribute.Name,
            DataType = attribute.DataType,
            IsRequired = attribute.IsRequired
        };
    }
}
