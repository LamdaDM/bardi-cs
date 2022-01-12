namespace Bardi.Captures;

public interface ICapturableMutator
{
    /// <summary>
    /// Copies all mutable fields to MutatorCapture.   
    /// </summary>
    public MutatorCapture Capture();
}