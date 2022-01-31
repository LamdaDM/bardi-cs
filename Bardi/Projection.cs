using Bardi.Captures;
using Bardi.Chrono;
using Bardi.User;
using Bardi.User.Mutators;

namespace Bardi;

public class Projection : IDisposable
{
    private EventMemento? _start;
    private List<Mutator>? _mutators;
    private List<Account>? _accounts;

    /// <summary>
    /// Projects the state of all accounts and mutators over a set of intervals, returning states at each point.
    /// Can be used for a fresh projection, or a redo of an old projection from an event memento. 
    /// </summary>
    /// <param name="mutators">Mutators used to generate events from.</param>
    /// <param name="accounts">Accounts for tracking owned assets.</param>
    /// <param name="start">Starting time in Epoch Days</param>
    /// <param name="intervalLen">Length of the interval period (ie. 30 days).</param>
    /// <param name="intervalCount">How many intervals to project through.</param>
    /// <param name="intervalDelay">Optional: How long to delay counting the first interval.</param>
    /// <param name="memento">Optional: For reloading from a specific point in a previous projection.</param>
    /// <remarks>If reloading through a memento, assumes the same list of mutators and accounts are used.</remarks>
    /// <returns>
    /// A packet of event mementos (max one per day) to reload from, and the state of user data at all interval points.
    /// </returns>
    public ProjectionResultsPacket Project(List<Mutator> mutators, List<Account> accounts, 
        long start, int intervalLen, int intervalCount, long intervalDelay = 0,
        EventMemento? memento = null)
    {
        _accounts = accounts;
        _mutators = mutators;
        
        // TODO: Figure out why passing IEnumerable forces elements to capture values by reference.
        // Passing list is a temporary solution, but unsure what's a good fix without first knowing why on earth
        // passing a more derived type makes it's elements unable to copy values without warning.
        _start = new EventMemento(0,
            AccountCapture.FromAccounts(_accounts),
            MutatorCapture.ToCollection(mutators).ToList()); // TODO: IEnumerable boxing problem marker


        if (memento != null)
        {
            ResetToTimePos(mutators, accounts, memento.Value); // Set affected assets in accounts to the given state 
            start += memento.Value.TimePos;
        }

        var end = start + intervalLen * intervalCount;
        var resultsPacket = new ProjectionResultsPacket
        (
            new List<ProjectionIntervalPoint>(), 
            new List<EventMemento>()
        );
        
        var events = new List<Event>();
            
        // TODO: Convert to observable
        foreach (var mutator in mutators) events.AddRange(mutator.CreateEvents(start, end));

        events.Sort(); // Sort events in chronological order.
        
        long lastPos = -1;
        int counter = 0;
        if (intervalDelay > 0) // If given delay, then delay first interval by amount.
        {
            for (int i = 0; i < events.Count; i++)
            {
                var (timePos, variant) = events[i]; 
                var currentPos = timePos % (intervalLen + intervalDelay);
                // This variable is insignificant and is only to represent current iteration's position in event line.

                if (currentPos != lastPos)
                {
                    var localAccountCaptures = AccountCapture.FromAccounts(accounts);
                    
                    // TODO: IEnumerable boxing problem marker
                    var localDebtCaptures = MutatorCapture.ToCollection(mutators).ToList();
                    resultsPacket.EventMementos
                        .Add(new EventMemento(timePos, localAccountCaptures, localDebtCaptures));    
                } 
                    
                variant.Mutate();
                
                if (currentPos < lastPos)
                {
                    resultsPacket.IntervalPoints.Add( new ProjectionIntervalPoint(
                        AccountCapture.FromAccounts(accounts),
                        MutatorCapture.ToCollection(mutators).ToList(), // TODO: IEnumerable boxing problem marker
                        timePos
                    ));
                    counter = i;
                    break;
                }
                
                lastPos = currentPos;
            }
        }

        long currentIntervalIdx = 0;
        for (var i = counter; i < events.Count; i++)
        {
            var (timePos, variant) = events[i];
            var currentPos = timePos % intervalLen;
            // This variable is insignificant and is only to represent current iteration's position in event line.

            if (currentPos != lastPos)
            {
                var localAccountCaptures = AccountCapture.FromAccounts(accounts);
                
                // TODO: IEnumerable boxing problem marker
                var localDebtCaptures = MutatorCapture.ToCollection(mutators).ToList();
                resultsPacket.EventMementos
                    .Add(new EventMemento(timePos, localAccountCaptures, localDebtCaptures));    
            } 
                
            variant.Mutate();
            
            if (i + 1 >= events.Count)
                resultsPacket.IntervalPoints.Add( new ProjectionIntervalPoint(
                    AccountCapture.FromAccounts(accounts),
                    MutatorCapture.ToCollection(mutators).ToList(), // TODO: IEnumerable boxing problem marker
                    currentIntervalIdx
                ));
            else if (events[i].TimePos % intervalLen < currentPos 
                || currentPos == intervalCount * intervalLen + intervalDelay)
            {
                resultsPacket.IntervalPoints.Add( new ProjectionIntervalPoint(
                    AccountCapture.FromAccounts(accounts),
                    MutatorCapture.ToCollection(mutators).ToList(), // TODO: IEnumerable boxing problem marker
                    currentIntervalIdx
                ));
            }
            
            currentIntervalIdx++;
            lastPos = currentPos;
        }

        return resultsPacket;
    }

    private void ResetToTimePos(List<Mutator> mutators, List<Account> accounts, EventMemento memento)
    {
        foreach (var (assetCaptures, accountIdx) in memento.AccountStates)
        foreach (var assetCapture in assetCaptures)
            if (assetCapture.AssetIdx < accounts[accountIdx].Assets.Count)
            {
                var account = accounts[accountIdx];

                account.Assets[assetCapture.AssetIdx]
                    .ResetToCapture(assetCapture);
            }

        // Set affected assets in debt-type mutators to the given state
        foreach (var state in memento.MutatorStates)
        {
            var idx = state.MutatorIdx;
            if (idx < mutators.Count)
                mutators[idx].ResetToCapture(state);
        }
    }
    
    /// <param name="mutators">Source of events.</param>
    /// <param name="start">Starting time zeroed to Epoch.</param>
    /// <returns>Lowest referential initial event from given mutators.</returns>
    public static long LowestRefInitialEventFromMutators(IList<Mutator> mutators,
        long start)
    {
        if (!mutators.Any()) return 0;
        
        var lowest = mutators[0].ReferentialInitialEvent(start);

        if (mutators.Count > 1)
            for (int i = 1; i < mutators.Count; i++)
            {
                var curr = mutators[i].ReferentialInitialEvent(start);
                if (curr < lowest) lowest = curr;
            }

        return lowest;
    }

    /// <summary>
    /// Resets the state of mutators to before the last projection.
    /// </summary>
    /// <remarks>Only mutators used in the last projection are affected.</remarks>
    public virtual void Dispose()
    {
        if (_mutators != null && _accounts != null && _start != null)
            ResetToTimePos(_mutators, _accounts, _start.Value);
    }
}
    
/// <summary>
/// A packet of event mementos (max one per day) to reload from, and the state of user data at all interval points.
/// </summary>
/// <param name="IntervalPoints">States of user data at each interval.</param>
/// <param name="EventMementos">Event memento representing the time and states of user data at that event.</param>
public readonly record struct ProjectionResultsPacket(List<ProjectionIntervalPoint> IntervalPoints,
    List<EventMemento> EventMementos);

public readonly record struct ProjectionIntervalPoint(List<AccountCapture> AccountCaptures, 
    List<MutatorCapture> MutatorCaptures, long IntervalIdx);