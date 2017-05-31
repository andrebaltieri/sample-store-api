using System;
using System.Linq;
using Sample.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sample.Api.Models
{
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        public OrderController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("v1/orders")]
        public IActionResult Get()
        {
            return Ok(_context.Orders.OrderByDescending(x => x.Date).ToList());
        }

        [HttpGet]
        [Route("v1/orders/{number}")]
        public IActionResult GetByNumber(string number)
        {
            return Ok(_context.Orders.Include(x => x.Items).Include(x => x.Customer).OrderByDescending(x => x.Date).Where(x => x.Number == number).ToList());
        }

        [HttpGet]
        [Route("v1/orders/status/{status}")]
        public IActionResult GetByStatus(EStatus status)
        {
            return Ok(_context.Orders.OrderByDescending(x => x.Date).Where(x => x.Status == status).ToList());
        }

        [HttpGet]
        [Route("v1/orders/customer/{customer}")]
        public IActionResult GetByStatus(Guid customer)
        {
            return Ok(_context.Orders.OrderByDescending(x => x.Date).Where(x => x.Customer.Id == customer).ToList());
        }

        [HttpPost]
        [Route("v1/orders")]
        public IActionResult Post([FromBody]Order order)
        {
            order.Number = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8).ToUpper();
            order.Status = EStatus.Created;
            order.Date = DateTime.Now;
            order.LastUpdate = DateTime.Now;

            _context.Orders.Add(order);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPut]
        [Route("v1/orders/{number}")]
        public IActionResult Put(string number, [FromBody]Order order)
        {
            var data = _context.Orders.FirstOrDefault(x => x.Number == number);

            data.Status = order.Status;
            data.LastUpdate = DateTime.Now;
           
            _context.Entry(data).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }
    }
}