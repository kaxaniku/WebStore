using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.CatalogApp.DTOs;

namespace Webstore.CatalogInfrastructure.Repositories.EntityConfigurations;

internal class AdminConfig : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.Property(p => p.Id)
                .ValueGeneratedNever();
    }
}
