using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Grimoire.Api.Repositories;
using Grimoire.Api.Models;
using System.Security.AccessControl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddSingleton<IBookRepository, BookRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/book/{isbn}", async (IBookRepository bookRepository, string isbn) =>
{
    var book = await bookRepository.GetBookByIsbnAsync(isbn);
    return book is null ? Results.NotFound() : Results.Ok(book);
})
.WithName("GetBook")
.WithOpenApi();

app.MapGet("/books", async (IBookRepository bookRepository, int count, string? lastEvaluatedKey) =>
{
    return await bookRepository.GetBooksAsync(count, lastEvaluatedKey);
})
.WithName("GetBooks")
.WithOpenApi();

app.MapPost("/save/book", async (IBookRepository bookRepository, Book book) =>
{
    await bookRepository.SaveBookAsync(book);
    return Results.Created($"/book/{book.Isbn}", book);
})
.WithName("SaveBook")
.WithOpenApi();

app.MapDelete("/delete/book/{isbn}", async (IBookRepository bookRepository, string isbn) =>
{
    await bookRepository.DeleteBookAsync(isbn);
    return Results.NoContent();
})
.WithName("DeleteBook")
.WithOpenApi();

app.Run();
