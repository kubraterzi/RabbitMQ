using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Subscriber.RabbitMQ.subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://rbgecpsh:cUOQH6YPFuBCcxyMfMTbq478xzhHMqmS@fish.rmq.cloudamqp.com/rbgecpsh");
            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare("hello-queue", true, false, false); // eğer bunu silersek subscriber ı ayağa kaldırdığımızda böyle bir kuyruk yoksa hata fırlatır.
                                                                     // silmezsek de eğer publisher içerisinde kuyruk oluşturulmadıysa burada oluşturulur. Eğer publisher da kuyruk oluşturduğumuzdan eminsek burada yazmak zorunda
                                                                     // değiliz. İki bağlantı arasında aynı türde ve isimde oluşturulan kuyrukların, hata vermemesi için aynı isim ve parametrelere sahip olması gerekir.

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue", true, consumer);
            // buradaki ilk parametre -> kuyruk adı
            // ikinci parametre ise -> AutoAck -> kuyruktaki mesajın iletişim tamamlandıktan sonra(Subscriber a ulaştıktan sonra), RabbitMQ üzerinden doğrudan silinmesini istiyorsak
            // true a set ederiz. Ama eğer, iletişimin olumlu olması durumunda, RabbitMQ ya olumlu bir yanıt döndürüp sonrasında silmesini istiyorsak false a set ederiz.


            consumer.Received += Consumer_Received; // RabbitMQ, subscriber a mesaj gönderdikten sonra bir event fırlatıyor(Received)

            Console.ReadLine();


        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine("Gelen Mesaj: " + message);
        }

    }
}
