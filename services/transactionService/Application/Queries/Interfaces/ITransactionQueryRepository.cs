using transactionService.Application.Queries.Models;
using transactionService.Domain.Enums;
using SharedKernel.ResultPattern;

namespace transactionService.Application.Queries.Interfaces;

public interface ITransactionQueryRepository
{
    Task<Result<TransactionDetailsDto>> GetTransactionDetailsAsync(Guid transactionId);
    Task<Result<IEnumerable<TransactionSummaryDto>>> GetUserTransactionsAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
    Task<Result<IEnumerable<TransactionSummaryDto>>> GetUserTransactionsByTypeAsync(Guid userId, TransactionType type, int pageNumber = 1, int pageSize = 10);
    Task<Result<UserTransactionStatsDto>> GetUserTransactionStatsAsync(Guid userId);
    Task<Result<IEnumerable<TransactionSummaryDto>>> GetTransactionsByStatusAsync(TransactionStatus status, int pageNumber = 1, int pageSize = 10);
    Task<Result<IEnumerable<TransactionSummaryDto>>> GetRecentTransactionsAsync(int pageNumber = 1, int pageSize = 10);
}
