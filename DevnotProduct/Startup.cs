using AutoMapper;
using Core;
using Core.Models;
using DAL.Entities.DbContexts;
using DAL.PartialEntites;
using Dashboard.Core.Caching;
using Dashboard.Services;
using DevnotProduct.Controllers;
using DevnotProduct.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repository;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevnotProduct
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

            services.AddDbContext<DevnotContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(GeneralRepository<>));
            services.AddTransient<IProductService, ProductService>();

            services.AddTransient<IEncryption, Encryption>();
            services.AddTransient<IRedisCacheService, RedisCacheService>();

            //ElasticSearch
            services.Configure<ElasticConnectionSettings>(Configuration.GetSection("ElasticConnectionSettings"));

            services.AddTransient(typeof(IElasticSearchService<>), typeof(ElasticSearchService<>));
            services.AddTransient(typeof(IElasticAuditService<>), typeof(ElasticAuditService<>));
            services.AddSingleton<ElasticClientProvider>();

            services.AddTransient<IRabbitMQService, RabbitMQService>();

            //Automapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.Configure<DevnotConfig>(Configuration.GetSection("DevnotConfig"));

            services.AddControllersWithViews().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevnotProduct", Version = "v1" });
            });

            services.AddSignalR()
                  .AddJsonProtocol(options => {
                      options.PayloadSerializerOptions.PropertyNamingPolicy = null; });
            //Normalde Hub class'dan dönen gelen data da property isimlerinin ilk harfi küçük geliyor,kendi çeviriyor. Olduğu gibi bıraksın diye bu satırı eklendi. 

            services.AddSingleton<IHubProductDispatcher, HubProductDispatcher>();

            //Normalde WebApi'den dönen json data da property isimlerinin ilk harfi küçük geliyor, kendi çeviriyor. Oldugu gibi biraksin diye bu satir eklendi. 
            services.AddControllersWithViews().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .Build();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevnotProduct v1"));
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("ApiCorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ProductHub>("/productHub");
                endpoints.MapControllers();
            });          

            app.UseAuthorization();
           
        }
    }
}
