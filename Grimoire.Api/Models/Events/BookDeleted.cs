using Grimoire.Api.Infrastructure.Visitors;

namespace Grimoire.Api.Models.Events;

public class BookDeleted(Book book) : Event
{
    public string Isbn { get; set; } = book.Isbn;

    public override string StreamId => Isbn;

    public override string Type { get; } = nameof(BookDeleted);

    public override void Visit(IBookVisitor visitor)
    {
        visitor.Visit(this);
    }
}
