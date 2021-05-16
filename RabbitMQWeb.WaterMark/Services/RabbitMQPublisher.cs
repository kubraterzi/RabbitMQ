using RabbitMQ.Client;
using RabbitMQWeb.WaterMark.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQWeb.WaterMark.Services
{
    public class RabbitMQPublisher
    {
        private RabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = _rabbitMQClientService.Connect(); // ClientService ten connect metodu ile RabbitMQ bağlantısını kur, kanal ve gerekli özellikleri oluştur

            var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);

            var bodyBytes = Encoding.UTF8.GetBytes(bodyString);

            var property = channel.CreateBasicProperties();
            property.Persistent = true;

            channel.BasicPublish(exchange:RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingWatermark, 
                basicProperties: property, body: bodyBytes); // kanal, exchange, kuyruk, bind işlemleri tanımlandıktan sonra, exchange gönderilmek üzere paketi hazırlar(bastırır)
        }
    }
}
