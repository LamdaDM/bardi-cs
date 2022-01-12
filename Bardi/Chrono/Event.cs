using System.Collections.Generic;
using Bardi.Captures;
using Bardi.User;
using Bardi.User.Mutators;
using Bardi.User.Mutators.Variants;

namespace Bardi.Chrono;

public record Event(long TimePos, IMutatorVariant MutatorVariant) : IComparable<Event>
{
    public EventMemento Trigger(IEnumerable<Account> accounts, IEnumerable<Mutator> mutators)
    {
        var _ = MutatorVariant.Mutate();

        var capturedAccounts = accounts.Select(AccountCapture.FromAccount).ToList();

        var capturedDebts = MutatorCapture.ToCollection(mutators).ToList();

        return new(TimePos, capturedAccounts, capturedDebts);
    }
    
    public int CompareTo(Event? other)
    {
        return (other == null)
            ? 0
            : TimePos == other.TimePos
                ? 0
                : TimePos > other.TimePos
                    ? 1
                    : -1;           }
}