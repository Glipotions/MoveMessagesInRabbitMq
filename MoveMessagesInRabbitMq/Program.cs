// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


string rabbitMqHost = "localhost";
string userName = "guest";
string password = "12345";

string aQueueName = "ReportTrigger_302_Local";
string bQueueName = "ReportTrigger_302";

// RabbitMQ bağlantısı oluşturalım
var factory = new ConnectionFactory() { HostName = rabbitMqHost,  };
using (var connection = factory.CreateConnection())
{
    using (var channel = connection.CreateModel())
    {
        //// AQueue kuyruğu tanımlayalım
        //channel.QueueDeclare(queue: aQueueName,
        //                     durable: false,
        //                     exclusive: false,
        //                     autoDelete: false,
        //                     arguments: null);

        //// BQueue kuyruğu tanımlayalım
        //channel.QueueDeclare(queue: bQueueName,
        //                     durable: false,
        //                     exclusive: false,
        //                     autoDelete: false,
        //                     arguments: null);

        // AQueue'dan mesajları alıp BQueue'ye aktaralım
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"AQueue'dan alınan mesaj: {message}");

            // AQueue'dan alınan mesajı BQueue'ye gönderelim
            channel.BasicPublish(exchange: "biotekno_all",
                                 routingKey: bQueueName,
                                 basicProperties: null,
                                 body: ea.Body);
        };

        channel.BasicConsume(queue: aQueueName,
                             autoAck: false, // ne olur ne olmaz veri kaybı olmasın diye şimdilik false ile işaretledim.
                             consumer: consumer);

        Console.WriteLine("AQueue'dan BQueue'ye mesaj aktarımı başladı.");
        Console.ReadLine();
    }
}