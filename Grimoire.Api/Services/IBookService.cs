using Grimoire.Api.Models;

namespace Grimoire.Api.Services;

public interface IBookService
{
    public Task<Book> AddBookAsync(Book book);

    public Task<PagedResults<Book>> GetBooksAsync(int? count, string? lastEvaluatedKey);

    public Task<Book> GetBookByIsbnAsync(string isbn);

    public Task DeleteBookAsync(string isbn);

    public Task<Book> EditBookAsync(string isbn, Book book);
}
