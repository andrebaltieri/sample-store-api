using Sample.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Sample.Api.Models;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Sample.Api.Security;

namespace Sample.Api
{
    public class Startup
    {
        private const string ISSUER = "b2cbf693";
        private const string AUDIENCE = "1ca52ad5";
        private const string SECRET_KEY = "store.69a9da40-adad-444f-a023-8b46515b97a9";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SECRET_KEY));

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase());
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddAuthorization(options =>
           {
               options.AddPolicy("User", policy => policy.RequireClaim("Store", "User"));
               options.AddPolicy("Admin", policy => policy.RequireClaim("Store", "Admin"));
           });

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = ISSUER;
                options.Audience = AUDIENCE;
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DataContext context)
        {
            loggerFactory.AddConsole();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = ISSUER,

                ValidateAudience = true,
                ValidAudience = AUDIENCE,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc();

            context.Customers.Add(new Customer { Id = Guid.NewGuid(), FirstName = "André", LastName = "Baltieri", Document = "99999999999", Email = "hello@balta.io", Birthdate = DateTime.Now.AddYears(-31), Username = "andrebaltieri", Password = "andrebaltieri" });
            context.SaveChanges();
        }
    }
}
