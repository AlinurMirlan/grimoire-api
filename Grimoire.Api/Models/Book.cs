using Amazon.DynamoDBv2.DataModel;

namespace Grimoire.Api.Models;

public class Book
{
    [DynamoDBHashKey]
    public string Isbn { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
