using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Grimoire.Api.Models;

namespace Grimoire.Api.Repositories;

public class BookRepository(
    IDynamoDBContext DynamoDBContext,
    IAmazonDynamoDB AmazonDynamoDB) 
    : IBookRepository
{
    public async Task SaveBookAsync(Book book) => await DynamoDBContext.SaveAsync(book);

    public async Task DeleteBookAsync(string isbn) => await DynamoDBContext.DeleteAsync<Book>(isbn);

    public Task<Book> GetBookByIsbnAsync(string isbn) => DynamoDBContext.LoadAsync<Book>(isbn);

    public async Task<PagedResults<Book>> GetBooksAsync(int count, string? lastEvaluatedKey)
    {
        var scanRequest = new ScanRequest
        {
            Limit = count,
            TableName = nameof(Book),
            ExclusiveStartKey = new Dictionary<string, AttributeValue>()
            {
                { nameof(Book.Isbn), new AttributeValue { S = lastEvaluatedKey } }
            }
        };

        var scanResponse = await AmazonDynamoDB.ScanAsync(scanRequest);
        var books = scanResponse.Items.Select(
            item => DynamoDBContext.FromDocument<Book>(
                Document.FromAttributeMap(item)));
    
        lastEvaluatedKey = scanResponse.LastEvaluatedKey[nameof(Book.Isbn)].S;
        return new PagedResults<Book>()
        {
            Items = books,
            LastEvaluatedKey = lastEvaluatedKey
        };
    }
}
