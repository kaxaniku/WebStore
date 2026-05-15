using MassTransit;
using Microsoft.EntityFrameworkCore;
using WebStore.CartApp.DTOs;

namespace WebStore.CartInfrastructure.Repositories;

public sealed class CartDbContext : DbContext
{
    public CartDbContext() { }
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }
    public DbSet<Cart>? Carts { get; set; }
    public DbSet<CartItem>? CartItems { get; set; }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Customer>? Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CartDbContext).Assembly,
            type => type.Namespace == "WebStore.CartInfrastructure.Repositories.EntityConfigurations"
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