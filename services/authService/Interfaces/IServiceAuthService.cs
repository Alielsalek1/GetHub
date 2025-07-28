using FluentResults;
using authService.Dtos;

namespace authService.Interfaces;

public interface IServiceAuthService
{
    Task<Result<string>> IssueServiceTokenAsync();
}
