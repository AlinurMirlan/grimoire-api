using Grimoire.Api.Infrastructure.Exceptions;
using Grimoire.Api.Models;
using Grimoire.Api.Repositories;

namespace Grimoire.Api.Services;

public class BookService : IBookService
{
    private readonly int defaultPageSize;
    private readonly int maxPageSize;
    private readonly IBookRepository bookRepository;

    public BookService(IConfiguration configuration, IBookRepository bookRepository)
    {
        this.bookRepository = bookRepository;
        defaultPageSize = configuration.GetValue<int>("Application:Pagination:DefaultPageSize");
        maxPageSize = configuration.GetValue<int>("Application:Pagination:MaxPageSize");
        if (defaultPageSize <= 0)
        {
            throw new ConfigurationException("Default page size must be greater than 0");
        }

        if (maxPageSize <= 0)
        {
            throw new ConfigurationException("Max page size must be greater than 0");
        }
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        var existingBook = await bookRepository.GetBookByIsbnAsync(book.Isbn);
        if (existingBook is not null)
        {
            throw new ConflictException("There is already a book with the given ISBN.");
        }

        await bookRepository.SaveBookAsync(book);
        return book;
    }

    public Task DeleteBookAsync(string isbn) => bookRepository.DeleteBookAsync(isbn);

    public async Task<Book> EditBookAsync(Book book)
    {
        _ = await bookRepository.GetBookByIsbnAsync(book.Isbn)
            ?? throw new ConflictException("There is no book with the given ISBN.");

        await bookRepository.SaveBookAsync(book);
        return book;
    }

    public Task<Book> GetBookByIsbnAsync(string isbn) => bookRepository.GetBookByIsbnAsync(isbn);

    public Task<PagedResults<Book>> GetBooksAsync(int? count, string? lastEvaluatedKey)
    {
        if (count is null || count <= 0)
        {
            count = defaultPageSize;
        }

        if (count > maxPageSize)
        {
            count = maxPageSize;
        }

        return bookRepository.GetBooksAsync(count.Value, lastEvaluatedKey);
    }
}
