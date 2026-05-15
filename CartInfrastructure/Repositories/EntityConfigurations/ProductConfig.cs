using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.CartApp.DTOs;

namespace WebStore.CartInfrastructure.Repositories.EntityConfigurations;

internal class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Id)
                .ValueGeneratedNever();
    }
}
