namespace Bardi.Captures;

public readonly record struct MutatorCapture(AssetCapture? AssetCapture, decimal TotalChange, int MutatorIdx)
{
    public bool TryWithDebt(out AssetCapture? assetCapture, out decimal totalChange, out int mutatorIdx)
    {
        assetCapture = AssetCapture;
        totalChange = TotalChange;
        mutatorIdx = MutatorIdx;
        return AssetCapture != null;
    }

    public static IEnumerable<MutatorCapture> ToCollection(IEnumerable<ICapturableMutator> mutators)
    {
        return mutators.Select(mutator => mutator.Capture());
    }
}