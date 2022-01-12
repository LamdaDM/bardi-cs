using Bardi.Captures;
using Bardi.Chrono;
using Bardi.User.Mutators.Base;

namespace Bardi.User.Mutators.Variants;

/// <inheritdoc />
/// <summary>
/// <para>Dynamic variant with an expiry date and an asset representing debt.</para>
/// <para> Gives the mutator non-standard event producing and mutation related behaviour.</para>
/// </summary>
public class ExpireFirstDebtVariant : StandardVariant
{
    private readonly ExpireFirstVariant _expireFirstV;
    private readonly DebtVariant _debtV;

    public ExpireFirstDebtVariant(MutatorBase @base, Asset debt, DateTimeOffset expiry) : base(@base)
    {
        _debtV = new DebtVariant(@base, debt);
        _expireFirstV = new ExpireFirstVariant(@base, expiry);
    }

    public ExpireFirstDebtVariant(MutatorBase @base, Asset debt, long expiryDays) : base(@base)
    {
        _debtV = new DebtVariant(@base, debt);
        _expireFirstV = new ExpireFirstVariant(@base, expiryDays);
    }

    public override decimal Mutate() => _debtV.Mutate();

    public override MutatorCapture Capture() => _debtV.Capture();

    public override void ResetToCapture(MutatorCapture capture) => _debtV.ResetToCapture(capture);

    public override IEnumerable<Event> CreateEvents(long start, long end) => _expireFirstV.CreateEvents(start, end);
}