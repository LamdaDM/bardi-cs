# About

Bardi is a general financial modelling library based on *mutators*, *accounts* and *assets*, which are defined as:
- **Mutator**: A source of timed events, where each event changes the value of an asset.
- **Asset**: A representation of a value, which either belongs to an account or is used by a mutator. 
If a mutator uses an asset, that mutator may have dynamic behaviour.
- **Account**: A collection of assets and their total value.

### Example

```c#
var account = new Account();
account.RegisterAssets(new List<Asset>
{
    new(0, _account, 0), // After projection: 700
    new(1000, _account, 1), // 8000.
    new(50_000, _account, 2) // 49_000
});

var debtAsset = new Asset(1000);
mutators = new List<Mutator>
{
    MutatorFactory.Create(0, _account.Assets[0], 100, 3, 48), // Standard mutator
    MutatorFactory.Create(1, _account.Assets[1], 2, 5, 50, 64, false), // ExpireFirst Mutator
    MutatorFactory.Create(2, _account.Assets[2], -300, 4, 45, _debtAsset, true, 0), // Debt Mutator
};

ProjectionResultsPacket res;
using (var projection = new Projection())
{
    res = projection.Project(mutators, accounts, 50, 19, 1); 
    // Projects from day 50 to day 69, both zeroed to Epoch. Returns 1 IntervalPointPacket spanning 19 days. 
} // Resets mutators to before projection.

// Total change from mutators should be 700, 7000 and -1000
var expectedMutatorCaptures = new List<MutatorCapture>
{
    new (null, 700, 0),
    new (null, 7_000, 1),
    new (new AssetCapture(0, 0, false), -1_000, 2) 
};

// Matching expected captures and captures from the interval point at day 19.
Assert.AreEqual(
    expectedMutatorCaptures, 
    res.IntervalPoints[0].MutatorCaptures
) // True
```
