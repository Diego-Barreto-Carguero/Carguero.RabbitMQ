// <copyright file="MessageManager.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Carguero.RabbitMQ.Interfaces;
using Carguero.RabbitMQ.Models.Producer;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Carguero.RabbitMQ
{
    public class MessageManager : IMessageSender, IDisposable
    {
        private readonly ILogger<MessageManager> _logger;

        public MessageManager(Producer producer, ILogger<MessageManager> logger)
        {
            Producer = producer;
            _logger = logger;

            var factory = new ConnectionFactory { Uri = new Uri(Producer.ConnectionUri) };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
        }

        internal IConnection Connection { get; private set; }

        internal IModel Channel { get; private set; }

        internal Producer Producer { get; private set; }

        public Task PublishAsync<T>(T message, int? delayMessage = null)
            where T : class
        {
            var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<object>(message, new JsonSerializerOptions() { WriteIndented = true }));

            var events = Producer.Events.Where(s => s.Name.Equals(typeof(T).Name)).SingleOrDefault();

            if (delayMessage.HasValue)
            {
                AddOrUpdateDelayHeader(events, delayMessage.Value);
            }

            var properties = Channel.CreateBasicProperties();

            properties.Persistent = events.Persistent;
            properties.Priority = Convert.ToByte(events.Priority);
            properties.Headers = events.Headers?.ToDictionary(x => x.key, x => (object)x.Value);

            Channel.BasicPublish(events.Exchange, events.RouteKey, properties, sendBytes.AsMemory());

            _logger.LogInformation("{event} criado com sucesso {body}.", typeof(T).Name, Newtonsoft.Json.JsonConvert.SerializeObject(message));

            return Task.CompletedTask;
        }

        private void AddOrUpdateDelayHeader(Event @event, int delayMessage)
        {
            var delay = "x-delay";

            @event.Headers ??= new List<Header>();

            switch (@event.Headers.Exists(c => c.key.Equals(delay)))
            {
                case true:
                    @event.Headers.Single(v => v.key.Equals(delay)).Value = delayMessage;
                    break;
                default:
                    @event.Headers.Add(new Header { key = delay, Value = delayMessage });
                    break;
            }
        }

        public void Dispose()
        {
            if (Channel.IsOpen)
            {
                Channel.Close();
            }

            if (Connection.IsOpen)
            {
                Connection.Close();
            }

            GC.SuppressFinalize(this);
        }
    }
}
