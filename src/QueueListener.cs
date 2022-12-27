// <copyright file="QueueListener.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Carguero.RabbitMQ.Interfaces;
using Carguero.RabbitMQ.Models.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Context;

namespace Carguero.RabbitMQ
{
    internal class QueueListener<T> : BackgroundService
    where T : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Event _event;
        private readonly ILogger<QueueListener<T>> _logger;
        //private readonly IErrorNotifier _errorNotifier;

        public QueueListener(IServiceProvider serviceProvider, Consumer consumer, ILogger<QueueListener<T>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            //_errorNotifier = errorNotifier;

            var factory = new ConnectionFactory
            {
                Uri = new Uri(consumer.ConnectionUri),
                ConsumerDispatchConcurrency = consumer.ConsumerDispatchConcurrency
            };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Channel.BasicQos(consumer.PrefetchSize, consumer.PrefetchCount, consumer.Global);

            _event = consumer.Events.SingleOrDefault(v => v.Name.Equals(typeof(T).Name));

            if (_event is null)
            {
                _logger.LogError("Configuração para o evento não localizada");
                throw new ArgumentNullException($"Configuração para o evento {typeof(T).Name} não localizada");
            }

            Channel.ExchangeDeclare(_event.Exchange.Name, _event.Exchange.Type, _event.Exchange.Durable, _event.Exchange.AutoDelete, _event.Exchange.Args?.ToDictionary(x => x.key, x => (object)x.Value));

            Channel.QueueDeclare(_event.Queue.Name, _event.Queue.Durable, _event.Queue.Exclusive, _event.Queue.AutoDelete, _event.Queue.Args?.ToDictionary(x => x.key, x => (object)x.Value));
            Channel.QueueBind(_event.Queue.Name, _event.Exchange.Name, _event.Queue.RouteKey);

            Channel.QueueDeclare(_event.DeadLetter.Name, _event.DeadLetter.Durable, _event.DeadLetter.Exclusive, _event.DeadLetter.AutoDelete);
            Channel.QueueBind(_event.DeadLetter.Name, _event.Exchange.Name, _event.DeadLetter.RouteKey);
        }

        private IConnection Connection { get; set; }

        private IModel Channel { get; set; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(Channel);

            consumer.Received += async (_, message) =>
            {
                using (LogContext.PushProperty("correlationId", Guid.NewGuid()))
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();

                        var receiver = scope.ServiceProvider.GetRequiredService<IMessageReceiver<T>>();
                        var response = JsonSerializer.Deserialize<T>(message.Body.Span, new JsonSerializerOptions() { WriteIndented = true });

                        _logger.LogInformation("{event} em recebido: {boby}.", typeof(T).Name, Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.None));

                        await receiver.ReceiveAsync(response, stoppingToken);

                        //switch (_errorNotifier.HasNotification())
                        //{
                        //    case true:
                        //        Channel.BasicReject(message.DeliveryTag, _event.Queue.Requeue);
                        //        _logger.LogWarning("{event} interrompido. {notification}", typeof(T).Name, string.Join(",", _errorNotifier.GetNotifications()));
                        //        break;
                        //    default:
                        //        Channel.BasicAck(message.DeliveryTag, _event.Queue.Requeue);
                        //        _logger.LogInformation("{event} processado com sucesso.", typeof(T).Name);
                        //        break;
                        //}
                    }
                    catch (Exception ex)
                    {
                        Channel.BasicReject(message.DeliveryTag, _event.Queue.Requeue);
                        _logger.LogError(ex, "{event} Erro ao processar evento.", typeof(T).Name);
                    }

                    stoppingToken.ThrowIfCancellationRequested();
                }
            };

            Channel.BasicConsume(_event.Queue.Name, _event.Queue.AutoAck, consumer);

            return Task.CompletedTask;
        }
    }
}
