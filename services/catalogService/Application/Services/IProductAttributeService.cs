using FluentResults;
using catalogService.Application.DTOs;

namespace catalogService.Application.Services;

public interface IProductAttributeService
{
    Task<Result<AttributeResponse>> CreateAttributeAsync(CreateAttributeRequest request);
    Task<Result<AttributeResponse>> GetAttributeByIdAsync(int id);
    Task<Result<IEnumerable<AttributeResponse>>> GetAllAttributesAsync();
    Task<Result> DeleteAttributeAsync(int id);
}
