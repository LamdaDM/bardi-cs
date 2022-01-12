using System.Collections.Generic;
using System.Linq;
using Bardi;
using Bardi.Captures;
using Bardi.User;
using Bardi.User.Mutators;
using NUnit.Framework;

namespace BardiTest;

[TestFixture]
public class Tests
{
    private Account _account;
    private Account _emptyAccount;
    private List<Account> _accounts;
    private List<Mutator> _mutators;
    private Asset _debtAsset;
    
    // START 50, END 69
    // 
    // Mutator[0] = reference date 48, cycle 3:
    //     Events: 51, 54, 57, 60, 63, 66, 69
    // Mutator[1] = reference date 50, cycle 5, expiry 64
    //     Events: 50, 55, 60 (expires before next event (65))
    // Mutator[2] = reference date 45, cycle 4, debt 1000, change 300
    //     Events: 53, 57, 61, 65, 69
    //     Remarks: event 4 is last mutation
    // 50, 51, 53, 54, 55, 57, 60, 61, 63, 65, 66, 69 (12 total)
    [SetUp]
    public void Setup()
    {
        _account = new Account();
        _account.RegisterAssets(new List<Asset>
        {
            new(0, _account, 0), // AP: 700
            new(1000, _account, 1), // AP: 2000, 4000, 8000.
            new(50_000, _account, 2) // AP: 49_000
        });

        _emptyAccount = new Account(1);
        _accounts = new List<Account>
        {
            _account,
            _emptyAccount
        };

        _debtAsset = new Asset(1000);
        
        _mutators = new List<Mutator>
        {
            MutatorFactory.Create(0, _account.Assets[0], 100, 3, 48, true, 0),
            MutatorFactory.Create(1, _account.Assets[1], 2, 5, 50, 64, false, 0),
            MutatorFactory.Create(2, _account.Assets[2], -300, 4, 45, _debtAsset, true, 0),
        };
    }

    private ProjectionResultsPacket ProjectUndisposed(out Projection projection)
    {
        projection = new Projection();
        return projection.Project(_mutators, _accounts, 50, 19, 1);
    }
    
    [Test]
    public void ProjectionResults()
    {
        var res = ProjectUndisposed(out var projection);
        Assert.AreEqual(1, res.IntervalPoints.Count);
        Assert.AreEqual(12, res.EventMementos.Count);
        var accountCapture = AccountCapture.FromAccount(_account);
        var expectedAssetCaptures = new List<AssetCapture>
        {
            new(700, 0, true),
            new(8_000, 1, true),
            new(49_000, 2, true)
        };
        var expectedAccountCapture = new AccountCapture(expectedAssetCaptures, 0);
        
        var mutatorCaptures = MutatorCapture.ToCollection(_mutators).ToList();
        var expectedMutatorCaptures = new List<MutatorCapture>
        {
            new (null, 700, 0),
            new (null, 7_000, 1),
            new (new AssetCapture(0, 0, false), -1_000, 2)
        };

        // Mutators and accounts' states after projection are correct.
        Assert.AreEqual(expectedMutatorCaptures, mutatorCaptures);
        Assert.AreEqual(expectedAccountCapture.AssetCaptures, accountCapture.AssetCaptures); 
        projection.Dispose();

        accountCapture = AccountCapture.FromAccount(_account);
        expectedAssetCaptures = new List<AssetCapture>
        {
            new(0, 0, true),
            new(1_000, 1, true),
            new(50_000, 2, true)
        };
        expectedAccountCapture = new AccountCapture(expectedAssetCaptures, 0);
        
        mutatorCaptures = MutatorCapture.ToCollection(_mutators).ToList();
        expectedMutatorCaptures = new List<MutatorCapture>
        {
            new(null, 0, 0),
            new(null, 0, 1),
            new(new AssetCapture(1000, 0, false), 0, 2)
        };

        // Mutator and accounts should be reset after calling projection.Dispose()
        Assert.AreEqual(expectedMutatorCaptures, mutatorCaptures);
        Assert.AreEqual(expectedAccountCapture.AssetCaptures, accountCapture.AssetCaptures);
    }

    [Test]
    public void Reload()
    {
        var res = ProjectUndisposed(out var projection);
        projection.Dispose();
        
        _mutators[0] = MutatorFactory.Create(0, _account.Assets[0], 200, 3, 48, true, 0);
        var timePos = _mutators[0].ReferentialInitialEvent(50);

        var memento = res.EventMementos.Find(memento => memento.TimePos == timePos);

        using var projection1 = projection = new Projection();
        res = projection.Project(_mutators, _accounts, 50, 19, 1, -memento.TimePos, memento);

        var standardTestTc = res.IntervalPoints[0].MutatorCaptures[0].TotalChange;
        Assert.AreEqual(1400, standardTestTc);
    }
    
    [Test]
    public void FirstEvent()
    {
        var first = Projection.LowestRefInitialEventFromMutators(_mutators, 50);
        Assert.AreEqual(0, first);
    }
}