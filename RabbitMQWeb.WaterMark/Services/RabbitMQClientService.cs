using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWeb.WaterMark.Services
{
    public class RabbitMQClientService : IDisposable // bağlantıyı kur, exchange oluştur, kuyruk oluştur, kuyruğu exchange e bind et
    {
        private readonly ConnectionFactory _connectionFactory; // connectionFactory nesnesi startup ta new leniyor, orada da appsettings.json üzerinden connectionString çekiyor
        private IConnection _connection;
        private IModel _channel; // _channel bir model olarak oluşturulduğundan, CreateModel metodu IModel interface i tipinde olduğu için bu şekilde tanımla

        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";

        private readonly ILogger<RabbitMQClientService> _logger;


        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            Connect();
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection(); // connection ı factory üzerinden, böylelikle  startup dosyasından doğru oluştur(startup da appsettings.json dosyasından connectionString çekecek)
            if (_channel is { IsOpen:true }) // mevcutta bir kanal varsa return et
            {
                return _channel;
            }

            // yoksa yeni kanal oluştur
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true, false); // Direct türünde ImageDirectExchange isminde bir exchange oluştur

            _channel.QueueDeclare(QueueName, true, false, false, null); // kuyruk oluştur

            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingWatermark); // oluşan kuyruğu ilgili exchange e bind et

            _logger.LogInformation("Connected with RabbitMQ."); // kullanıcıyı bilgilendir.

            return _channel; // oluşturulan tüm bilgileri kanalda topla, kanalı return et
        }

        public void Dispose()
        {
            _channel ?. Close(); // kanalın işi bittiyse kapat
            _channel ?. Dispose(); // bellekten uçur

            _connection ?. Close();// bağlantının işi bittiyse kapat
            _connection ?. Dispose(); // bellekten uçur

            _logger.LogInformation("Connection lost."); // kullanıcıyı bilgilendir
           
        }
    }
}
