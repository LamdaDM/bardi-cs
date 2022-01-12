using Bardi.Captures;
using Bardi.Chrono;
using Bardi.User.Mutators.Base;
using Bardi.User.Mutators.Variants;

namespace Bardi.User.Mutators;

public sealed class Mutator : IMutatorProcessor, ICapturableMutator, IEventFactory
{
    public IMutatorVariant Variant;
    public MutatorBase Base;

    public Mutator(IMutatorVariant variant, MutatorBase @base)
    {
        Variant = variant;
        Base = @base;
    }

    public decimal Mutate() => Variant.Mutate();
    public void ResetToCapture(MutatorCapture capture) => Variant.ResetToCapture(capture);
    public IEnumerable<Event> CreateEvents(long start, long end) => Variant.CreateEvents(start, end);
    public MutatorCapture Capture() => Variant.Capture();
    public long ProjectionLength(long unixInitialEvent) => Base.ProjectionLength(unixInitialEvent);
    public long DifferenceInDays(long start, long end) => Base.DifferenceInDays(start, end);
    public void InitialEvent(long start, out long referentialInitialEvent, out long unixInitialEvent)
    {
        Base.InitialEvent(start, out referentialInitialEvent, out unixInitialEvent);
    }
    public long ReferentialInitialEvent(long start) => Base.ReferentialInitialEvent(start);
}