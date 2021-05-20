using APICrudBasica.Data;
using APICrudBasica.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using APICrudBasica.Middles;
using System.Collections.Generic;
using Microsoft.Extensions.Caching;

namespace APICrudBasica
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
            services.AddCors();
            services.AddResponseCompression(opt =>
            {
                opt.Providers.Add<GzipCompressionProvider>();
                opt.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });

            //services.AddResponseCaching();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "APICrudBasica", Version = "v1" ,
                    
                    });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = ParameterLocation.Header
                });
               c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference 
                                { 
                                    Type = ReferenceType.SecurityScheme, 
                                    Id = "Bearer" 
                                }
                            },
                            new string[] {}
 
                    }
                });
            });

            services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("cn")));
            // services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("DataBase"));
            //services.AddScoped<DataContext,DataContext>();
            //services.AddScoped<DataContext,Category>();
            //services.AddScoped<DataContext,Product>();
            //services.AddScoped<DataContext,User>();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                     c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICrudBasica v1");
                     
                     });
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseMiddleware<CacheMiddleware>();
/*
            app.Use(async (ctx, next) => {
                    ctx.Request.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(3600)
                };
                    await next();
                }); 
*/

            app.UseResponseCaching();
            app.UseCors(c => 
                    c.AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowAnyOrigin()
                     
            );

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
