using Amazon.SQS;
using Amazon.SQS.Model;
using Grimoire.Api.Infrastructure.Exceptions;
using Grimoire.Api.Models.Events;
using System.Text.Json;

namespace Grimoire.Api.Services;

public class BookEventService(
    IAmazonSQS amazonSqs,
    IConfiguration configuration) 
    : IBookEventService
{
    private readonly string queueUrl = configuration["Application:EventSource:SQS:QueueUrl"] 
        ?? throw new ConfigurationException(nameof(queueUrl));
    private readonly string sqsAttributeTimeStamp = configuration["Application:EventSource:SQS:Attributes:TimeStamp"] 
        ?? throw new ConfigurationException(nameof(sqsAttributeTimeStamp));

    public Task FireEvent(Event @event)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(@event),
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
