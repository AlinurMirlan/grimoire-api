using Amazon.SQS;
using Amazon.SQS.Model;
using Grimoire.Api.Infrastructure.Exceptions;
using Grimoire.Api.Infrastructure.Visitors;
using Grimoire.Api.Models.Events;
using System.Runtime.Serialization;

namespace Grimoire.Api.Services;

public class BookEventService(
    IAmazonSQS amazonSqs,
    IConfiguration configuration,
    BookJsonVisitor bookJsonVisitor) 
    : IBookEventService
{
    private readonly string queueUrl = configuration["Application:EventSource:SQS:QueueUrl"] 
        ?? throw new ConfigurationException(nameof(queueUrl));
    private readonly string sqsAttributeTimeStamp = configuration["Application:EventSource:SQS:Attributes:TimeStamp"] 
        ?? throw new ConfigurationException(nameof(sqsAttributeTimeStamp));

    public Task FireBookEvent(Event bookEvent)
    {
        bookEvent.Visit(bookJsonVisitor);
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = bookJsonVisitor.BookJson 
                ?? throw new SerializationException($"{nameof(Event)} could not be serialized."),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    sqsAttributeTimeStamp,
                    new MessageAttributeValue
                    {
                        DataType = "Number",
                        StringValue = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                    }
                }
            }
        };

        return amazonSqs.SendMessageAsync(request);
    }
}
