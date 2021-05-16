using ClosedXML.Excel;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FileCreateWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private RabbitMQClientService _rabbitMQClientService;
        private IModel _channel;


        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMQClientService = rabbitMQClientService;
        }




        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }




        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }




        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {

            var createExcelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));
            using var ms = new MemoryStream();

            var wb = new XLWorkbook(); // çalýþma kitabý
            var ds = new DataSet(); //  çalýþma kitabýnýn içerisine ekleyeceðimiz tablo için ortam hazýrlar
            ds.Tables.Add(GetTable("products")); // ortama tabloyu ekler

            wb.Worksheets.Add(ds); // çalýþma sayfasýna dataset i (tabloyu) bastýrdýk
            wb.SaveAs(ms); // stream i kaydettik



            MultipartFormDataContent multipartFormDataContent = new();// excel dosyasýný byte[] a döndürüyor

            multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()),"file", (Guid.NewGuid().ToString() + ".xlsx"));


            var client = new RestClient("https://localhost:44365/api/files/upload?fileId=" + createExcelMessage.FileId);
            client.Timeout = -1;

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            var request = new RestRequest(Method.POST);
            request.AddFile("file", ms.ToArray(), Guid.NewGuid().ToString() + ".xlsx");
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                _logger.LogInformation($"File ( Id : {createExcelMessage.FileId}) was created by successful");
                _channel.BasicAck(@event.DeliveryTag, false);
            }



        }




        private DataTable GetTable(string tableName)
        {
            List<Product> products;

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<NorthwindContext>();
                products = context.Products.ToList();
            }

            DataTable table = new DataTable { TableName = tableName };
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("UnitsInStock", typeof(short));
            table.Columns.Add("QuantityPerUnit", typeof(string));

            products.ForEach(x =>
            {
                table.Rows.Add(x.ProductName, x.UnitPrice, x.UnitsInStock, x.QuantityPerUnit);
            });

            return table;
        }

        

    }
}
