using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace WorkQueue.WorkQueue.subscriber
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

            channel.BasicQos(0, 1, false);
            // ilk parametre -> herhangi bir boyutta olan mesajlar gönderilebilir
            // ikinci parametre -> tüm subscriber a 1er 1er mesajlar gönderir.(5 verilirse 5er 5er)
            // üçüncü parametre -> false -> her bir subscriber a  1 tane mesaj göndererek, sırayla hepsini gezer. 

            // << true dersek(sayının 1 değil de 6 olduğunu varsayalım) totalde 6 tane mesajı subscriber sayısına göre bölerek, her birine tek seferde 
            // gönderir. (2 subscriber olduğunu varsayalım) aynı anda 3er tane gönderir. (3 birine, 3 birine) (5 olduğunu varsayalım -> 3 birine, 2 birine >>

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("WorkQueue", false, consumer); // subscriber ın kuyruğa bağlanmasını sağlar


            consumer.Received += (object sender, BasicDeliverEventArgs e) => // e -> kuyruktaki mesaj byte[]
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray()); // byte ları string e çeviriyor.
                Console.WriteLine(message);
                Thread.Sleep(1500);
                channel.BasicAck(e.DeliveryTag, false); // veri aktarımı tamamlandıktan sonra mesajı hemen silme, subscriber dan haber bekle. Ulaştırılam tag i
                // RabbitMQ ya gönderiyor, hangi tag ile ulşatıysa kuyruktan bulup siliyor.
                // boolean -> multiple -> true dersek eğer, memory de işlenmiş ama RabbitMQ ya gitmemiş mesajlar varsa, RabbitMQ yu haberdar eder.
                // biz false diyerek, yalnızca ilgili mesajın durumunu bildirmesini istiyoruz.
            };

            Console.ReadLine();
        }
    }
}
