using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuickNote_Data;
using QuickNote_Services.Note;
using QuickNote_Services.Token;
using QuickNote_Services.User;

namespace QuickNote_Web
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
            // Add connection string and dbContext setup
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Sets up dbContext injection and sets up any options wanted to be used in the ApplicationDbContext
            services.AddDbContext<ApplicationDbContext>(options =>  options.UseSqlServer(connectionString));
            
            // Add User Service/Interface for DI
            services.AddScoped<IUserService, UserService>();

            // Add Token Service/Interface for DI
            services.AddScoped<ITokenService, TokenService>();

            // Add Note Service/Interface for DI
            services.AddScoped<INoteService, NoteService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    // Getting the value stored in Issuer from Jwt object in appsettings.json
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    // Getting the value stored in Audience from Jwt object in appsettings.json
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QuickNote_Web", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuickNote_Web v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Add authentication middleware to the IApplicationBuilder, enabling authentication capabilities
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
