using Bardi.Captures;
using Bardi.Chrono;

namespace Bardi.User.Mutators.Variants;

public interface IMutatorVariant : IEventFactory, ICapturableMutator
{
    /// <summary>
    /// Triggers mutation, changing the target's value.
    /// </summary>
    /// <returns>Change occured from mutation.</returns>
    public decimal Mutate();
    
    /// <summary>
    /// Sets all capture-related data to the data stored in the capture.
    /// </summary>
    public void ResetToCapture(MutatorCapture capture);
}