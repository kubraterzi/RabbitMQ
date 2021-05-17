using System;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Logging;
using RabbitMQWordToPdf.Models;
using Spire.Doc;

namespace RabbitMQWordToPdfConsumer
{
    class Program
    {
        private bool result = false;
        static void Main(string[] args)
        {
            bool result = false;
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://bzwginur:GfUybRIt7pFbohuBU5_PQGrC9cKsDy_r@baboon.rmq.cloudamqp.com/bzwginur");

            using (var connection= factory.CreateConnection())
            {
                using (var channel= connection.CreateModel())
                {
                    channel.ExchangeDeclare("convert_exchange", ExchangeType.Direct, true, false,null);
                    channel.QueueBind("word_to_pdf", "convert_exchange","WordToPdf", null);
                    channel.BasicQos(0,1,false);

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("word_to_pdf", false,consumer); // false -> mesaj tamamlandığı zaman onay gelmeden bellekten silmesin 
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            Console.WriteLine("Received a message from the queue. Message processing...");

                            MessageWordToPdf messageWordToPdf = new MessageWordToPdf();
                            messageWordToPdf = JsonConvert.DeserializeObject<MessageWordToPdf>(Encoding.UTF8.GetString(ea.Body.ToArray()));

                            //create document with Spire.doc
                            Document document = new Document();
                            document.LoadFromStream(new MemoryStream(messageWordToPdf.WordByte), FileFormat.Docx2013); //dinlenen dosyanın formatı

                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                document.SaveToStream(memoryStream, FileFormat.PDF); // dönüştürüleceği format
                                result = EmailSend(messageWordToPdf.Email, memoryStream, messageWordToPdf.FileName);
                            }

                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message); // eğer uygulama sırasında dönüştürme sorunu yaşanırsa, if sorgusuna inemeyeceği için BasicAck ile kuyruktan silinmeyecek.
                            throw;
                        }


                        if (result)
                        {
                            Console.WriteLine("Message processed successfully.");
                            channel.BasicAck(ea.DeliveryTag, false); // başarılı sonuç alındıysa, yalnızca bu mesaj kuyruktan silinecek.
                        }

                    };

                    Console.WriteLine("Click to exit.");
                    Console.ReadLine();
                }
            }
        }

        

        public static bool EmailSend(string email, MemoryStream memoryStream, string fileName)
        {
            try
            {
                memoryStream.Position = 0; // email e koyabilmek için 0. satırdan yazmaya başlaması gerekir. Eğer bu position ayarı verilmezse, 0kb olarak aktarım sağlar. Başlangıç noktasını belirlemek zorunludur.

                //create pdf document
                System.Net.Mime.ContentType contentType =
                    new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf); // attach ile kullanacğaımız dosyanın tipini belirledik

                Attachment attach = new Attachment(memoryStream, contentType); //memorystream 0dan itibaren oluşturmaya başlayacak ve pdf formatında olacak.
                attach.ContentDisposition.FileName = $"{fileName}.pdf";

                //send mail
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("kubraterzi0303@gmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Pdf Document Sample";
                mailMessage.Body = "Sample sended.";
                mailMessage.IsBodyHtml = true;
                mailMessage.Attachments.Add(attach);

                //SMTPClient settings
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "kubraterzi0303@gmail.com"; //mail.blabla.net
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("kubraterzi0303@gmail.com", ".kubra450790."); //"admin@blabla.net", "blabla123"
                smtpClient.Send(mailMessage);
                Console.WriteLine($" Sended to:  {email}");
                memoryStream.Close();
                memoryStream.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.InnerException}");
                return false;
            }

           

        }
    }
}
