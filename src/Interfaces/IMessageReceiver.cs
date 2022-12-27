// <copyright file="IMessageReceiver.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;

namespace Carguero.RabbitMQ.Interfaces
{
    /// <summary>
    ///  This interface contains the contracts for receiving messages from the rabbitMQ client.
    /// </summary>
    /// <typeparam name="T">Event Generic type parameter.</typeparam>
    public interface IMessageReceiver<T>
        where T : class
    {
        /// <summary>
        /// Receive event in RabbitMQ.
        /// </summary>
        /// <param name="message">Receive message.</param>
        /// <param name="cancellationToken"> enables cooperative cancellation between threads, thread pool work items, or Task objects.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task ReceiveAsync(T message, CancellationToken cancellationToken);
    }
}
