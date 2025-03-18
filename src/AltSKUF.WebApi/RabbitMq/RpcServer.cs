using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RpcServer : IAsyncDisposable
{
    private const string QUEUE_NAME = "test_queue";

    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RpcServer(string connectionString)
    {
        _connectionFactory = new ConnectionFactory { Uri = new Uri(connectionString) };
    }

    public async Task StartAsync()
    {
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: QUEUE_NAME, durable: false, exclusive: false,
            autoDelete: false, arguments: null);
    }

    public async Task<string> SendMessageToClientAsync(string message)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("Channel is not initialized.");
        }

        var replyQueue = await _channel.QueueDeclareAsync(queue: "", durable: false, exclusive: false,
            autoDelete: false, arguments: null);

        var tcs = new TaskCompletionSource<string>();
        var correlationId = Guid.NewGuid().ToString();

        var props = new BasicProperties
        {
            CorrelationId = correlationId,
            ReplyTo = replyQueue.QueueName 
        };

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                tcs.TrySetResult(response);
            }
            return Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(replyQueue.QueueName, true, consumer);

        var messageBytes = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: QUEUE_NAME,
            mandatory: true,
            basicProperties: props,
            body: messageBytes
        );

        Console.WriteLine($" [x] Sent message to client: {message}");

        return await tcs.Task;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
    }
}