using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentCommon;

namespace DocumentCreatorConsumer
{
    class Program
    { 
        public static string createdDocumentQueue = "created_document_queue";
        private static readonly string createDocumentQueue = "create_document_queue";
        static void Main(string[] args)
        {
            RabbitMQClientService rabbitMqClientService = new RabbitMQClientService();

          

            rabbitMqClientService.CreateConnection("amqps://gbnmnylz:KzHtnsE1Fhwa6r9_uO-dbcGxjTVApuUu@clam.rmq.cloudamqp.com/gbnmnylz");
            rabbitMqClientService.MakeDeclaration(createdDocumentQueue);
            rabbitMqClientService.ConsumeFromQueue(createdDocumentQueue);
            Console.WriteLine($"{createdDocumentQueue} listening...");

            Task.Delay(5000);
            Console.WriteLine("Created PDF.");

            rabbitMqClientService.ConsumeFromQueue(createDocumentQueue);
            rabbitMqClientService.WriteToQueue(createDocumentQueue, rabbitMqClientService.Model);
            Task.Delay(5000);
            Console.WriteLine("Sended PDF.");


            Console.ReadLine();
        }
    }
}
