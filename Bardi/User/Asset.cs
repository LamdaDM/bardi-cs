using Bardi.Captures;

namespace Bardi.User;

/// <summary>
/// <para>A record of an asset's value, which belongs to up to one account.
/// If not owned by an account, it is either unused or used by a mutator.</para>
/// 
/// <para>An asset's value will always remain the same unless changed by a mutation event,
/// meaning it cannot have any variance and can only be mutated internally.</para>
/// </summary>
public class Asset
{
    private bool Equals(Asset other)
    {
        return Idx == other.Idx && Equals(Owner, other.Owner) && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is decimal @decimal) return this == @decimal;
        if (obj.GetType() != GetType()) return false;
        
        return Equals((Asset)obj);
    }

    public override int GetHashCode()
    {
        return Idx.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Idx} : {Value}";
    }

    public readonly int Idx;
    public readonly Account? Owner;
    public decimal Value { get; internal set; }

    public static decimal operator +(Asset a, decimal b) => a.Value + b;
    public static decimal operator +(decimal a, Asset b) => a + b.Value;
    
    public static decimal operator *(Asset a, decimal b) => a.Value * b;
    public static decimal operator *(decimal a, Asset b) => a * b.Value;
    
    public static bool operator ==(Asset a, decimal b) => a.Value == b;
    public static bool operator !=(Asset a, decimal b) => a.Value != b;
    public static bool operator ==(decimal a, Asset b) => a == b.Value;
    public static bool operator !=(decimal a, Asset b) => a != b.Value;
    
    public static bool operator <(decimal a, Asset b) => a < b.Value;
    public static bool operator <(Asset a, decimal b) => a.Value < b;
    public static bool operator >(decimal a, Asset b) => a > b.Value;
    public static bool operator >(Asset a, decimal b) => a.Value > b;
    public static bool operator >=(Asset a, decimal b) => a.Value >= b;
    public static bool operator >=(decimal a, Asset b) => a >= b.Value;
    public static bool operator <=(Asset a, decimal b) => a.Value <= b;
    public static bool operator <=(decimal a, Asset b) => a <= b.Value;

    public Asset(decimal value, Account owner, int idx)
    {
        Value = value; 
        Owner = owner;
        Idx = idx;
    }

    public Asset(decimal value)
    {
        Value = value;
    }

    internal void ResetToCapture(AssetCapture capture)
    {
        Value = capture.Value;
    }
    
    public bool TryGetOwner(out Account? owner)
    {
        owner = Owner;
        return Owner != null;
    }

    public bool IsOwnedByAccount()
    {
        return Owner != null;
    }
}