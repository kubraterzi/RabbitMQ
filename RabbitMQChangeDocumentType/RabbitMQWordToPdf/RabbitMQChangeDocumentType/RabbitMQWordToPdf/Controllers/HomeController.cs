using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQWordToPdf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace RabbitMQWordToPdf.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult WordToPdfPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult WordToPdfPage(WordToPdf wordToPdf) // wordToPdf nesnesi, kullanıcıdan aldığımız file dosyasını ve kullanıcının email bilgisini içerir.
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(_configuration["ConnectionStrings:RabbitMQ"]);
            using (var connection = factory.CreateConnection())
            {
                using (var channel=connection.CreateModel())
                {
                    channel.ExchangeDeclare("convert_exchange", ExchangeType.Direct, true, false, null);
                    channel.QueueDeclare("word_to_pdf", true, false, false);
                    channel.QueueBind("word_to_pdf", "convert_exchange", "WordToPdf");

                    MessageWordToPdf messageWordToPdf = new MessageWordToPdf(); // messageWordToPdf nesnesi, bizim kuyruk üzerinden göndereceğimiz dosya bilgileri ve dosyanın Byte[] halini içerir.

                    using (MemoryStream memoryStream=new MemoryStream()) // veriyi  bellekte saklamak için bir MemoryStream nesnesi oluşturduk.
                    {
                        wordToPdf.File.CopyTo(memoryStream); // kullanıcıdan gelen dosyayı memory de sabitledik.
                        messageWordToPdf.WordByte = memoryStream.ToArray(); // bellekteki dosyayı Byte[] ne çevirdik.
                    }

                    messageWordToPdf.FileName = Path.GetFileNameWithoutExtension(wordToPdf.File.FileName); // word belgesinin pdf hali de aynı ismi taşıyacağı için uzantısız halini çektik.
                    messageWordToPdf.Email = wordToPdf.Email;

                    var messageInfo = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageWordToPdf)); // gönderilecek mesajı Byte[] çevirdik.

                    var properties = channel.CreateBasicProperties(); // mesajın RabbitMQ içerisinde sağlam kalabilmesi için bir property sini alıp bellekte saklamak için Persistent özelliğini true verdik.
                    properties.Persistent = true;

                    channel.BasicPublish("convert_exchange", "WordToPdf",properties, messageInfo);

                    ViewBag.result = "After your word file is converted to pdf, it will be sent to your email.";
                    return View();

                }
            };
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
