using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace DocumentCommon
{
    public class RabbitMQClientService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly string createDocumentQueue = "create_document_queue";
        private readonly string createdDocumentQueue = "created_document_queue";
        private readonly string createDocumentExchange = "create_document_exchange";
        public CreateDocumentModel Model { get; set; }
        


        public void CreateConnection(string uriString)
        {
            var connectionFactory = new ConnectionFactory{ Uri = new Uri(uriString) };
            _connection = connectionFactory.CreateConnection();
            CreateChannel();
        }

        public void CreateChannel()
        {
            _channel=  _connection.CreateModel();
        }

        public void MakeDeclaration(string queueName)
        {
            _channel.ExchangeDeclare(createDocumentExchange, ExchangeType.Direct);
            _channel.QueueDeclare(queueName, false, false, false);
            _channel.QueueBind(queueName, createDocumentExchange, queueName);
        }


        public void WriteToQueue(string queueName, CreateDocumentModel documentModel)
        {
            var messageArr = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(documentModel));
            _channel.BasicPublish(createDocumentExchange, queueName, null, messageArr);
        }


        public void ConsumeFromQueue(string queueName)
        {
            var consumerEvent = new EventingBasicConsumer(_channel);
            consumerEvent.Received += ConsumerEvent_Received;
            _channel.BasicConsume(queueName, true, consumerEvent);
        }


        private void ConsumerEvent_Received(object sender, BasicDeliverEventArgs e)
        {
            var model = JsonConvert.DeserializeObject<CreateDocumentModel>(Encoding.UTF8.GetString(e.Body.ToArray()));
            WriteToQueue(createdDocumentQueue, model);
            Model = model;

        }
    }
}
