﻿using Kinetq.ServiceProvider.Interfaces;

namespace Kinetq.ServiceProvider.Tests.Models
{
    public class Order : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}