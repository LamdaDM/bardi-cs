using Bardi.User.Mutators;

namespace Bardi.Chrono;

public interface IEventFactory
{
    /// <param name="start">Starting time zeroed to Epoch.</param>
    /// <param name="end">Final day of the timespan, zeroed to Epoch.</param>
    public IEnumerable<Event> CreateEvents(long start, long end);
}