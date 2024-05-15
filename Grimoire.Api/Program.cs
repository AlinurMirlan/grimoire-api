using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Grimoire.Api.Exceptions;
using Grimoire.Api.Models;
using Grimoire.Api.Repositories;
using Grimoire.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddSingleton<IBookService, BookService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/book/{isbn}", async (IBookService bookService, string isbn) =>
{
    var book = await bookService.GetBookByIsbnAsync(isbn);
    return book is null ? Results.NotFound() : Results.Ok(book);
})
.WithName("BookGet")
.WithOpenApi();

app.MapGet("/books", async (IBookService bookService, int? count, string? lastEvaluatedKey) =>
{
    return await bookService.GetBooksAsync(count, lastEvaluatedKey);
})
.WithName("BooksGet")
.WithOpenApi();

app.MapPost("/book/add", async (IBookService bookService, Book book) =>
{
    Book createdBook;
    try
    {
        createdBook = await bookService.AddBookAsync(book);
    }
    catch (ConflictException exception)
    {
        return Results.Conflict(exception.Message);
    }

    return Results.Created($"/book/{book.Isbn}", createdBook);
})
.WithName("BookSave")
.WithOpenApi();

app.MapDelete("/book/delete/{isbn}", async (IBookService bookService, string isbn) =>
{
    await bookService.DeleteBookAsync(isbn);
    return Results.NoContent();
})
.WithName("DeleteBook")
.WithOpenApi();

app.MapPut("/book/edit", async (IBookService bookService, Book book) =>
{
    Book editedBook;
    try
    {
        editedBook = await bookService.EditBookAsync(book);
    }
    catch (ConflictException exception)
    {
        return Results.Conflict(exception.Message);
    }

    return Results.Ok(editedBook);
})
.WithName("BookEdit")
.WithOpenApi();

app.Run();
