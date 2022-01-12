using Bardi.User;

namespace Bardi.Captures;

[Serializable]
public readonly record struct AccountCapture(IEnumerable<AssetCapture> AssetCaptures, int AccountIdx)
{
    public static AccountCapture FromAccount(Account account)
    {
        return new
        (
            account.Assets.Select(asset => new AssetCapture(asset)).ToList(), 
            account.Idx
        );
    }

    public static List<AccountCapture> FromAccounts(IEnumerable<Account> accounts)
    {
        return accounts.Select(FromAccount).ToList();
    }
}