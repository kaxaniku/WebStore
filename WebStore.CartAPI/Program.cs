using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebStore.CartInfrastructure.Repositories;
using WebStore.CartAPI.Middlewares;
using WebStore.CartAPI.Extensions;
using WebStore.CartApp.Interfaces.Repositories;
using WebStore.CartApp.Interfaces.Services;
using WebStore.CartAPI.Mappings;
using WebStore.CartApp.Services;

namespace WebStore.CartAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddDbContext<CartDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("WebStore.CartInfrastructure");
                    }));
            builder.Services.RegisterMaps();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<CartDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
                    cfg.Host(rabbitHost, "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangFireConnection")));

            //builder.AddSerilogLogging();

            var app = builder.Build();

            app.UseHangfireDashboard("/hangfire-cart", new DashboardOptions
            {
                Authorization = [new DashboardNoAuthorizationFilter()]
            });
            //app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
