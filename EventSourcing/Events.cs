namespace EventSourcing;

public abstract record BankAccountEvent
{
    public required Guid AccountId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required uint SequenceNumber { get; init; }
}

public sealed record AccountOpened : BankAccountEvent
{
    public required Guid OwnerId { get; init; }
}

public sealed record AccountApproved : BankAccountEvent
{
    public required Guid ApprovedBy { get; init; }
}

public sealed record FundsDeposited : BankAccountEvent
{
    public required decimal Amount { get; init; }
}

public sealed record FundsWithdrawn : BankAccountEvent
{
    public required decimal Amount { get; init; }
}