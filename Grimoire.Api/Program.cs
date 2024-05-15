using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FluentValidation;
using Grimoire.Api.Infrastructure.Exceptions;
using Grimoire.Api.Infrastructure.FIlters;
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
builder.Services.AddValidatorsFromAssemblyContaining<Book>();

const string corsPolicyName = "CorsPolicy";
builder.Services.AddCors(p => p.AddPolicy(corsPolicyName, build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

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
.AddEndpointFilter<ValidationFilter<Book>>()
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
.AddEndpointFilter<ValidationFilter<Book>>()
.WithName("BookEdit")
.WithOpenApi();

app.Run();
