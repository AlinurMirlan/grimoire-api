namespace Grimoire.Api.Models;

public class PagedResults<T>
{
    public IEnumerable<T> Items { get; set; } = [];

    public string? LastEvaluatedKey { get; set; }
}
