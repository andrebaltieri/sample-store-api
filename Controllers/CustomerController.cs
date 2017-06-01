using System;
using System.Linq;
using Sample.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Sample.Api.Models
{
    public class CustomerController : Controller
    {
        private readonly DataContext _context;
        public CustomerController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("v1/customers")]
        [AllowAnonymous]
        public IActionResult Post([FromBody]Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("v1/customers")]
        public IActionResult Get()
        {
            return Ok(_context.Customers.ToList());
        }

        [HttpGet]
        [Route("v1/customers/{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_context.Customers.Find(id));
        }

        [HttpGet]
        [Route("v1/customers/{skip}/{take}")]
        public IActionResult Get(int skip, int take)
        {
            return Ok(_context.Customers.Skip(skip).Take(take).ToList());
        }

        [HttpPut]
        [Route("v1/customers/{id}")]
        public IActionResult Put(Guid id, [FromBody]Customer customer)
        {
            var cx = _context.Customers.Find(id);
            cx.FirstName = customer.FirstName;
            cx.LastName = customer.LastName;
            cx.Document = customer.Document;
            cx.Email = customer.Email;
            cx.Birthdate = customer.Birthdate;
            cx.Username = customer.Username;
            cx.Password = customer.Password;

            _context.Entry(cx).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Route("v1/customers/{id}")]
        public IActionResult Delete(Guid id)
        {
            _context.Customers.Remove(_context.Customers.Find(id));
            _context.SaveChanges();
            return Ok();
        }
    }
}