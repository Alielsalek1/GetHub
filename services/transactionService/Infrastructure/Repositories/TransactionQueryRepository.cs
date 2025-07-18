using Microsoft.EntityFrameworkCore;
using transactionService.Application.Queries.Interfaces;
using transactionService.Application.Queries.Models;
using transactionService.Infrastructure.Persistence;
using transactionService.Domain.Enums;
using SharedKernel.ResultPattern;

namespace transactionService.Infrastructure.Repositories;

public class TransactionQueryRepository : ITransactionQueryRepository
{
    private readonly TransactionReadDbContext _context;
    private readonly TransactionDbContext _writeContext; // For direct queries when read model isn't available

    public TransactionQueryRepository(TransactionReadDbContext context, TransactionDbContext writeContext)
    {
        _context = context;
        _writeContext = writeContext;
    }

    public async Task<Result<TransactionDetailsDto>> GetTransactionDetailsAsync(Guid transactionId)
    {
        try
        {
            // First try to get from read model
            var transactionDetails = await _context.TransactionDetails
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transactionDetails != null)
                return Result<TransactionDetailsDto>.Success(transactionDetails);

            // Fallback to write model if read model not available
            var transaction = await _writeContext.Transactions
                .Include(t => t.Items)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return Result<TransactionDetailsDto>.Failure("Transaction not found");

            var transactionDetailsDto = new TransactionDetailsDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                Type = transaction.Type,
                Status = transaction.Status,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                PaymentMethod = transaction.PaymentMethod,
                CreatedAt = transaction.CreatedAt,
                UpdatedAt = transaction.UpdatedAt,
                CompletedAt = transaction.CompletedAt,
                Description = transaction.Description,
                ReferenceNumber = transaction.ReferenceNumber ?? string.Empty,
                TotalItemsValue = transaction.TotalItemsValue,
                TotalItemsCount = transaction.TotalItemsCount,
                Items = transaction.Items.Select(item => new TransactionItemDetailsDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    CreatedAt = item.CreatedAt
                }).ToList()
            };

            return Result<TransactionDetailsDto>.Success(transactionDetailsDto);
        }
        catch (Exception ex)
        {
            return Result<TransactionDetailsDto>.Failure($"Error retrieving transaction details: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TransactionSummaryDto>>> GetUserTransactionsAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            // First try to get from read model
            var summaries = await _context.TransactionSummaries
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (summaries.Any())
                return Result<IEnumerable<TransactionSummaryDto>>.Success(summaries);

            // Fallback to write model
            var transactions = await _writeContext.Transactions
                .Include(t => t.Items)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var transactionSummaries = transactions.Select(t => new TransactionSummaryDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Type = t.Type,
                Status = t.Status,
                Amount = t.Amount,
                Currency = t.Currency,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                ReferenceNumber = t.ReferenceNumber ?? string.Empty,
                TotalItemsCount = t.TotalItemsCount
            });

            return Result<IEnumerable<TransactionSummaryDto>>.Success(transactionSummaries);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TransactionSummaryDto>>.Failure($"Error retrieving user transactions: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TransactionSummaryDto>>> GetUserTransactionsByTypeAsync(Guid userId, TransactionType type, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var summaries = await _context.TransactionSummaries
                .Where(t => t.UserId == userId && t.Type == type)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (summaries.Any())
                return Result<IEnumerable<TransactionSummaryDto>>.Success(summaries);

            // Fallback to write model
            var transactions = await _writeContext.Transactions
                .Include(t => t.Items)
                .Where(t => t.UserId == userId && t.Type == type)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var transactionSummaries = transactions.Select(t => new TransactionSummaryDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Type = t.Type,
                Status = t.Status,
                Amount = t.Amount,
                Currency = t.Currency,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                ReferenceNumber = t.ReferenceNumber ?? string.Empty,
                TotalItemsCount = t.TotalItemsCount
            });

            return Result<IEnumerable<TransactionSummaryDto>>.Success(transactionSummaries);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TransactionSummaryDto>>.Failure($"Error retrieving user transactions by type: {ex.Message}");
        }
    }

    public async Task<Result<UserTransactionStatsDto>> GetUserTransactionStatsAsync(Guid userId)
    {
        try
        {
            // First try to get from read model
            var stats = await _context.UserTransactionStats
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (stats != null)
                return Result<UserTransactionStatsDto>.Success(stats);

            // Calculate from write model
            var transactions = await _writeContext.Transactions
                .Where(t => t.UserId == userId && t.Status == TransactionStatus.Completed)
                .ToListAsync();

            var purchases = transactions.Where(t => t.Type == TransactionType.Purchase).ToList();
            var sales = transactions.Where(t => t.Type == TransactionType.Sale).ToList();

            var userStats = new UserTransactionStatsDto
            {
                UserId = userId,
                TotalPurchases = purchases.Sum(p => p.Amount),
                TotalSales = sales.Sum(s => s.Amount),
                TotalPurchaseCount = purchases.Count,
                TotalSaleCount = sales.Count,
                AveragePurchaseAmount = purchases.Any() ? purchases.Average(p => p.Amount) : 0,
                AverageSaleAmount = sales.Any() ? sales.Average(s => s.Amount) : 0,
                LastPurchaseDate = purchases.OrderByDescending(p => p.CompletedAt).FirstOrDefault()?.CompletedAt,
                LastSaleDate = sales.OrderByDescending(s => s.CompletedAt).FirstOrDefault()?.CompletedAt,
                NetBalance = sales.Sum(s => s.Amount) - purchases.Sum(p => p.Amount)
            };

            return Result<UserTransactionStatsDto>.Success(userStats);
        }
        catch (Exception ex)
        {
            return Result<UserTransactionStatsDto>.Failure($"Error retrieving user transaction stats: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TransactionSummaryDto>>> GetTransactionsByStatusAsync(TransactionStatus status, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var summaries = await _context.TransactionSummaries
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<IEnumerable<TransactionSummaryDto>>.Success(summaries);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TransactionSummaryDto>>.Failure($"Error retrieving transactions by status: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TransactionSummaryDto>>> GetRecentTransactionsAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var summaries = await _context.TransactionSummaries
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<IEnumerable<TransactionSummaryDto>>.Success(summaries);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TransactionSummaryDto>>.Failure($"Error retrieving recent transactions: {ex.Message}");
        }
    }
}
