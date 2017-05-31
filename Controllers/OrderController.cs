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

        [HttpPost]
        [Route("v1/orders")]
        public IActionResult Post([FromBody]Order order)
        {
            order.Number = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8).ToUpper();

            _context.Orders.Add(order);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("v1/orders")]
        public IActionResult Get()
        {
            return Ok(_context.Orders.ToList());
        }
    }
}