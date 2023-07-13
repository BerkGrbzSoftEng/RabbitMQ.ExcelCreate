using System.Text;
using System.Text.Json;
using RabbitMQ.ExcelCreate.Models;
using RabbitMQ.Client;

namespace RabbitMQ.ExcelCreate.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }

        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel = _rabbitMqClientService.Connect();
            var bodyString = JsonSerializer.Serialize(createExcelMessage);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(RabbitMQClientService.ExChangeName, RabbitMQClientService.RoutingExcel,
                basicProperties: properties, body: bodyByte);
        }
    }
}
