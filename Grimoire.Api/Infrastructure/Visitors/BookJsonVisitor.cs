using Grimoire.Api.Models.Events;
using System.Text.Json;

namespace Grimoire.Api.Infrastructure.Visitors;

public class BookJsonVisitor : IBookVisitor
{
    public string? BookJson { get; private set; }

    public void Visit(BookDeleted bookDeleted)
    {
        BookJson = JsonSerializer.Serialize(bookDeleted);
    }

    public void Visit(BookUpdated bookUpdated)
    {
        BookJson = JsonSerializer.Serialize(bookUpdated);
    }

    public void Visit(BookCreated bookCreated)
    {
        BookJson = JsonSerializer.Serialize(bookCreated);
    }
}
