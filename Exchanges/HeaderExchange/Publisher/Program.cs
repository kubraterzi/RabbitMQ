using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeaderExchanges.HeaderExchanges.publisher
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

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            Dictionary<string, object> currentHeaders = new Dictionary<string, object>();
            currentHeaders.Add("format", "pdf");
            currentHeaders.Add("shape2", "a4");

            var properties = channel.CreateBasicProperties();
            properties.Headers = currentHeaders;
            properties.Persistent = true; // mesajları kalıcı hale getirir. RabbitMQ resetlense dahi mesajlar kaybolmaz.

            channel.BasicPublish("header-exchange",String.Empty, properties, Encoding.UTF8.GetBytes("Headers are matched."));

            Console.WriteLine("Sended message.");

            Console.ReadLine();
        }
    }
}
