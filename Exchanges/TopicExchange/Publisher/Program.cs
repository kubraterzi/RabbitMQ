using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace TopicExchange.TopicExchange.publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://rbgecpsh:cUOQH6YPFuBCcxyMfMTbq478xzhHMqmS@fish.rmq.cloudamqp.com/rbgecpsh");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);




            Random random = new Random();
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {       
                LogNames logName1 = (LogNames)random.Next(1, 5);
                LogNames logName2 = (LogNames)random.Next(1, 5);
                LogNames logName3 = (LogNames)random.Next(1, 5);

                var routeKey = $"{logName1}.{logName2}.{logName3}";
                string message = $"log - type : {logName1}-{logName2}-{logName3}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-topic", routeKey, null, messageBody); // kuyrukları subscriber kendisi kullanağı zaman oluşturacağı için kuyruk ismini boş veriyoruz.
                Console.WriteLine(message);
            });

            Console.ReadLine();
        }
    }
}
