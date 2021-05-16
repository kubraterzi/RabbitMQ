using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace WorkQueue.WorkQueue.publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://rbgecpsh:cUOQH6YPFuBCcxyMfMTbq478xzhHMqmS@fish.rmq.cloudamqp.com/rbgecpsh");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare("WorkQueue", true, false, false);
            // ilk boolean -> durable -> true -> belekte tut, silme.
            // ikinci boolean -> exclusive -> false -> tüm kanallar üzerinden bağlanılabilsin.
            // üçüncü boolean -> autodelete -> true -> bana subscriber dan olumlu yanıt döndürmeden kuyruktan silme.


            Enumerable.Range(1, 150).ToList().ForEach(x =>
            {
                string message = $"WorkQueue{x}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(string.Empty, "WorkQueue", null, messageBody); // BasicPublish -> mesajı tüm parametreleri ile hazırlayan hazırlayan komut
                //WorkQueue ya mesajı gönderir.
                Console.WriteLine("Sended message.");
            });

            Console.ReadLine();
            
        }
    }
}
