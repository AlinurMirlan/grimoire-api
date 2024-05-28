using Grimoire.Api.Infrastructure.Exceptions;
using Grimoire.Api.Models;
using Grimoire.Api.Models.Events;
using Grimoire.Api.Repositories;

namespace Grimoire.Api.Services;

public class BookService : IBookService
{
    private readonly int defaultPageSize;
    private readonly int maxPageSize;
    private readonly IBookRepository bookRepository;
    private readonly IBookEventService bookEventService;

    public BookService(IConfiguration configuration, IBookRepository bookRepository, IBookEventService bookEventService)
    {
        this.bookRepository = bookRepository;
        this.bookEventService = bookEventService;
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
        await bookEventService.FireEvent(new BookCreated(book));
        return book;
    }

    public async Task DeleteBookAsync(string isbn)
    {
        await bookRepository.DeleteBookAsync(isbn);
        await bookEventService.FireEvent(new BookDeleted(new Book() { Isbn = isbn } ));
    }

    public async Task<Book> EditBookAsync(string isbn, Book book)
    {
        _ = await bookRepository.GetBookByIsbnAsync(isbn)
            ?? throw new ConflictException("There is no book with the given ISBN.");

        book.Isbn = isbn;
        await bookRepository.SaveBookAsync(book);
        await bookEventService.FireEvent(new BookUpdated(book));
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
