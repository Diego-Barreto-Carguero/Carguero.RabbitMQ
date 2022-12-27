// <copyright file="RabbitMQExtensions.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using Carguero.RabbitMQ.Interfaces;
using Carguero.RabbitMQ.Models.Consumer;
using Carguero.RabbitMQ.Models.Producer;
using Microsoft.Extensions.DependencyInjection;

namespace Carguero.RabbitMQ.Extentions
{
    public static class RabbitMQExtensions
    {
        public static void AddProducer(this IServiceCollection services, Producer producer)
        {
            services.AddSingleton<MessageManager>();
            services.AddSingleton<IMessageSender>(provider => provider.GetService<MessageManager>());
            services.AddSingleton(producer);
        }

        public static IMessagingBuilder AddConsumer(this IServiceCollection services, Consumer consumer)
        {
            services.AddSingleton<IMessageSender>(provider => provider.GetService<MessageManager>());
            services.AddSingleton(consumer);
            //services.AddSingleton<IErrorNotifier, ErrorNotifier>();

            return new DefaultMessagingBuilder(services);
        }

        public static IMessagingBuilder AddReceiver<TObject, TReceiver>(this IMessagingBuilder builder)
            where TObject : class
            where TReceiver : class, IMessageReceiver<TObject>
        {
            builder.Services.AddHostedService<QueueListener<TObject>>();
            builder.Services.AddTransient<IMessageReceiver<TObject>, TReceiver>();

            return builder;
        }
    }
}
