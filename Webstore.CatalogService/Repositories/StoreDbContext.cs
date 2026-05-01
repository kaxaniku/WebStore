using Microsoft.EntityFrameworkCore;
using WebStore.CatalogApp.DTOs;

namespace Webstore.CatalogInfrastructure.Repositories;

public sealed class StoreDbContext : DbContext
{
    public StoreDbContext() { }
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Category>? Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(StoreDbContext).Assembly,
            type => type.Namespace == "Webstore.CatalogInfrastructure.Repositories.EntityConfigurations"
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