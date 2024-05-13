using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Grimoire.Api.Repositories;
using Grimoire.Api.Models;

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

app.MapGet("/book/{isbn}", (IBookRepository bookRepository, string isbn) =>
{
    return bookRepository.GetBookByIsbnAsync(isbn);
})
.WithName("GetBook")
.WithOpenApi();

app.MapGet("/books", (IBookRepository bookRepository, int count, string? lastEvaluatedKey) =>
{
    return bookRepository.GetBooksAsync(count, lastEvaluatedKey);
})
.WithName("GetBooks")
.WithOpenApi();

app.MapPost("/save/book", (IBookRepository bookRepository, Book book) =>
{
    return bookRepository.SaveBookAsync(book);
})
.WithName("SaveBook")
.WithOpenApi();

app.MapDelete("/delete/book/{isbn}", (IBookRepository bookRepository, string isbn) =>
{
    return bookRepository.DeleteBookAsync(isbn);
})
.WithName("DeleteBook")
.WithOpenApi();

app.Run();
