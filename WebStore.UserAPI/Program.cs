using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebStore.UserInfrastructure.Repositories;
using WebStore.UserAPI.Middlewares;
using WebStore.UserAPI.Extensions;
using WebStore.UserApp.Interfaces.Repositories;
using WebStore.UserApp.Interfaces.Services;
using WebStore.UserAPI.Mappings;
using WebStore.UserApp.Services;

namespace WebStore.UserAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            builder.Services.RegisterMaps();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<UserDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
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

            builder.AddSerilogLogging();

            var app = builder.Build();

            app.UseHangfireDashboard("/hangfire-user");
            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
