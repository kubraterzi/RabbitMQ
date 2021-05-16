using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQWeb.WaterMark.Models.Services;
using RabbitMQWeb.WaterMark.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQWeb.WaterMark.BackgroundServices
{
    public class ImageWatermarkProcessBackground : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private ILogger<ImageWatermarkProcessBackground> _logger;
        private IModel _channel; // constructor da set etmek yerine metot içerisinde set edileceği için readonly girilmedi

        public ImageWatermarkProcessBackground(RabbitMQClientService rabbitMQClientService, ILogger<ImageWatermarkProcessBackground> logger)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false); //birer birer değil beşer beşer gönderseydik, bellekte bekleyip teker teker çekerdi, çünkü tek bir instance var.
            return base.StartAsync(cancellationToken);

        }

       
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer); // Event dinlenir

            consumer.Received += Consumer_Received;
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }


        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
            var websiteName = "www.rabbitmq.com";

            // event burada byte[] şeklinde geldiği için önce String ifadeye çevirerek ProductImageCreatedEvent a deseralize edilir.
            var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", productImageCreatedEvent.ImageUrl); // resim yolu alınır

            using var image = Image.FromFile(path); // verilen path e göre file ı verir.

            using var graphic = Graphics.FromImage(image); // resme yazı yazılabilmesi için bir graphic nesnesine çevrilir, ortam hazırlanır

            var font = new Font(FontFamily.GenericMonospace, 42, FontStyle.Bold, GraphicsUnit.Pixel); // yazılacak yazının font bilgileri girilir -> birim olarak piksel kullanılacak(sağdan 30px boşluk bırak gibi)

            var textSize = graphic.MeasureString(websiteName, font); // MeasureString-> string ifadenin ölçülerini çekecek(graphic halinde şuan)

            var color = Color.FromArgb(128,255,255,255); // RGB formatında renk belirlenir

            var brush = new SolidBrush(color); // yazının çizim rengini verir


            var position = new Point(image.Width - ((int)textSize.Width + 30), image.Height - ((int)textSize.Height + 30)); //resmin start koordinatını belirler(y koordinati yazının en üst/başlangıç noktasıdır)

            graphic.DrawString(websiteName, font, brush, position); // graphic türüne dönüştürülen resmin üstüne çizim(yazı) eklenir

            image.Save("wwwroot/images/watermarks/" + productImageCreatedEvent.ImageUrl);

            image.Dispose();
            graphic.Dispose();

            _channel.BasicAck(@event.DeliveryTag, false);// eğer başarılı olursa, channel->Basic.Ack üzerinden bilgi verilsin, RabbitMQ ya yalnızca ilgili mesaj hakkında bilgi verilsin(false) -> feedback yapıyor

            }catch(Exception e)
            {
                _logger.LogError(e.Message);
            }

            return Task.CompletedTask;


        }
    }
}
