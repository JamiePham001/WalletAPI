using System.Transactions;
namespace WalletAPI.models;

public class Wallet
{
    private string _walletId;
    public string _transactionId = "";
    private int _version = 0;
    private int _coins = 0;

    public string walletId
    {
        get => _walletId;
        init => _walletId = value;
    }

    public string transactionId
    {
        get => _transactionId;
        set => _transactionId = value;
    }

    public int version
    {
        get => _version;
        set => _version = value;
    }

    public int coins
    {
        get => _coins;
        set => _coins = int.IsEvenInteger(value) ? value : throw new ArgumentOutOfRangeException(nameof(value));
    }

    public Wallet(string walletId)
    {
        _walletId = walletId;
    }


    public Wallet? Credit(int credit, string transactionId)
    {
        // cant add negative credit
        if (credit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(credit), $"Credit must be positive. {credit}");
        }

        // check if transactinoId's of request and wallet are identical
        if (transactionId == _transactionId)
        {
            return null;
        }

        _coins += credit;
        _transactionId = transactionId;
        _version++;
        return this;
    }

    public Wallet? Debit(int debit, string transactionId)
    {

        // debit amount cant exceed balance nor can be a negative number
        if (debit > _coins || debit <= 0)
        {
            throw new ArgumentException("Debit must be a positive number or there are insufficient funds.");
        }

        // check if transactinoId's of request and wallet are identical
        if (transactionId == _transactionId)
        {
            return null;
        }

        _coins -= debit;
        _transactionId = transactionId;
        _version++;
        return this;
    }
}

