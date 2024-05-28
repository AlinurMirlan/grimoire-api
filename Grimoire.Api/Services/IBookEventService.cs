using Grimoire.Api.Models.Events;

namespace Grimoire.Api.Services;

public interface IBookEventService
{
    public Task FireEvent(Event @event);
}
