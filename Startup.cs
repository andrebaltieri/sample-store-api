using Sample.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Sample.Api.Models;
using System;

namespace Sample.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase());
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DataContext context)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc();

            context.Customers.Add(new Customer { Id = Guid.NewGuid(), FirstName = "André", LastName = "Baltieri", Document = "99999999999", Email = "hello@balta.io", Birthdate = DateTime.Now.AddYears(-31), Username = "andrebaltieri", Password = "andrebaltieri" });
            context.SaveChanges();
        }
    }
}
