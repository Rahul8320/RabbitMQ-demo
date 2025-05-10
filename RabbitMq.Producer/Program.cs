using RabbitMQ.Client;
using System.Text;

const string queueName = "message";

var factory = new ConnectionFactory { HostName = "localhost" };
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: queueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

await Task.Delay(TimeSpan.FromSeconds(10));

for (var i = 1; i <= 10; i++)
{
    var message = $"[{DateTime.Now}] - [{i}] {Guid.CreateVersion7()}";
    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: queueName,
        mandatory: true,
        basicProperties: new BasicProperties { Persistent = true },
        body: body);

    Console.WriteLine($"Sent: {message}");

    await Task.Delay(TimeSpan.FromSeconds(10));
}