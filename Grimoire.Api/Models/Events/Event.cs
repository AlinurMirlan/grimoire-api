using Grimoire.Api.Infrastructure.Visitors;

namespace Grimoire.Api.Models.Events;

public abstract class Event
{
    public abstract string Type { get; }

    public abstract string StreamId { get; }

    public abstract void Visit(IBookVisitor visitor);
}
