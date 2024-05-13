using Grimoire.Api.Models;

namespace Grimoire.Api.Repositories;

public interface IBookRepository
{
    public Task SaveBookAsync(Book book);

    public Task<PagedResults<Book>> GetBooksAsync(int count, string? lastEvaluatedKey);

    public Task<Book> GetBookByIsbnAsync(string isbn);

    public Task DeleteBookAsync(string isbn);
}
