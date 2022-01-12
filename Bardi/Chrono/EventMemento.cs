using Bardi.Captures;

namespace Bardi.Chrono;

public readonly record struct EventMemento(long TimePos,
    IEnumerable<AccountCapture> AccountStates, IEnumerable<MutatorCapture> MutatorStates);