using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

const string queueName = "message.two";
const string exchangeName = "messages.exchange";

var factory = new ConnectionFactory { HostName = "localhost" };
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    exchange: exchangeName,
    durable: true,
    autoDelete: false,
    type: ExchangeType.Fanout);

await channel.QueueDeclareAsync(
    queue: queueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

await channel.QueueBindAsync(queueName, exchangeName, string.Empty);

Console.WriteLine("[Consumer Two] Waiting for messages");
await Task.Delay(TimeSpan.FromSeconds(3));

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Received: {message}");

    await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(queueName, autoAck: false, consumer);

Console.ReadLine();