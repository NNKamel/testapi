using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TestAPI.Helpers;

namespace TestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            var permissions = new[] {
                "loggedin", // for signed in
                "manage:forums", // for moderator (is signed in)
                "manage:awebsite", // for admin (is moderator and signed in)
            };
            services.AddCors(options =>
            {
                options.AddPolicy(name: "_corsPolicy",
                    builder => builder
                    // .WithOrigins("http://localhost:4200/")
                    // .WithOrigins("https://inthekitchenfront.azurewebsites.net/")
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    // .AllowCredentials()
                    );
            });

            // for authentication
            // services.AddAuthentication("loggedin");
            // // string domain = $"https://{Configuration["Auth0:Domain"]}/";
            // string domain = "https://localhost:5001/Authentication";
            // services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // }).AddJwtBearer(options =>
            // {
            //     options.Authority = domain;
            //     // options.Audience = "Configuration["Auth0:Audience"]";
            //     // options.Audience = "https://cinephiliacs-api/";
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         NameClaimType = ClaimTypes.NameIdentifier
            //     };
            // });

            services.AddAuthorization(options =>
            {
                for (int i = 0; i < permissions.Length; i++)
                {
                    options.AddPolicy(permissions[i], policy => policy.RequireClaim(permissions[i], "true"));
                }
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("_corsPolicy");

            app.UseMiddleware<TestMiddleware>();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
