namespace Grimoire.Api.Models.Events;

public abstract class Event
{
    public abstract string Type { get; }
    public abstract string StreamId { get; }
}
