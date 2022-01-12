using Bardi.Captures;
using Bardi.User.Mutators.Base;

namespace Bardi.User.Mutators.Variants;

/// <inheritdoc />
/// <summary>
/// <para>Dynamic variant with an asset representing debt.</para>
/// <para>Gives the mutator non-standard mutation related behaviour. </para>
/// </summary>
public class DebtVariant : StandardVariant
{
    public Asset Debt;
    
    public DebtVariant(MutatorBase @base, Asset debt) : base(@base)
    {
        Debt = debt;
    }

    public override decimal Mutate()
    {
        if (Debt < 0) return Base.AddChange(0);
        if (Debt <= Base.Change * -1)
        {
            var localCh = -Debt.Value;
            Base.Target.Value += localCh;
            Debt.Value = 0;
            return Base.AddChange(localCh);
        }

        Debt.Value += Base.Change;
        Base.Target.Value += Base.Change;
        return Base.AddChange(Base.Change);
    }
    
    public override MutatorCapture Capture()
    {
        return new(new(Debt), Base.TotalChange, Base.Idx);
    }

    public override void ResetToCapture(MutatorCapture capture)
    {
        if (capture.TryWithDebt(out var assetCapture, out var totalChange, out var idx))
        {
            if (idx == Base.Idx)
            {
                Debt.ResetToCapture(assetCapture!.Value);
                Base.TotalChange = totalChange;
                return;
            }
        }

        throw new Exception($"MutatorCapture.{idx} is incompatible with Mutator.{Base.Idx}"); 
    }
}