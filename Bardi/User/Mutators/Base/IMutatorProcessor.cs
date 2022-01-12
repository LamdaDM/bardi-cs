namespace Bardi.User.Mutators.Base;

public interface IMutatorProcessor
{
    /// <param name="unixInitialEvent">Time until first event, zeroed to Epoch.</param>
    /// <returns>How many mutations will occur from the source over the projection.</returns>
    public long ProjectionLength(long unixInitialEvent);

    public long DifferenceInDays(long start, long end);
    
    /// <param name="start">Starting time zeroed to Epoch.</param>
    /// <param name="referentialInitialEvent">Time position of the initial event zeroed to start.</param>
    /// <param name="unixInitialEvent">Time of the initial event zeroed to Epoch.</param>
    public void InitialEvent(long start, 
        out long referentialInitialEvent,
        out long unixInitialEvent);
    
    /// <param name="start">Starting time zeroed to Epoch.</param>
    /// <returns>Time position of the initial event zeroed to start.</returns>
    public long ReferentialInitialEvent(long start);
}