using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Grimoire.Api.Models;

namespace Grimoire.Api.Repositories;

public class BookRepository(
    IDynamoDBContext dynamoDbContext,
    IAmazonDynamoDB amazonDynamoDb) 
    : IBookRepository
{
    public async Task SaveBookAsync(Book book) => await dynamoDbContext.SaveAsync(book);

    public async Task DeleteBookAsync(string isbn) => await dynamoDbContext.DeleteAsync<Book>(isbn);

    public Task<Book> GetBookByIsbnAsync(string isbn) => dynamoDbContext.LoadAsync<Book>(isbn);

    public async Task<PagedResults<Book>> GetBooksAsync(int count, string? lastEvaluatedKey)
    {
        var scanRequest = new ScanRequest
        {
            Limit = count,
            TableName = nameof(Book)
        };

        if (lastEvaluatedKey is not null)
        {
            scanRequest.ExclusiveStartKey = new Dictionary<string, AttributeValue>
            {
                { nameof(Book.Isbn), new AttributeValue { S = lastEvaluatedKey } }
            };
        }

        var scanResponse = await amazonDynamoDb.ScanAsync(scanRequest);
        var books = scanResponse.Items.Select(
            item => dynamoDbContext.FromDocument<Book>(
                Document.FromAttributeMap(item)));
    
        if (scanResponse.LastEvaluatedKey.TryGetValue(nameof(Book.Isbn), out AttributeValue? attributeValue))
        {
            lastEvaluatedKey = attributeValue.S;
        }

        return new PagedResults<Book>()
        {
            Items = books,
            LastEvaluatedKey = lastEvaluatedKey
        };
    }
}
