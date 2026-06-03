using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebStore.OrderInfrastructure.Repositories;
using WebStore.OrderAPI.Middlewares;
using WebStore.OrderAPI.Extensions;
using WebStore.OrderApp.Interfaces.Repositories;
using WebStore.OrderApp.Interfaces.Services;
using WebStore.OrderAPI.Mappings;
using WebStore.OrderApp.Services;

namespace WebStore.OrderAPI
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
            builder.Services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("WebStore.OrderInfrastructure");
                    }));
            builder.Services.RegisterMaps();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderProductService, OrderProductService>();
            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
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

            app.UseHangfireDashboard("/hangfire-order", new DashboardOptions
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
