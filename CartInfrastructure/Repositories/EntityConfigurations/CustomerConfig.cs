using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.CartApp.DTOs;

namespace WebStore.CartInfrastructure.Repositories.EntityConfigurations;

internal class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(p => p.Id)
                .ValueGeneratedNever();
    }
}
