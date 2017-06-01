using System;
using System.Linq;
using Sample.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sample.Api.Models
{
    public class ProductController : Controller
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("v1/products")]
        public IActionResult Post([FromBody]Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("v1/products")]
        public IActionResult Get()
        {
            return Ok(_context.Products.OrderBy(x => x.Title).ToList());
        }

        [HttpGet]
        [Route("v1/products/{skip}/{take}")]
        public IActionResult Get(int skip, int take)
        {
            return Ok(_context.Products.OrderBy(x => x.Title).Skip(skip).Take(take).ToList());
        }

        [HttpGet]
        [Route("v1/products/{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_context.Products.Where(x=>x.Id == id).FirstOrDefault());
        }

        [HttpPut]
        [Route("v1/products/{id}")]
        public IActionResult Put(Guid id, [FromBody]Product product)
        {
            var prd = _context.Products.Find(id);
            prd.Title = product.Title;
            prd.Price = product.Price;
            prd.Image = product.Image;

            _context.Entry(prd).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Route("v1/products/{id}")]
        public IActionResult Delete(Guid id)
        {
            _context.Products.Remove(_context.Products.Find(id));
            _context.SaveChanges();
            return Ok();
        }
    }
}