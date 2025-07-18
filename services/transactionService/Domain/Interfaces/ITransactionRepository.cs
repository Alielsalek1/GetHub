using transactionService.Domain.Entities;
using transactionService.Domain.Enums;
using SharedKernel.ResultPattern;

namespace transactionService.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Result<Transaction>> GetByIdAsync(Guid transactionId);
    Task<Result<Transaction>> GetByReferenceNumberAsync(string referenceNumber);
    Task<Result<IEnumerable<Transaction>>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
    Task<Result<IEnumerable<Transaction>>> GetByUserAndTypeAsync(Guid userId, TransactionType type, int pageNumber = 1, int pageSize = 10);
    Task<Result<Transaction>> CreateAsync(Transaction transaction);
    Task<Result<Transaction>> UpdateAsync(Transaction transaction);
    Task<Result<bool>> ExistsAsync(Guid transactionId);
}
