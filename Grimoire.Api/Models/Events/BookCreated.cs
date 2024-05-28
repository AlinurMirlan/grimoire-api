namespace Grimoire.Api.Models.Events;

public class BookCreated(Book book) : Event
{
    public string Isbn { get; set; } = book.Isbn;

    public string Title { get; set; } = book.Title;

    public string Description { get; set; } = book.Description;

    public override string StreamId => Isbn;

    public override string Type { get; } = nameof(BookCreated);
}
