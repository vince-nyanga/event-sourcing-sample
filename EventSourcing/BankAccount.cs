namespace EventSourcing;

public record BankAccount
{
    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }
    public Guid OwnerId { get; private set; }
    public AccountStatus Status { get; private set; }
    
    public static BankAccount FromSnapshot(BankAccountSnapshot snapshot) =>
        new()
        {
            Id = snapshot.Id,
            Balance = snapshot.Balance,
            OwnerId = snapshot.OwnerId,
            Status = snapshot.Status
        };

    public BankAccount When(BankAccountEvent @event)
    {
        return @event switch
        {
            AccountOpened accountOpened => Apply(accountOpened),
            AccountApproved accountApproved => Apply(accountApproved),
            FundsDeposited moneyDeposited => Apply(moneyDeposited),
            FundsWithdrawn moneyWithdrawn => Apply(moneyWithdrawn),
            _ => this
        };
    }

    private BankAccount Apply(AccountOpened accountOpened) =>
        this with { Id = accountOpened.AccountId, OwnerId = accountOpened.OwnerId, Balance = 0, Status = AccountStatus.Pending };

    private BankAccount Apply(AccountApproved accountApproved) =>
        this with { Status = AccountStatus.Active };

    private BankAccount Apply(FundsDeposited fundsDeposited) =>
        this with { Balance = Balance + fundsDeposited.Amount };
    
    private BankAccount Apply(FundsWithdrawn fundsDeposited) =>
        this with { Balance = Balance - fundsDeposited.Amount };
}

public enum AccountStatus
{
    Pending,
    Active
}

public record BankAccountSnapshot(Guid Id, decimal Balance, Guid OwnerId, AccountStatus Status, uint SequenceNumber);