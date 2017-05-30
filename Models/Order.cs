using System;
using System.Collections.Generic;

namespace Sample.Api.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public ICollection<OrderItem> Items { get; set; }

    }
}