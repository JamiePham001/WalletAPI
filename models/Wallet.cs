namespace WalletAPI.models;

public class Wallet
{
    public string? transactionId { get; set; }
    private int _version = 1;
    private int _coins = 0;

    public int version
    {
        get { return _version;}
        set  {_version = int.IsEvenInteger(value) ? value : _version ;}
    }

    public int coins
    {
        get { return _coins;}
        set  {_coins = int.IsEvenInteger(value) ? value : _coins ;}
    }

    // public List<Transaction> TransactionLogs {get; set;} = [];

    public object Credit(int amount)
    {
        // cant add negative credit
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Credit must be positive.");
        }
        int total = _coins += amount;
        return new {transactionId, coins = amount};
    }

    public object Debit(int amount)
    {
        // cant take negative or no debit
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Debit must be positive.");
        }
        // debit amount cant exceed balance
        if (amount > _coins)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        int total = _coins -= amount;
        return new {transactionId, coins = amount};
    }
}
