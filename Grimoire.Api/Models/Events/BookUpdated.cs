using Grimoire.Api.Infrastructure.Visitors;

namespace Grimoire.Api.Models.Events;

public class BookUpdated(Book book) : Event
{
    public string Isbn { get; set; } = book.Isbn;

    public string? Title { get; set; } = book.Title;

    public string? Description { get; set; } = book.Description;

    public override string StreamId => Isbn;

    public override string Type { get; } = nameof(BookUpdated);

    public override void Visit(IBookVisitor visitor)
    {
        visitor.Visit(this);
    }
}
