// <copyright file="IMessageSender.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace Carguero.RabbitMQ.Interfaces
{
    /// <summary>
    ///  This interface contains the contracts for publishing messages to the RabbitMQ client.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Create event in RabbitMQ.
        /// </summary>
        /// <param name="message">Generic object for sending message.</param>
        /// <param name="delayMessage">Delay for sending message between Exchange and Queue, value in milliseconds.</param>
        /// <typeparam name="T">Event Generic type parameter.</typeparam>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task PublishAsync<T>(T message, int? delayMessage = null)
            where T : class;
    }
}
