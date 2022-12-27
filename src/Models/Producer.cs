// <copyright file="Producer.cs" company="Carguero">
// Copyright (c) Carguero. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Carguero.RabbitMQ.Models.Producer
{
    public class Producer
    {
        public string ConnectionUri { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();
    }

    public class Event
    {
        public string Name { get; set; }

        public string Exchange { get; set; }

        public string RouteKey { get; set; }

        public bool Persistent { get; set; }

        public int Priority { get; set; }

        public List<Header> Headers { get; set; } = new List<Header>();
    }

    public class Header
    {
        public string key { get; set; }

        public int Value { get; set; }
    }
}
