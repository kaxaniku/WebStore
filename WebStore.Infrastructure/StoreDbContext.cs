using Microsoft.EntityFrameworkCore;
using WebStore.Application.DTOs;

namespace Webstore.Infrastructure;

public sealed class StoreDbContext : DbContext
{
    public StoreDbContext() { }
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Cart>? Carts { get; set; }
    public DbSet<CartItem>? CartItems { get; set; }
    public DbSet<Order>? Orders { get; set; }
    public DbSet<OrderItem>? OrderItems { get; set; }
    public DbSet<User>? Users { get; set; }
    public DbSet<Admin>? Admins { get; set; }
    public DbSet<Customer>? Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(StoreDbContext).Assembly,
            type => type.Namespace == "Webstore.Infrastructure.EntityConfigurations"
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionString);
        }
    }
}