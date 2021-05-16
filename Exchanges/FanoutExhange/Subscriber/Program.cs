using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace FanoutExchange.FanoutExchange.subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://rbgecpsh:cUOQH6YPFuBCcxyMfMTbq478xzhHMqmS@fish.rmq.cloudamqp.com/rbgecpsh");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            //var randomQueueName = channel.QueueDeclare().QueueName;// eğer biz subscriber a göre kuyruk üretmez de bir tan ekuyruk declare edersek, 
            // işlem bittiğinde dahi o kuyruk sistemimizde kalır. Biz Subscriber tarafından kuyruk oluşturulmasını, işi bitince silinmesini istiyoruz.

            var randomQueueName = "log-save-to-database-queue"; // bir tane kuyruğun elimizde kalmasını ve silinmemesini istediğmiz için bu şekilde oluşturduk.
            channel.QueueDeclare(randomQueueName, true, false, false); // log-save-to-database-queue kuyruğunun deklarasyonu yapıldı.
            channel.QueueBind(randomQueueName, "logs-fanout", "", null); // queue name -> randomQueueName, routing key -> logs-fanout -> FanoutExhange türünden exchange imizin ismi
            // her subscriber kendi kuyruğunu oluşturacak ve publisher na channel üzerinden bind olacak, sonrasında işi bitince silinecek.



            channel.BasicQos(0, 1, false);           

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(randomQueueName, false, consumer); // subscriber ın kuyruğa bağlanmasını sağlar

            Console.WriteLine("Listening logs...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) => // e -> kuyruktaki mesaj byte[]
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray()); // byte ları string e çeviriyor.
                Console.WriteLine(message);
                Thread.Sleep(1500);
                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
