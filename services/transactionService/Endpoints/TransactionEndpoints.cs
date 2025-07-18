using transactionService.Application.Commands.CreateTransaction;
using transactionService.Application.Commands.UpdateTransactionStatus;
using transactionService.Application.Queries.GetTransactionById;
using transactionService.Application.Queries.GetUserTransactions;
using transactionService.Application.Queries.GetUserStats;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SharedKernel.Extensions;

namespace transactionService.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/transactions")
            .WithTags("Transactions")
            .RequireAuthorization();

        // Commands
        group.MapPost("/", CreateTransaction)
            .WithName("CreateTransaction")
            .WithSummary("Create a new transaction")
            .WithDescription("Creates a new purchase or sale transaction");

        group.MapPut("/{transactionId}/status", UpdateTransactionStatus)
            .WithName("UpdateTransactionStatus")
            .WithSummary("Update transaction status")
            .WithDescription("Updates the status of an existing transaction");

        // Queries
        group.MapGet("/{transactionId}", GetTransactionById)
            .WithName("GetTransactionById")
            .WithSummary("Get transaction by ID")
            .WithDescription("Retrieves detailed transaction information by ID");

        group.MapGet("/user/{userId}", GetUserTransactions)
            .WithName("GetUserTransactions")
            .WithSummary("Get user transactions")
            .WithDescription("Retrieves all transactions for a specific user with optional filtering by type");

        group.MapGet("/user/{userId}/stats", GetUserTransactionStats)
            .WithName("GetUserTransactionStats")
            .WithSummary("Get user transaction statistics")
            .WithDescription("Retrieves comprehensive transaction statistics for a user");
    }

    private static async Task<IResult> CreateTransaction(
        CreateTransactionCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateTransactionStatus(
        Guid transactionId,
        UpdateTransactionStatusCommand command,
        IMediator mediator)
    {
        // Ensure the transaction ID in the route matches the command
        if (transactionId != command.TransactionId)
        {
            return Results.BadRequest("Transaction ID mismatch");
        }

        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetTransactionById(
        Guid transactionId,
        IMediator mediator)
    {
        var query = new GetTransactionByIdQuery { TransactionId = transactionId };
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetUserTransactions(
        Guid userId,
        IMediator mediator,
        int? type = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = new GetUserTransactionsQuery 
        { 
            UserId = userId,
            Type = type.HasValue ? (transactionService.Domain.Enums.TransactionType)type.Value : null,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetUserTransactionStats(
        Guid userId,
        IMediator mediator)
    {
        var query = new GetUserStatsQuery { UserId = userId };
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }
}
