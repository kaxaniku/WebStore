using MassTransit;
using Microsoft.EntityFrameworkCore;
using WebStore.OrderApp.DTOs;

namespace WebStore.OrderInfrastructure.Repositories;

public sealed class OrderDbContext : DbContext
{
    public OrderDbContext() { }
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Order>? Orders { get; set; }
    public DbSet<OrderItem>? OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OrderDbContext).Assembly,
            type => type.Namespace == "WebStore.OrderInfrastructure.Repositories.EntityConfigurations"
        );

        modelBuilder.AddTransactionalOutboxEntities();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionString);
        }
    }
}