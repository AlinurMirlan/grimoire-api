using Grimoire.Api.Models.Events;

namespace Grimoire.Api.Infrastructure.Visitors;

public interface IBookVisitor
{
    void Visit(BookDeleted bookDeleted);

    void Visit(BookUpdated bookUpdated);

    void Visit(BookCreated bookCreated);
}
