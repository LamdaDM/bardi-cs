using Bardi.User;

namespace Bardi.Captures;

public readonly record struct AssetCapture(decimal Value, int AssetIdx, bool IsOwned)
{
    public AssetCapture(Asset asset) : this(asset.Value, asset.Idx, asset.Owner != null)
    {}
}