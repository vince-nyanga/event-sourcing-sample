namespace EventSourcing;

public class BankAccountStore
{
    private readonly Dictionary<Guid, SortedList<uint, BankAccountEvent>> _eventStreams = new();
    private readonly Dictionary<Guid, SortedList<uint, BankAccountSnapshot>> _snapshots = new();
    
    public void Append(BankAccountEvent @event)
    {
        if (!_eventStreams.ContainsKey(@event.AccountId))
        {
            _eventStreams.Add(@event.AccountId, new());
        }

        _eventStreams[@event.AccountId].Add(@event.SequenceNumber, @event);
        
        // if every 5 events, take a snapshot
        if (_eventStreams[@event.AccountId].Count % 5 == 0)
        {
            TakeSnapshot(@event);
        }
    }
    
    public (BankAccount? BankAccount, uint HighestSequenceNumber) Load(Guid accountId)
    {
        if (!_eventStreams.TryGetValue(accountId, out var stream))
        {
            return (null, 0);
        }

        var snapshot = GetLatestSnapshot(accountId);
        var bankAccount = snapshot != null ? BankAccount.FromSnapshot(snapshot) : new BankAccount();
        
        var latestEvents = stream.Values
            .Where(x => x.SequenceNumber > (snapshot?.SequenceNumber ?? 0)).ToArray();
        
        // apply events to bank account
        bankAccount = latestEvents.Aggregate(bankAccount, (current, @event) => current.When(@event));
        
        var highestSequenceNumber = latestEvents.Length != 0 
            ? latestEvents.Max(x => x.SequenceNumber)
            : snapshot?.SequenceNumber ?? 0;
    
        return (bankAccount, highestSequenceNumber);
    }
    
    private BankAccountSnapshot? GetLatestSnapshot(Guid accountId)
    {
        return !_snapshots.TryGetValue(accountId, out var snapshots)
            ? null : snapshots.Last().Value;
    }
    
    private void TakeSnapshot(BankAccountEvent @event)
    {
        var (bankAccount, sequenceNumber) = Load(@event.AccountId);
        if (!_snapshots.TryGetValue(@event.AccountId, out var bankAccountSnapshots))
        {
            bankAccountSnapshots = new();
            _snapshots.Add(@event.AccountId, bankAccountSnapshots);
        }
            
        var snapshot = new BankAccountSnapshot(bankAccount!.Id, bankAccount.Balance, bankAccount.OwnerId, bankAccount.Status, sequenceNumber);
        bankAccountSnapshots.Add(@event.SequenceNumber, snapshot);
    }
}