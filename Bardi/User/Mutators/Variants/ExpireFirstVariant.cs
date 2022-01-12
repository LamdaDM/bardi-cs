using Bardi.Chrono;
using Bardi.User.Mutators.Base;

namespace Bardi.User.Mutators.Variants;

/// <inheritdoc />
/// <summary>
/// <para>Static variant with an expiry date.</para>
/// <para>Gives the mutator non-standard event producing behaviour. </para>
/// </summary>
public class ExpireFirstVariant : StandardVariant
{
    private readonly long _expiryDays;
    
    public override IEnumerable<Event> CreateEvents(long start, long end)
    {
        var localEnd = end;
        var cycle = Base.Cycle;

        if (_expiryDays < end) localEnd = _expiryDays;
        
        if (cycle > localEnd - start || cycle == 0) yield break;

        Base.InitialEvent(start, out var referentialInitialEvent, out var unixInitialEvent);

        var projectionLength = Base
            .ProjectionLength(Base.DifferenceInDays(unixInitialEvent, localEnd));

        for (int i = 0; i < projectionLength; i++) // !!!LOOK HERE!!!
            yield return new Event(referentialInitialEvent + cycle * i, this);
    }

    public ExpireFirstVariant(MutatorBase @base, DateTimeOffset expiryDays) : base(@base)
    {
        _expiryDays = (long)(expiryDays.ToUnixTimeSeconds() * (1/86400m));
    }

    public ExpireFirstVariant(MutatorBase @base, long expiryDays) : base(@base)
    {
        _expiryDays = expiryDays;
    }
}