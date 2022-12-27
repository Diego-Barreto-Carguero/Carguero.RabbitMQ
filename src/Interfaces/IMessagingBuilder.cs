// <copyright file="IMessagingBuilder.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;

namespace Carguero.RabbitMQ.Interfaces
{
    /// <summary>
    ///  This interface contains the contracts for get collection of services.
    /// </summary>
    /// <typeparam name="T">Event Generic type parameter.</typeparam>
    public interface IMessagingBuilder
    {
        /// <summary>
        /// Get collection of services.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
