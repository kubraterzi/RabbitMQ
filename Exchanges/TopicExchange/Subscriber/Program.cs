using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;

namespace TopicExchange.TopicExchange.subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://rbgecpsh:cUOQH6YPFuBCcxyMfMTbq478xzhHMqmS@fish.rmq.cloudamqp.com/rbgecpsh");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

          //  channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

           // channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var routeKey = "*.Error.*";

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueName, "logs-topic", routeKey); //  Bind etmek -> Subscriber düştüğünde, kuyruk da düşsün. 


            channel.BasicConsume(queueName, false, consumer); // subscriber ın kuyruğa bağlanmasını sağlar

            Console.WriteLine("Listening logs...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) => // e -> kuyruktaki mesaj byte[]
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray()); // byte ları string e çeviriyor.
                Console.WriteLine(message);

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
