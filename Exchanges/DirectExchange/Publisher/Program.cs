using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Publisher.RabbitMQ.publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://rbgecpsh:cUOQH6YPFuBCcxyMfMTbq478xzhHMqmS@fish.rmq.cloudamqp.com/rbgecpsh"); //RabbitMQ ile bağlantı adresidir. Cloud üzerinden, RabbitMQ configuration larından alındı.
            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel(); // açılan bağlantı üzerinde bir iletişim kurmak için kanal oluşturuyoruz.

            channel.QueueDeclare("hello-queue", true, false, false); // bir kuyruk oluşturuyoruz ve isim veriyoruz.
            // ilk boolean ifade -> durable -> eğer true dersek, memory de tutulur. RabbitMQ ya restart atınca kuyruk kaybolmaz. false olduğunda restart ile kaybolur.
            // ikinci boolean ifade -> exclusive -> eğer true dersek, yalnızca bu kanal üzerinden bağlanılabilir. Bu kuyruğa subscriber tarafındaki başka bir kanal 
            // üzerinden de bağlanmak istedğimiz için false demeliyiz.
            // üçüncü boolean ifade -> auto delete -> kuyruğa bağlı olan son subscriber bağlantısı kopacak olursa, kuyruk da silinir. Buna engel olmak için false a set ediyoruz.

            string message = "hello world!"; // byte[] şeklinde gönderildiği için pdf,image, document gibi istenilen her şey gönderilebilir.

            var messageBody = Encoding.UTF8.GetBytes(message); // Türkçe karakter problemi yaşamamak için önlem aldık

            channel.BasicPublish(string.Empty, "hello-queue", null, messageBody); // ilk parametre exchange ismidir, biz kullanmadığımız için boş geçtik. Böylesi durumda, 
            // bir exchange imiz olmadığı için kuyruk artık defaultExchange olarak geçer, bu sebeple buradaki routingkey e kuyruk ismi verilmelidir ki root map e göre gelen mesajı 
            // kuyruğa gönderebilsin.

            Console.WriteLine("Mesaj gönderilmiştir.");

            Console.ReadLine();

        }
    }
}
   