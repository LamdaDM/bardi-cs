using Bardi.User.Mutators.Base;
using Bardi.User.Mutators.Variants;

namespace Bardi.User.Mutators;

public static class MutatorFactory
{
    /// <summary>
    /// Creates a Standard Mutator.
    /// </summary>
    public static Mutator Create(int idx, Asset target, decimal change, int cycle,
        long referenceDateDays, bool isAmountAdd = true, decimal totalChange = 0)
    {
        var mutatorBase = new MutatorBase(idx, target, change, isAmountAdd, cycle, totalChange, referenceDateDays);
        return new Mutator(new StandardVariant(mutatorBase), mutatorBase);
    }
    
    /// <summary>
    /// Creates a Standard Mutator.
    /// </summary>
    public static Mutator Create(int idx, Asset target, decimal change, int cycle,
        DateTimeOffset referenceDate, bool isAmountAdd = true, decimal totalChange = 0)
    {
        var mutatorBase = new MutatorBase(idx, target, change, isAmountAdd, cycle, totalChange, referenceDate);
        return new Mutator(new StandardVariant(mutatorBase), mutatorBase);
    }
    
    /// <summary>
    /// Creates a ExpireFirst Mutator.
    /// </summary>
    public static Mutator Create(int idx, Asset target, decimal change, int cycle,
        long referenceDateDays, long expiryDays, bool isAmountAdd = true, decimal totalChange = 0)
    {
        var mutatorBase = new MutatorBase(idx, target, change, isAmountAdd, cycle, totalChange, referenceDateDays);
        return new Mutator(new ExpireFirstVariant(mutatorBase, expiryDays), mutatorBase);
    }
    
    /// <summary>
    /// Creates a ExpireFirst Mutator.
    /// </summary>
    public static Mutator Create(int idx, Asset target, decimal change, int cycle,
        DateTimeOffset referenceDate, DateTimeOffset expiry, bool isAmountAdd = true, decimal totalChange = 0)
    {
        var mutatorBase = new MutatorBase(idx, target, change, isAmountAdd, cycle, totalChange, referenceDate);
        return new Mutator(new ExpireFirstVariant(mutatorBase, expiry), mutatorBase);
    }

    /// <summary>
    /// Creates a Debt Mutator.
    /// </summary>
    public static Mutator Create(int idx, Asset target, decimal change, int cycle,
        long referenceDateDays, Asset debt, bool isAmountAdd = true, decimal totalChange = 0)
    {
        var mutatorBase = new MutatorBase(idx, target, change, isAmountAdd, cycle, totalChange, referenceDateDays);
        return new Mutator(new DebtVariant(mutatorBase, debt), mutatorBase);
    }
    
    /// <summary>
    /// Creates a Debt Mutator.
    /// </summary>
    public static Mutator Create(int idx, Asset target, decimal change, int cycle,
        DateTimeOffset referenceDate, Asset debt, bool isAmountAdd = true, decimal totalChange = 0)
    {
        var mutatorBase = new MutatorBase(idx, target, change, isAmountAdd, cycle, totalChange, referenceDate);
        return new Mutator(new DebtVariant(mutatorBase, debt), mutatorBase);
    }
    
    /// <summary>
    /// Creates a ExpireFirst-Debt Mutator.
    /// </summary>
    public static Mutator Create(int idx, Asset target, decimal change, int cycle,
        long referenceDateDays, Asset debt, long expiryDays, bool isAmountAdd = true, decimal totalChange = 0)
    {
        var mutatorBase = new MutatorBase(idx, target, change, isAmountAdd, cycle, totalChange, referenceDateDays);
        return new Mutator(new ExpireFirstDebtVariant(mutatorBase, debt, expiryDays), mutatorBase);
    }
    
    /// <summary>
    /// Creates a ExpireFirst-Debt Mutator.
    /// </summary>
    public static Mutator Create(int idx, Asset target, decimal change, int cycle, DateTimeOffset referenceDate, 
        Asset debt, DateTimeOffset expiry, bool isAmountAdd = true, decimal totalChange = 0)
    {
        var mutatorBase = new MutatorBase(idx, target, change, isAmountAdd, cycle, totalChange, referenceDate);
        return new Mutator(new ExpireFirstDebtVariant(mutatorBase, debt, expiry), mutatorBase);
    }
}
