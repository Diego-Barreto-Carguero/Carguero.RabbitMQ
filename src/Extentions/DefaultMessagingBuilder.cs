// <copyright file="DefaultMessagingBuilder.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using Carguero.RabbitMQ.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Carguero.RabbitMQ.Extentions
{
    public class DefaultMessagingBuilder : IMessagingBuilder
    {
        public DefaultMessagingBuilder(IServiceCollection services)
            => Services = services;

        public IServiceCollection Services { get; }
    }
}
