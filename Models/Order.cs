using System;
using System.Collections.Generic;

namespace Sample.Api.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastUpdate { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public EStatus Status { get; set; }
        public Customer Customer { get; set; }

    }

    public enum EStatus
    {
        Created = 1,
        Paid = 2,
        Shipping = 3,
        Completed = 4,
        Canceled = 5
    }
}