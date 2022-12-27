// <copyright file="Consumer.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Carguero.RabbitMQ.Models.Consumer
{
    public class Consumer
    {
        public string ConnectionUri { get; set; }

        public ushort PrefetchCount { get; set; }

        public int ConsumerDispatchConcurrency { get; set; }

        public ushort PrefetchSize { get; set; }

        public bool Global { get; set; }

        public List<Event> Events { get; set; }
    }

    public class DeadLetter
    {
        public string Name { get; set; }

        public string RouteKey { get; set; }

        public bool Exclusive { get; set; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }
    }

    public class Arg
    {
        public string key { get; set; }

        public string Value { get; set; }
    }

    public class Event
    {
        public string Name { get; set; }

        public Exchange Exchange { get; set; }

        public Queue Queue { get; set; }

        public DeadLetter DeadLetter { get; set; }
    }

    public class Exchange
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }

        public List<Arg> Args { get; set; }
    }

    public class Queue
    {
        public string Name { get; set; }

        public string RouteKey { get; set; }

        public bool Exclusive { get; set; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }

        public bool AutoAck { get; set; }

        public bool Requeue { get; set; }

        public List<Arg> Args { get; set; }
    }

    public class Root
    {
        public Consumer Consumer { get; set; }
    }
}
