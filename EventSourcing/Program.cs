using EventSourcing;

var bankAccountStore = new BankAccountStore();
var commandHandler = new CommandHandler(bankAccountStore);

var accountId = commandHandler.Handle(new OpenAccount(Guid.NewGuid()));
commandHandler.Handle(new ApproveAccount(accountId, Guid.NewGuid()));
commandHandler.Handle(new DepositFunds(accountId, 100m));
commandHandler.Handle(new WithdrawFunds(accountId, 50m));
commandHandler.Handle(new DepositFunds(accountId, 100m));
commandHandler.Handle(new DepositFunds(accountId, 50m));
commandHandler.Handle(new WithdrawFunds(accountId, 150));
commandHandler.Handle(new DepositFunds(accountId, 50));
commandHandler.Handle(new DepositFunds(accountId, 100m));
commandHandler.Handle(new DepositFunds(accountId, 100m));
commandHandler.Handle(new WithdrawFunds(accountId, 150m));


var (account, latestSequenceNumber) = bankAccountStore.Load(accountId);

Console.WriteLine(account);
Console.WriteLine($"Latest sequence number: {latestSequenceNumber}");
Console.ReadLine();