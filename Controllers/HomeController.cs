using System;
using System.Linq;
using Sample.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sample.Api.Models
{
    public class HomeController : Controller
    {

        [HttpGet]
        [Route("v1")]
        public IActionResult Get()
        {
            return Ok(new { Version = "0.0.1" });
        }
    }
}