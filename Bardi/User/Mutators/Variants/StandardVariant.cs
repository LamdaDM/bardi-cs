using Bardi.Captures;
using Bardi.Chrono;
using Bardi.User.Mutators.Base;

namespace Bardi.User.Mutators.Variants;

/// <inheritdoc />
public class StandardVariant : IMutatorVariant
{
    protected readonly MutatorBase Base;

    public virtual decimal Mutate()
    {
        if (Base.IsAmountAdd)
        {
            Base.Target.Value += Base.Change;
            return Base.AddChange(Base.Change);
        }

        var vm = Base.Target.Value;
        Base.Target.Value *= Base.Change;

        return Base.AddChange(Base.Target.Value - vm);
    }

    public virtual MutatorCapture Capture()
    {
        var tc = Base.TotalChange;
        
        return new MutatorCapture(null, Base.TotalChange, Base.Idx);
    }

    public virtual void ResetToCapture(MutatorCapture capture)
    {
        if (capture.MutatorIdx == Base.Idx) Base.TotalChange = capture.TotalChange;
    } 

    public virtual IEnumerable<Event> CreateEvents(long start, long end)
    {
        var cycle = Base.Cycle;

        if (cycle > end - start || cycle == 0) yield break; // If no events can be made, return empty.

        Base.InitialEvent(start,
            out var referentialInitialEvent,
            out var unixInitialEvent);

        var projectionLength = Base
            .ProjectionLength(Base.DifferenceInDays(unixInitialEvent, end));

        for (var i = 0; i < projectionLength; i++)
            yield return new Event(referentialInitialEvent + cycle * i, this);
    }

    public StandardVariant(MutatorBase @base)
    {
        Base = @base;
    }
}