using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLogs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class ReceiveLogs
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // logs how in EmitLog
            channel.ExchangeDeclare("logs", ExchangeType.Fanout);

            // when we supply no parameters to QueueDeclare() we create a non-durable, exclusive, autodelete queue with a generated name
            var queueName = channel.QueueDeclare().QueueName;
            // Tell exchange to send messages to the queue = binding
            channel.QueueBind(queueName, "logs", "");

            Console.WriteLine(" [*] Waiting for logs.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] {0}", message);
            };
            
            channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}