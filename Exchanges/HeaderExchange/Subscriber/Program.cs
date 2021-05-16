using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeaderExchanges.HeaderExchanges.subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://rbgecpsh:cUOQH6YPFuBCcxyMfMTbq478xzhHMqmS@fish.rmq.cloudamqp.com/rbgecpsh");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();


            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> desiredHeaders = new Dictionary<string, object>();
            desiredHeaders.Add("format", "pdf");
            desiredHeaders.Add("shape", "a4");
            desiredHeaders.Add("x-match", "any");

            channel.QueueBind(queueName, "header-exchange", String.Empty, desiredHeaders); //  Bind etmek -> Subscriber düştüğünde, kuyruk da düşsün. 


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
