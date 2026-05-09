using MassTransit;
using Microsoft.EntityFrameworkCore;
using WebStore.CatalogApp.DTOs;

namespace Webstore.CatalogInfrastructure.Repositories;

public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext() { }
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Category>? Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CatalogDbContext).Assembly,
            type => type.Namespace == "Webstore.CatalogInfrastructure.Repositories.EntityConfigurations"
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