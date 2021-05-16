using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace FanoutExchange.Publisher.FanoutExchange.publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://rbgecpsh:cUOQH6YPFuBCcxyMfMTbq478xzhHMqmS@fish.rmq.cloudamqp.com/rbgecpsh");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-fanout", durable:true,type:ExchangeType.Fanout);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"log - {x}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("logs-fanout", "", null, messageBody); // kuyrukları subscriber kendisi kullanağı zaman oluşturacağı için kuyruk ismini boş veriyoruz.
                Console.WriteLine("Sended message.");
            });

            Console.ReadLine();
        }
    }
}
