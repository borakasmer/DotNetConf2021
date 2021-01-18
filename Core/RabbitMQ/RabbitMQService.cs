using Core.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ServiceStack;
using System;
using System.Text;

namespace Core
{
    public class RabbitMQService : IRabbitMQService
    {
        public readonly Microsoft.Extensions.Options.IOptions<DevnotConfig> _devnotConfig;
        public RabbitMQService(Microsoft.Extensions.Options.IOptions<DevnotConfig> devnotConfig)
        {
            _devnotConfig = devnotConfig;
        }

        public bool Post(string channelName, object data)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = _devnotConfig.Value.RabbitMqHostname };
                using (IConnection connection = factory.CreateConnection())
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: channelName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    string message = JsonConvert.SerializeObject(data);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: channelName,
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine($"Gönderilen Object: {JSON.stringify(data)}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}