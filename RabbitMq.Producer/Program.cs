using RabbitMQ.Client;
using System.Text;

const string Queue_Name = "message";

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: Queue_Name,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);


for (int i = 0; i < 10; i++)
{
    var message = $"[{DateTime.UtcNow}] - {Guid.CreateVersion7()}";
    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: Queue_Name,
        mandatory: true,
        basicProperties: new BasicProperties { Persistent = true },
        body: body);

    Console.WriteLine($"Sent: {message}");

    await Task.Delay(3000);
}