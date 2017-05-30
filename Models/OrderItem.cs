using System;

namespace Sample.Api.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

    }
}