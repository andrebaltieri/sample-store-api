using System;

namespace Sample.Api.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Image { get; private set; }

    }
}