namespace Bardi.User.Mutators.Base;

/// <inheritdoc cref="Bardi.User.Mutators.Base.IMutatorProcessor" />
public class MutatorBase : IMutatorPacket, IMutatorProcessor
{
    public long ProjectionLength(long unixInitialEvent)
    {
        return (unixInitialEvent - unixInitialEvent % Cycle) / Cycle + 1;
    }
    
    public long DifferenceInDays(long start, long end)
    {
        return end - start;
    }
    
    public void InitialEvent(long start,
        out long referentialInitialEvent,
        out long unixInitialEvent)
    {
        var localRef = ReferenceDateDays;
        var localCycle = localRef < start 
            ? Cycle
            : -Cycle;
        
        var top = start + Cycle;
        var bottom = start - Cycle;
        
        if (localRef != start)
            while (localRef > top || localRef < bottom) 
                localRef += localCycle;

        unixInitialEvent = localRef >= start 
            ? localRef
            : localRef + Cycle;
        referentialInitialEvent = unixInitialEvent - start;
    }

    public long ReferentialInitialEvent(long start)
    {
        InitialEvent(start, out var referentialInitialEvent, out _);
        return referentialInitialEvent;
    }
    
    public decimal AddChange(decimal change)
    {
        TotalChange += change;
        return change;
    }

    public int Idx { get; }
    public Asset Target { get; }
    public decimal Change { get; }
    public bool IsAmountAdd { get; }
    public int Cycle { get; }
    public decimal TotalChange { get; set; }
    public long ReferenceDateDays { get; }
    public MutatorBase(int idx, Asset target, decimal change, bool isAmountAdd, int cycle, decimal totalChange,
        long referenceDateDays)
    {
        Idx = idx;
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Change = change;
        IsAmountAdd = isAmountAdd;
        Cycle = cycle;
        TotalChange = totalChange;
        ReferenceDateDays = referenceDateDays; 
    }
    
    public MutatorBase(int idx, Asset target, decimal change, bool isAmountAdd, int cycle, decimal totalChange,
        DateTimeOffset referenceDateDays)
    {
        Idx = idx;
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Change = change;
        IsAmountAdd = isAmountAdd;
        Cycle = cycle;
        TotalChange = totalChange;
        ReferenceDateDays = (long)(referenceDateDays.ToUnixTimeSeconds() * 1/86400m); 
    }
}