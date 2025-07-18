using Microsoft.EntityFrameworkCore;
using transactionService.Domain.Entities;
using transactionService.Domain.Interfaces;
using transactionService.Domain.Enums;
using transactionService.Infrastructure.Persistence;
using SharedKernel.ResultPattern;

namespace transactionService.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly TransactionDbContext _context;

    public TransactionRepository(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Transaction>> GetByIdAsync(Guid transactionId)
    {
        try
        {
            var transaction = await _context.Transactions
                .Include(t => t.Items)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return Result<Transaction>.Failure("Transaction not found");

            return Result<Transaction>.Success(transaction);
        }
        catch (Exception ex)
        {
            return Result<Transaction>.Failure($"Error retrieving transaction: {ex.Message}");
        }
    }

    public async Task<Result<Transaction>> GetByReferenceNumberAsync(string referenceNumber)
    {
        try
        {
            var transaction = await _context.Transactions
                .Include(t => t.Items)
                .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber);

            if (transaction == null)
                return Result<Transaction>.Failure("Transaction not found");

            return Result<Transaction>.Success(transaction);
        }
        catch (Exception ex)
        {
            return Result<Transaction>.Failure($"Error retrieving transaction by reference: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Transaction>>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var transactions = await _context.Transactions
                .Include(t => t.Items)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<IEnumerable<Transaction>>.Success(transactions);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Transaction>>.Failure($"Error retrieving user transactions: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Transaction>>> GetByUserAndTypeAsync(Guid userId, TransactionType type, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var transactions = await _context.Transactions
                .Include(t => t.Items)
                .Where(t => t.UserId == userId && t.Type == type)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<IEnumerable<Transaction>>.Success(transactions);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Transaction>>.Failure($"Error retrieving user transactions by type: {ex.Message}");
        }
    }

    public async Task<Result<Transaction>> CreateAsync(Transaction transaction)
    {
        try
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return Result<Transaction>.Success(transaction);
        }
        catch (Exception ex)
        {
            return Result<Transaction>.Failure($"Error creating transaction: {ex.Message}");
        }
    }

    public async Task<Result<Transaction>> UpdateAsync(Transaction transaction)
    {
        try
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return Result<Transaction>.Success(transaction);
        }
        catch (Exception ex)
        {
            return Result<Transaction>.Failure($"Error updating transaction: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid transactionId)
    {
        try
        {
            var exists = await _context.Transactions.AnyAsync(t => t.Id == transactionId);
            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking transaction existence: {ex.Message}");
        }
    }
}
