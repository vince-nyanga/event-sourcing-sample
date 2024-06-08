namespace EventSourcing;

public sealed record OpenAccount(Guid OwnerId);

public sealed record ApproveAccount(Guid AccountId, Guid ApprovedBy);

public sealed record DepositFunds(Guid AccountId, decimal Amount);

public sealed record WithdrawFunds(Guid AccountId, decimal Amount);