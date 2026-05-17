using MassTransit;
using Microsoft.EntityFrameworkCore;
using WebStore.UserApp.DTOs;

namespace WebStore.UserInfrastructure.Repositories;

public sealed class UserDbContext : DbContext
{
    public UserDbContext() { }
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
    public DbSet<User>? Users { get; set; }
    public DbSet<Admin>? Admins { get; set; }
    public DbSet<Customer>? Customers { get; set; }
    public DbSet<Cart>? Carts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UserDbContext).Assembly,
            type => type.Namespace == "WebStore.UserInfrastructure.Repositories.EntityConfigurations"
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