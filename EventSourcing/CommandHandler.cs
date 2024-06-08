namespace EventSourcing;

public class CommandHandler(BankAccountStore store)
{
    public Guid Handle(OpenAccount command)
    {
        var accountOpened = new AccountOpened
        {
            AccountId = Guid.NewGuid(),
            OwnerId = command.OwnerId,
            Timestamp = DateTimeOffset.UtcNow,
            SequenceNumber = 1
        };
        
        store.Append(accountOpened);
        return accountOpened.AccountId;
    }
    
    public void Handle(ApproveAccount command)
    {
        var (account, sequenceNumber) = GetAccount(command.AccountId);
        if (account.Status != AccountStatus.Pending)
        {
            throw new Exception("Account not pending approval");
        }
        
        var accountApproved = new AccountApproved
        {
            AccountId = command.AccountId,
            ApprovedBy = command.ApprovedBy,
            Timestamp = DateTimeOffset.UtcNow,
            SequenceNumber = sequenceNumber + 1
        };
        
        store.Append(accountApproved);
    }
    
    public void Handle(DepositFunds command)
    {
        var (account, sequenceNumber) = GetAccount(command.AccountId);

        EnsureAccountIsActive(account);
        
        var fundsDeposited = new FundsDeposited
        {
            AccountId = command.AccountId,
            Amount = command.Amount,
            Timestamp = DateTimeOffset.UtcNow,
            SequenceNumber = sequenceNumber + 1
        };
        
        store.Append(fundsDeposited);
    }
    
    public void Handle(WithdrawFunds command)
    {
        var (account, sequenceNumber) = GetAccount(command.AccountId);

        EnsureAccountIsActive(account);

        if (account.Balance < command.Amount)
        {
            throw new Exception("Insufficient funds");
        }
        
        var fundsWithdrawn = new FundsWithdrawn
        {
            AccountId = command.AccountId,
            Amount = command.Amount,
            Timestamp = DateTimeOffset.UtcNow,
            SequenceNumber = sequenceNumber + 1
        };
        
        store.Append(fundsWithdrawn);
    }

    private static void EnsureAccountIsActive(BankAccount account)
    {
        if (account.Status != AccountStatus.Active)
        {
            throw new Exception("Account not active");
        }
    }

    private (BankAccount BankAccount, uint HighestSequenceNumber) GetAccount(Guid accountId)
    {
        var (account, sequenceNumber) = store.Load(accountId);
        if (account is null)
        {
            throw new Exception("Account not found");
        }

        return (account, sequenceNumber);
    }
}