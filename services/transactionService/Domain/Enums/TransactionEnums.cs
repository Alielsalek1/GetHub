namespace transactionService.Domain.Enums;

public enum TransactionType
{
    Purchase = 1,
    Sale = 2,
    Refund = 3,
    Cancellation = 4
}

public enum TransactionStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    Refunded = 5
}

public enum PaymentMethod
{
    CreditCard = 1,
    DebitCard = 2,
    PayPal = 3,
    BankTransfer = 4,
    Cash = 5,
    Cryptocurrency = 6
}
